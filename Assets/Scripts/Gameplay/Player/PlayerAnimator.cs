using UnityEngine;
using System.Collections;

public class PlayerAnimator : MonoBehaviour
{
	private Animator animator;
	private PlayerInputManager playerInputManager;
	private Rigidbody2D body;

	void Awake ()
	{
		animator = GetComponentInChildren<Animator> ();
		playerInputManager = GetComponent<PlayerInputManager> ();
		body = GetComponent<Rigidbody2D> ();
	}

	void OnEnable()
	{
		playerInputManager.Move += MovePlayer;
	}

	void OnDisable()
	{
		playerInputManager.Move -= MovePlayer;
	}

	private void MovePlayer (float x, bool jump, bool isGrounded, Vector2 platformVelocity)
	{
		animator.SetBool ("grounded", isGrounded);
		animator.SetBool ("move", x != 0);
		animator.SetFloat ("vx", Mathf.Abs(x));
		animator.SetFloat ("vy", body.velocity.y);
	}
}

