using UnityEngine;
using System.Collections;

public class PlayerAnimator : MonoBehaviour
{
	private Animator animator;
	private PlayerInputManager playerInputManager;

	void Awake ()
	{
		animator = GetComponent<Animator> ();
		playerInputManager = GetComponent<PlayerInputManager> ();
	}

	void OnEnable()
	{
		playerInputManager.Move += MovePlayer;
	}

	void OnDisable()
	{
		playerInputManager.Move -= MovePlayer;
	}

	private void MovePlayer (float x, bool jump, bool isGrounded)
	{
		if (x != 0)
			animator.SetBool ("PlayerMove", true);
		else
			animator.SetBool ("PlayerMove", false);
	}
}

