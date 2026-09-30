/*
* Copyright (c) ArtoGames
* PraveenKrishnaGames
*/

using UnityEngine;

public class PushableAndPullable : MonoBehaviour
{
	[SerializeField] float distance;
	[SerializeField] LayerMask boxMask;
	GameObject box;
	[SerializeField] Transform rayPushable;
	[SerializeField] PlayerController playerController;
    bool dragged = false;
    bool changeSide = false;
	private void Update()
	{
        Physics2D.queriesStartInColliders = false;
		RaycastHit2D hit = Physics2D.Raycast(rayPushable.position, Vector2.right * transform.localScale.x, distance, boxMask);
		if (hit.collider != null&&GameInput.GrabPressed&&!dragged)
		{
            dragged = true;
			box = hit.collider.gameObject;

            box.GetComponent<Rigidbody2D>().mass = 1;
            box.gameObject.AddComponent<FixedJoint2D>();
            box.GetComponent<FixedJoint2D>().connectedBody = this.GetComponent<Rigidbody2D>();
           // playerController.;
            Debug.Log("Grabbing");
           // ChangeSide(changeSide);
		}
        else if (!GameInput.GrabHeld&&dragged)
        {
            dragged = false;
            box.GetComponent<FixedJoint2D>().connectedBody = null;
            box.GetComponent<Rigidbody2D>().mass = 500;
          //  playerController.Pushing = false;
            Destroy(box.gameObject.GetComponent<FixedJoint2D>());
        }
    }
	private void OnDrawGizmos()
	{
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
