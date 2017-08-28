using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{          
	private Rigidbody2D rigidBody;
	private bool facingRight = true;

	[SerializeField] private float smoothSpeed;
	[SerializeField] private float jumpForce = 400f;
	[SerializeField] private bool airControl = false;


	void Start ()
	{
		rigidBody = GetComponent <Rigidbody2D> ();
	}

	public void Move (float xDir, bool jump, bool isGrounded)
	{
		// Only control the player if grounded or airControl is turned on
		if (isGrounded || airControl)
		{
			// Move the character
			rigidBody.velocity = new Vector2 (smoothSpeed * xDir, rigidBody.velocity.y);

			// If the input is moving the player right and the player is facing left...
			if (xDir > 0 && !facingRight)
				Flip();
			
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (xDir < 0 && facingRight)
				Flip();
		}

		// If the player should jump
		if (isGrounded && jump)
		{
			// Add a vertical force to the player.
			isGrounded = false;
			rigidBody.AddForce(new Vector2(0f, jumpForce));
		}
	}
		
	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}