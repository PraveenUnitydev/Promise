/*
* Copyright (c) ArtoGames
* PraveenKrishnaGames
*/

using UnityEngine;
using DragonBones;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] UnityArmatureComponent PlayerAn;
    bool waiting;
    float waitTimer = 5;
    bool timer;
    // private IPlayerController _player;
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip[] _footsteps;

    PlayerController characterController;

    private void Awake()
    {
        characterController = GetComponent<PlayerController>();
        if (PlayerAn == null)
            PlayerAn = GetComponentInChildren<UnityArmatureComponent>();
    }

    private void Start()
    {
        // _player = GetComponentInParent<IPlayerController>();
        Idle();
    }

    private void Update()
    {
      /*  if (_player.Input.X != 0) transform.localScale = new Vector3(_player.Input.X > 0 ? 1 : -1, 1, 1);
        if (_player.Input.X != 0 && _player.Grounded)
        {
            Run();

            //Debug.Log(_player.Input.X);
        }
        else
        {
            Idle();
        }



        if (_player.LandingThisFrame)
        {
            // _anim.SetTrigger(GroundedKey);
            _source.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
        }*/

    }



























    enum State
    {
        IDLE, RUN, JUMP, WAITING, PUSH, PULL, JUMPO
    }
    private State state = State.JUMPO;
    public void Idle()
    {
        if (state != State.IDLE)
        {
            PlayerAn.animation.timeScale = 1f;
            PlayerAn.animation.FadeIn("Idle", .25f, -1, 0);
            state = State.IDLE;
        }
    }
    public void Run()
    {
        if (state != State.RUN)
        {
            PlayerAn.animation.timeScale = 1.25f;
            PlayerAn.animation.FadeIn("Run", 0.25f, -1, 0);
            state = State.RUN;
        }
    }
    public void Waiting()
    {
        if (state != State.WAITING)
        {
            PlayerAn.animation.timeScale = 1f;
            PlayerAn.animation.FadeIn("IdleStraight", .25f, -1, 0);
            state = State.WAITING;
        }
    }
    [Header("Jump")]
    [Tooltip("Vertical speed that maps to the start (rising) or end (falling) of the Jump clip. Roughly Blacky's take-off speed.")]
    [SerializeField] float _jumpVelocityRange = 12f;
    [Tooltip("Point in the Jump clip (0-1) shown at the top of the arc.")]
    [Range(0, 1)] [SerializeField] float _jumpApexProgress = 0.5f;
    DragonBones.AnimationState _jumpState;

    // The single Jump clip is shorter than a real jump, so instead of playing it we scrub it by vertical speed:
    // rising shows the first half, the apex the middle, falling the second half. It holds as long as Blacky is airborne.
    public void Jump(float verticalVelocity)
    {
        if (state != State.JUMP)
        {
            PlayerAn.animation.timeScale = 1f;
            _jumpState = PlayerAn.animation.FadeIn("Jump", .1f, 1, 0);
            if (_jumpState != null)
                _jumpState.timeScale = 0f;
            state = State.JUMP;
        }

        if (_jumpState != null)
        {
            float v = Mathf.Clamp(verticalVelocity / _jumpVelocityRange, -1f, 1f);
            float progress = v > 0f
                ? Mathf.Lerp(_jumpApexProgress, 0f, v)
                : Mathf.Lerp(_jumpApexProgress, 1f, -v);
            _jumpState.currentTime = Mathf.Min(progress * _jumpState.totalTime, _jumpState.totalTime - 0.001f);
        }
    }
    public void Jumpo()
    {
        if (state != State.JUMPO)
        {
            PlayerAn.animation.Play("Idle", 1);
            state = State.JUMPO;
        }
    }
    /*private void Update()
    {
        if (!Input.anyKeyDown)
        {
            timer = true;
        }
        else
        {
            timer = false;
            waitTimer = 5;
            waiting = false;
        }
        if (timer == true)
        {
            if (waitTimer > 0)
            {
                waitTimer -= Time.deltaTime;
            }
            else
            {
                waiting = true;
            }
        }
        if (!waiting)
        {
            Idle();
            timer = true;
        }
        else if (waiting)
        {
            timer = false;
            waitTimer = 5;
        }
    }*/
      public void PushOrPull(float grab)
      {
          PlayerAn.animation.timeScale = .75f;
          // Resume a clip paused by PushOrPullHold.
          var current = PlayerAn.animation.lastAnimationState;
          if (current != null)
              current.timeScale = 1f;
          if (characterController.m_FacingRight==true)
          {
              if (grab > 0)
              {
                  if (state != State.PUSH)
                  {
                      PlayerAn.animation.FadeIn("Push", .25f, -1, 0);
                      state = State.PUSH;
                  }
              }
              else if (grab < 0)
              {
                  if (state != State.PULL)
                  {
                      PlayerAn.animation.FadeIn("Pull", .25f, -1, 0);
                      state = State.PULL;
                  }
              }
          } else if (characterController.m_FacingRight == false)
          {
              if (grab < 0)
              {
                  if (state != State.PUSH)
                  {
                      PlayerAn.animation.FadeIn("Push", .25f, -1, 0);
                      state = State.PUSH;
                  }
              }else if (grab>0)
              {
                  if (state != State.PULL)
                  {
                      PlayerAn.animation.FadeIn("Pull", .25f, -1, 0);
                      state = State.PULL;
                  }
              }
          }

      }

    // Holding a box without moving: freeze the push/pull pose instead of dropping to Idle.
    public void PushOrPullHold()
    {
        if (state != State.PUSH && state != State.PULL)
        {
            PlayerAn.animation.FadeIn("Push", .25f, -1, 0);
            state = State.PUSH;
        }
        // Pause only the current clip, so a fade into it still finishes.
        PlayerAn.animation.timeScale = 1f;
        var current = PlayerAn.animation.lastAnimationState;
        if (current != null)
            current.timeScale = 0f;
    }

}
