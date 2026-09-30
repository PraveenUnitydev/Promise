using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [Range(0, .5f)] [SerializeField] private float m_AirMovementSmoothing = .15f; // Smoothing while airborne; higher keeps more momentum
    [Range(0, 1)] [SerializeField] private float m_JumpCutMultiplier = .5f;     // Upward speed kept when jump is released early (variable jump height)
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    private float jumpLockCounter;                                              // Ignores ground contact just after a jump so it can't retrigger.
    const float k_JumpLockTime = .15f;
    private PlayerMovement m_Movement;
    private bool m_JumpRising;                                                  // True from a jump until the player starts falling or releases jump.

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    public bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    public Rigidbody2D m_Rigidbody2D;
    public bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;
    [SerializeField] float distance = 3f;
    Vector2 _directionFacing;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Movement = GetComponent<PlayerMovement>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
    }
    private void Start()
    {
        Physics2D.queriesStartInColliders = false;
    }
    private void Update()
    {
        if (jumpLockCounter > 0f)
            jumpLockCounter -= Time.deltaTime;

        if (m_Grounded && jumpLockCounter <= 0f)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }




        ///Check blockables
        ///
        if (m_FacingRight)
        {
            _directionFacing = transform.right;
        }
        else
        {
            _directionFacing = -transform.right;
        }

        Vector2 pos = new Vector2(transform.position.x, transform.position.y + 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(pos,_directionFacing, distance);
        bool blocked = hit.collider != null && hit.collider.CompareTag("Blockable");
        Debug.DrawRay(pos, _directionFacing * distance, blocked ? Color.red : Color.green);

        // Recomputed every frame so the flags clear once the blockable is out of the ray.
        if (m_Movement != null)
        {
            m_Movement._freezeRight = blocked && m_FacingRight;
            m_Movement._freezeLeft = blocked && !m_FacingRight;
        }
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // Right after a jump the ground check still overlaps the floor; skip it so the jump isn't doubled.
        if (jumpLockCounter > 0f)
            return;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    }


    /// <summary>Applies movement for this physics step. Returns true if a jump started.</summary>
    public bool Move(float move, bool crouch, bool jump, bool jumpHeld = true)
    {
        bool jumped = false;
        // If crouching, check to see if the character can stand up
        if (!crouch)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }

        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {

            // If crouching
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                // Reduce the speed by the crouchSpeed multiplier
                move *= m_CrouchSpeed;

                // Disable one of the colliders when crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                // Enable the collider when not crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }

            // Move the character by finding the target velocity


            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.linearVelocity.y);
            // And then smoothing it out and applying it to the character
            float smoothing = m_Grounded ? m_MovementSmoothing : m_AirMovementSmoothing;
            m_Rigidbody2D.linearVelocity = Vector3.SmoothDamp(m_Rigidbody2D.linearVelocity, targetVelocity, ref m_Velocity, smoothing);
            
            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
              
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
               
            }
        }
        // If the player should jump...
        if (coyoteTimeCounter>0f && jump)
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            // Reset vertical speed so a coyote-time jump is as high as a grounded one.
            m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocity.x, 0f);
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            coyoteTimeCounter = 0f;
            jumpLockCounter = k_JumpLockTime;
            m_JumpRising = true;
            jumped = true;
        }
        // Variable jump height: releasing jump while still rising cuts the jump short (once per jump).
        else if (m_JumpRising)
        {
            if (m_Rigidbody2D.linearVelocity.y <= 0f)
            {
                m_JumpRising = false;
            }
            else if (!jumpHeld)
            {
                m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocity.x, m_Rigidbody2D.linearVelocity.y * m_JumpCutMultiplier);
                m_JumpRising = false;
            }
        }
        return jumped;
    }



    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    private void OnDrawGizmos()
    {
        
    }
}
