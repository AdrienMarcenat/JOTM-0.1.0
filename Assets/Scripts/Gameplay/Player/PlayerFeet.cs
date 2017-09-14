using UnityEngine;
using System.Collections;

public class PlayerFeet : PlayerPart
{
	[SerializeField] private float raycastFeetDistance = 0.25f;
	private Vector2 raycastdirection = new Vector2 (0, -1);

	protected void Update ()
	{
		base.Update ();

		RaycastHit2D raycastHit = Physics2D.Raycast (transform.position, raycastdirection, raycastFeetDistance, Physics2D.GetLayerCollisionMask(gameObject.layer));
		Debug.DrawRay (transform.position, raycastFeetDistance * raycastdirection, Color.green, 0.01f);
		if (raycastHit.collider != null && raycastHit.collider.tag == "MovingPlatform")
			body.velocity += raycastHit.collider.GetComponent<Rigidbody2D> ().velocity;
	}
}

