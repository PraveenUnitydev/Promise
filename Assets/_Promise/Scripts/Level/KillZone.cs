using UnityEngine;

// Restarts the level when the player enters this trigger.
// Reuses the GUID of the old TB GameManager, whose only job in this project was this kill zone.
public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FindAnyObjectByType<LevelChanger>().LoadCurrentLevel();
        }
    }
}
