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
    public void Jump()
    {
        if (state != State.JUMP)
        {
            PlayerAn.animation.timeScale = 1f;
            PlayerAn.animation.FadeIn("Jump", .1f, 1, 0);
            state = State.JUMP;
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

}
