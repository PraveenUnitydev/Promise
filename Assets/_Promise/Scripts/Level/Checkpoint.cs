using UnityEngine;

// Trigger area that saves progress when Blacky walks through it. Place one at the start of each puzzle room.
// The level start counts as the first checkpoint automatically.
[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    [Tooltip("Where Blacky reappears (his feet). Defaults to this object's position.")]
    [SerializeField] Transform _spawnPoint;

    bool _reached;

    public Vector3 SpawnPosition => _spawnPoint != null ? _spawnPoint.position : transform.position;

    void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_reached || !other.CompareTag("Player") || RespawnManager.Instance == null)
            return;
        _reached = true;
        RespawnManager.Instance.SetCheckpoint(this);
    }

    void OnDrawGizmos()
    {
        Vector3 p = SpawnPosition;
        Gizmos.color = _reached ? Color.green : new Color(1f, 0.85f, 0.2f);
        Gizmos.DrawLine(p, p + Vector3.up * 2f);
        Gizmos.DrawLine(p + Vector3.up * 2f, p + new Vector3(0.7f, 1.7f));
        Gizmos.DrawLine(p + new Vector3(0.7f, 1.7f), p + Vector3.up * 1.4f);
    }
}
