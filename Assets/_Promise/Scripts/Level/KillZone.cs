using UnityEngine;

// Kills the player on contact: pits, spikes, hazards. Blacky respawns at the last checkpoint.
// Reuses the GUID of the old TB GameManager, whose only job in this project was this kill zone.
public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            Kill();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            Kill();
    }

    static void Kill()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.KillPlayer();
        else
            FindAnyObjectByType<LevelChanger>().LoadCurrentLevel();
    }
}
