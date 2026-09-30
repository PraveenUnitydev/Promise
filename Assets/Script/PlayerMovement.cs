using System.Collections;
using System.Collections.Generic;
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

    public bool _freezeLeft;
    public bool _freezeRight;

    public bool _freeze = false;

    bool _jump = false;
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
            horizontalMove = Input.GetAxisRaw("Horizontal");

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _jump = true;
            }
        }
        else
        {
            // Stop instead of carrying the last input while paused or frozen.
            horizontalMove = 0;
            _jump = false;
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
        _controller.Move(horizontalMove, false, _jump);
        _jump = false;
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
