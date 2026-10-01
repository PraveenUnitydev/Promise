using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Per-level death and checkpoint handling. Created automatically in any scene that contains the player, so levels
// don't need to place it. On death: freeze the player, fade to black, rewind every IResettable to its state at the
// last checkpoint, move the player to the checkpoint and fade back in. The scene is never reloaded.
public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance { get; private set; }

    [Tooltip("Pause after dying before the screen fades out (time for a death animation).")]
    [SerializeField] float _deathDelay = 0.6f;
    [SerializeField] float _fadeOutTime = 0.35f;
    [SerializeField] float _fadeInTime = 0.5f;

    public event Action PlayerDied;
    public event Action PlayerRespawned;

    public bool IsRespawning { get; private set; }
    public Checkpoint CurrentCheckpoint { get; private set; }

    PlayerMovement _player;
    PlayerController _controller;
    Rigidbody2D _playerBody;
    Vector3 _spawnPosition;
    Coroutine _fadeIn;
    readonly List<IResettable> _resettables = new List<IResettable>();

    static bool s_Hooked;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        Instance = null;
        s_Hooked = false;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        if (!s_Hooked)
        {
            SceneManager.sceneLoaded += (scene, mode) => EnsureForScene();
            s_Hooked = true;
        }
        EnsureForScene();
    }

    static void EnsureForScene()
    {
        if (Instance == null && FindAnyObjectByType<PlayerMovement>() != null)
            new GameObject("RespawnManager").AddComponent<RespawnManager>();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void Start()
    {
        _player = FindAnyObjectByType<PlayerMovement>();
        if (_player == null)
        {
            Destroy(gameObject);
            return;
        }
        _controller = _player.GetComponent<PlayerController>();
        _playerBody = _player.GetComponent<Rigidbody2D>();
        _spawnPosition = _player.transform.position;

        foreach (var behaviour in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (behaviour is IResettable resettable)
                _resettables.Add(resettable);
        }

        // The level start is the first checkpoint.
        CaptureAll();
    }

    public void SetCheckpoint(Checkpoint checkpoint)
    {
        if (checkpoint == CurrentCheckpoint || IsRespawning)
            return;
        CurrentCheckpoint = checkpoint;
        _spawnPosition = checkpoint.SpawnPosition;
        CaptureAll();
    }

    public void KillPlayer()
    {
        if (IsRespawning || _player == null)
            return;
        if (_fadeIn != null)
            StopCoroutine(_fadeIn);
        StartCoroutine(RespawnRoutine());
    }

    void CaptureAll()
    {
        foreach (var r in _resettables)
        {
            if (r as UnityEngine.Object != null)
                r.CaptureState();
        }
    }

    IEnumerator RespawnRoutine()
    {
        IsRespawning = true;
        _player._freeze = true;
        _controller.ResetMotion();
        // Hold the body where it died; no more collisions or kill-zone hits while dead.
        _playerBody.simulated = false;
        PlayerDied?.Invoke();

        yield return new WaitForSeconds(_deathDelay);
        yield return ScreenFader.Instance.FadeTo(1f, _fadeOutTime);

        // Off any moving platform before the platforms rewind.
        _player.transform.SetParent(null);
        _player.transform.position = _spawnPosition;
        foreach (var r in _resettables)
        {
            if (r as UnityEngine.Object != null)
                r.RestoreState();
        }
        _playerBody.simulated = true;
        _controller.ResetMotion();
        Physics2D.SyncTransforms();

        // Let physics settle one step before showing the level again.
        yield return new WaitForFixedUpdate();
        _player._freeze = false;
        IsRespawning = false;
        PlayerRespawned?.Invoke();

        // Separate so a death during the fade-in can interrupt it.
        _fadeIn = StartCoroutine(ScreenFader.Instance.FadeTo(0f, _fadeInTime));
    }
}
