using System.Collections.Generic;
using UnityEngine;

// Lever, pressure plate or trigger area. Needs a Collider2D (usually a trigger) for the activation area.
//   Pressure plate: Mode = Hold, activators = Player + Pushable (a box can hold it down).
//   Lever:          Require Interact on (press Grab inside the area), Mode = Toggle or OneShot.
//   Trigger area:   Mode = OneShot, activators = Player.
// Occupancy is polled every physics step (not enter/exit events), so it stays correct when objects are teleported
// by a respawn.
[RequireComponent(typeof(Collider2D))]
public class SignalSwitch : SignalSource, IResettable
{
    public enum Mode
    {
        [Tooltip("Turns on once and stays on.")] OneShot,
        [Tooltip("Each activation flips it on/off.")] Toggle,
        [Tooltip("On only while something is in the area (or Grab is held, with Require Interact).")] Hold,
        [Tooltip("Turns on for Duration seconds after each activation.")] Timed
    }

    [Header("Activation")]
    [SerializeField] Mode _mode = Mode.OneShot;
    [Tooltip("Layers that can activate it. Default: Player (9) and Pushable (11).")]
    [SerializeField] LayerMask _activators = (1 << 9) | (1 << 11);
    [Tooltip("Player must press Grab inside the area (lever) instead of just entering it.")]
    [SerializeField] bool _requireInteract;
    [Tooltip("Timed mode: seconds it stays on.")]
    [SerializeField] float _duration = 3f;

    [Header("Feedback")]
    [Tooltip("Objects shown while on (indicator lights). Hidden while off.")]
    [SerializeField] List<GameObject> _showWhenOn = new List<GameObject>();
    [Tooltip("Optional. Gets a bool parameter set while on. Defaults to an Animator on this object.")]
    [SerializeField] Animator _animator;
    [SerializeField] string _animatorBool = "Switch_On";

    Collider2D _area;
    ContactFilter2D _filter;
    readonly List<Collider2D> _overlaps = new List<Collider2D>();
    bool _occupied;
    bool _playerInside;
    bool _interactQueued;
    float _timer;
    bool _hasAnimatorBool;

    bool _savedOn, _savedOccupied;
    float _savedTimer;

    protected override void Awake()
    {
        base.Awake();
        _area = GetComponent<Collider2D>();
        if (_animator == null)
            _animator = GetComponent<Animator>();
        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            foreach (var p in _animator.parameters)
            {
                if (p.name == _animatorBool && p.type == AnimatorControllerParameterType.Bool)
                    _hasAnimatorBool = true;
            }
        }
        _filter = new ContactFilter2D { useLayerMask = true, layerMask = _activators, useTriggers = false };
    }

    void Start()
    {
        OnStateChanged(IsOn);
    }

    void Update()
    {
        if (_requireInteract && _playerInside && GameInput.GrabPressed)
            _interactQueued = true;

        if (_mode == Mode.Timed && IsOn)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
                SetOn(false);
        }
    }

    void FixedUpdate()
    {
        _overlaps.Clear();
        _area.Overlap(_filter, _overlaps);
        bool occupied = _overlaps.Count > 0;
        _playerInside = false;
        foreach (var c in _overlaps)
        {
            if (c.CompareTag("Player"))
                _playerInside = true;
        }

        bool activated = _requireInteract ? _interactQueued : occupied && !_occupied;
        _interactQueued = false;
        _occupied = occupied;

        switch (_mode)
        {
            case Mode.OneShot:
                if (activated)
                    SetOn(true);
                break;
            case Mode.Toggle:
                if (activated)
                    SetOn(!IsOn);
                break;
            case Mode.Hold:
                SetOn(_requireInteract ? _playerInside && GameInput.GrabHeld : occupied);
                break;
            case Mode.Timed:
                if (activated)
                {
                    _timer = _duration;
                    SetOn(true);
                }
                break;
        }
    }

    protected override void OnStateChanged(bool on)
    {
        foreach (var go in _showWhenOn)
        {
            if (go != null)
                go.SetActive(on);
        }
        if (_hasAnimatorBool)
            _animator.SetBool(_animatorBool, on);
    }

    public void CaptureState()
    {
        _savedOn = IsOn;
        _savedOccupied = _occupied;
        _savedTimer = _timer;
    }

    public void RestoreState()
    {
        // Restoring the occupancy too means an object rewound onto the plate doesn't count as a new press.
        _occupied = _savedOccupied;
        _timer = _savedTimer;
        _interactQueued = false;
        SetOnSilently(_savedOn);
    }
}
