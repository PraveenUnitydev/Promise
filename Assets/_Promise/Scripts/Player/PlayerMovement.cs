using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController _controller;
    private PushableAndPullable _grab;
    // Blacky is rigged with DragonBones, so animation goes through PlayerAnimationController, not an Animator.
    // The prefab already references the PlayerAnimationController component in this field.
    [SerializeField] PlayerAnimationController anim;

    public float _runSpeed;

    float horizontalMove = 0f;

    [SerializeField] ParticleSystem _launchParticle;

    [Tooltip("A jump pressed this long before landing still happens on landing.")]
    [SerializeField] float _jumpBufferTime = 0.15f;
    [Tooltip("Speed multiplier while pushing or pulling a box.")]
    [Range(0, 1)] [SerializeField] float _grabSpeedMultiplier = 0.45f;

    public bool _freezeLeft;
    public bool _freezeRight;

    public bool _freeze = false;

    float _jumpBufferCounter;
    bool _jumpHeld;

    bool _airborne;
    float _airTime;
    const float k_AirborneDelay = 0.1f;

    bool IsGrabbing => _grab != null && _grab.IsGrabbing;

    private void Start()
    {
        _controller = GetComponent<PlayerController>();
        _grab = GetComponent<PushableAndPullable>();
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

        // No jumping while holding a box, and keep facing it so pulling doesn't turn Blacky around.
        _controller.m_LockFacing = IsGrabbing;
        if (IsGrabbing)
            _jumpBufferCounter = 0f;

        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        if (anim == null)
            return;

        float vy = _controller.m_Rigidbody2D.linearVelocity.y;

        // Enter the air pose only on real vertical motion or after a moment off the ground, so bumps and slope
        // edges don't flicker into it. Once airborne, stay there until landing: at the top of the arc vy is ~0,
        // and dropping out of the air pose there is what made Run and Jump play over each other.
        if (_controller.m_Grounded)
        {
            _airTime = 0f;
            _airborne = false;
        }
        else
        {
            _airTime += Time.deltaTime;
            if (Mathf.Abs(vy) > 0.5f || _airTime > k_AirborneDelay)
                _airborne = true;
        }

        if (_airborne)
        {
            anim.Jump(vy);
        }
        else if (IsGrabbing)
        {
            if (horizontalMove != 0)
                anim.PushOrPull(horizontalMove);
            else
                anim.PushOrPullHold();
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
        float move = IsGrabbing ? horizontalMove * _grabSpeedMultiplier : horizontalMove;
        bool jumped = _controller.Move(move, false, _jumpBufferCounter > 0f, _jumpHeld);
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
