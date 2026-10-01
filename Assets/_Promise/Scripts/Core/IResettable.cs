// Anything whose state must rewind when the player respawns at a checkpoint (boxes, platforms, switches, doors).
// RespawnManager calls CaptureState when a checkpoint is reached (and at level start), and RestoreState on respawn.
public interface IResettable
{
    void CaptureState();
    void RestoreState();
}
