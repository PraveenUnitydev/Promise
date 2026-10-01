/*
* Copyright (c) ArtoGames
* PraveenKrishnaGames
*/

using UnityEngine;

public class PushableAndPullable : MonoBehaviour, IResettable
{
	[SerializeField] float distance;
	[SerializeField] LayerMask boxMask;
	GameObject box;
	[SerializeField] Transform rayPushable;
	[SerializeField] PlayerController playerController;
    bool dragged = false;
    bool changeSide = false;
    PlayerMovement movement;

    public bool IsGrabbing => dragged;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        if (playerController == null)
            playerController = GetComponent<PlayerController>();
    }

	private void Update()
	{
        // Only on the ground and while the player is in control.
        bool canGrab = !GameManagerPk._isPaused
            && (movement == null || !movement._freeze)
            && (playerController == null || playerController.m_Grounded);

        if (dragged)
        {
            if (!GameInput.GrabHeld || !canGrab)
                Release();
            return;
        }

        if (!canGrab || !GameInput.GrabHeld)
            return;

        // Holding Grab while walking into a box grabs it, as well as pressing Grab next to one.
        Physics2D.queriesStartInColliders = false;
		RaycastHit2D hit = Physics2D.Raycast(rayPushable.position, Vector2.right * transform.localScale.x, distance, boxMask);
		if (hit.collider != null && hit.rigidbody != null)
		{
            dragged = true;
			box = hit.rigidbody.gameObject;

            box.GetComponent<Rigidbody2D>().mass = 1;
            var joint = box.AddComponent<FixedJoint2D>();
            joint.connectedBody = GetComponent<Rigidbody2D>();
		}
    }

    void Release()
    {
        dragged = false;
        if (box == null)
            return;
        var joint = box.GetComponent<FixedJoint2D>();
        if (joint != null)
        {
            joint.connectedBody = null;
            Destroy(joint);
        }
        box.GetComponent<Rigidbody2D>().mass = 500;
    }

    // Let go of the box on respawn, before the player and box are moved back.
    public void CaptureState() { }
    public void RestoreState()
    {
        if (dragged)
            Release();
    }
	private void OnDrawGizmos()
	{
		if (rayPushable == null)
			return;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(rayPushable.position, (Vector2)rayPushable.position + Vector2.right * transform.localScale.x * distance);
	}
    ///////
    ///change sides to push
    ///
    void ChangeSide(bool change)
    {
        if (change)
        {
            box.GetComponent<BoxCollider2D>().isTrigger = true;
            Debug.Log("Changing Sides");
            changeSide = true;
        }
        else if(change)
        {
            box.GetComponent<BoxCollider2D>().isTrigger = false;
        }
    }
}
