using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController _controller;
    // Blacky is rigged with DragonBones, so animation goes through PlayerAnimationController, not an Animator.
    // The prefab already references the PlayerAnimationController component in this field.
    [SerializeField] PlayerAnimationController anim;

    public float _runSpeed;

    float horizontalMove = 0f;

    [SerializeField] ParticleSystem _launchParticle;

    [Tooltip("A jump pressed this long before landing still happens on landing.")]
    [SerializeField] float _jumpBufferTime = 0.15f;

    public bool _freezeLeft;
    public bool _freezeRight;

    public bool _freeze = false;

    float _jumpBufferCounter;
    bool _jumpHeld;

    private void Start()
    {
        _controller = GetComponent<PlayerController>();
        if (anim == null)
            anim = GetComponent<PlayerAnimationController>();
        GameManagerPk._isPaused = false;
        Invoke("Launch", .7f);
    }

    private void Update()
    {
        if (!GameManagerPk._isPaused && !_freeze)
        {
            horizontalMove = GameInput.MoveX;
            _jumpHeld = GameInput.JumpHeld;

            if (GameInput.JumpPressed)
                _jumpBufferCounter = _jumpBufferTime;
            else
                _jumpBufferCounter -= Time.deltaTime;
        }
        else
        {
            // Stop instead of carrying the last input while paused or frozen.
            horizontalMove = 0;
            _jumpHeld = false;
            _jumpBufferCounter = 0f;
        }

        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        if (anim == null)
            return;

        // Ignore tiny vertical motion so bumps and slope edges don't flicker into the jump pose.
        if (!_controller.m_Grounded && Mathf.Abs(_controller.m_Rigidbody2D.linearVelocity.y) > 0.5f)
        {
            anim.Jump();
        }
        else if (horizontalMove != 0 && !_freezeLeft && !_freezeRight)
        {
            anim.Run();
        }
        else
        {
            anim.Idle();
        }
    }

    private void FixedUpdate()
    {
        bool jumped = _controller.Move(horizontalMove, false, _jumpBufferCounter > 0f, _jumpHeld);
        if (jumped)
            _jumpBufferCounter = 0f;
    }
    void Launch()
    {
        if (_launchParticle != null)
            _launchParticle.Play();
    }
    public void FreezeDir(string face)
    {
        if (face == "Right")
        {
            _freezeRight = true;
        }else if (face == "Left")
        {
            _freezeLeft = true;
        }
    }

}
