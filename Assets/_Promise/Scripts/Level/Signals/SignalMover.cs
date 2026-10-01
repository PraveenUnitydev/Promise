using UnityEngine;

// Slides between its start position (off) and start + Open Offset (on): doors, gates, lifts, drawbridges.
// With a kinematic Rigidbody2D it moves through physics, which is smoother for things the player stands on.
public class SignalMover : SignalReceiver
{
    [Header("Movement")]
    [Tooltip("Where it moves to when on, relative to where it starts.")]
    [SerializeField] Vector2 _openOffset = new Vector2(0f, 3f);
    [Tooltip("Units per second.")]
    [SerializeField] float _speed = 2f;
    [Tooltip("Parent the player while they stand on it, so they ride along.")]
    [SerializeField] bool _carryPlayer = true;

    Rigidbody2D _body;
    Vector3 _closedPosition;
    float _progress; // 0 = closed (start), 1 = open
    float _savedProgress;

    void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _closedPosition = transform.position;
    }

    protected override void OnSignalChanged(bool on) { }

    void FixedUpdate()
    {
        float target = IsOn ? 1f : 0f;
        if (Mathf.Approximately(_progress, target))
            return;
        float distance = Mathf.Max(_openOffset.magnitude, 0.0001f);
        _progress = Mathf.MoveTowards(_progress, target, _speed * Time.fixedDeltaTime / distance);
        Vector3 position = PositionAt(_progress);
        if (_body != null && _body.bodyType == RigidbodyType2D.Kinematic)
            _body.MovePosition(position);
        else
            transform.position = position;
    }

    Vector3 PositionAt(float progress) => _closedPosition + (Vector3)(_openOffset * progress);

    public override void CaptureState()
    {
        base.CaptureState();
        _savedProgress = _progress;
    }

    public override void RestoreState()
    {
        base.RestoreState();
        _progress = _savedProgress;
        transform.position = PositionAt(_progress);
        if (_body != null)
            _body.position = transform.position;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (_carryPlayer && collision.collider.CompareTag("Player"))
            collision.collider.transform.SetParent(transform);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (_carryPlayer && collision.collider.CompareTag("Player") && collision.collider.transform.parent == transform)
            collision.collider.transform.SetParent(null);
    }

    void OnDrawGizmosSelected()
    {
        Vector3 closed = Application.isPlaying ? _closedPosition : transform.position;
        Vector3 open = closed + (Vector3)_openOffset;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(closed, open);
        Gizmos.DrawWireSphere(open, 0.2f);
    }
}
