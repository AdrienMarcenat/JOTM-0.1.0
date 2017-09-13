using UnityEngine;
using System.Collections;

public class PlayerInputManager : MonoBehaviour
{
	public delegate void MoonlightAction(bool enable);
	public static event MoonlightAction OnMoonLight;

	public delegate void PressButtonAction();
	public event PressButtonAction OnAction;

	public delegate void MoveAction(float x, bool jump, bool isGrounded);
	public event MoveAction Move;

	[SerializeField] private LayerMask whatIsGround;

	private Transform groundCheck;
	private const float groundedRadius = .2f;
	private bool isGrounded;
	private bool moonlightEnable = false;

	private void Awake()
	{
		groundCheck = transform.Find("GroundCheck");
	}
		
	private void FixedUpdate()
	{
		isGrounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
				isGrounded = true;
		}
	}

	protected void Update () 
	{
		if (GameManager.pause)
			return;

		float horizontal = Input.GetAxisRaw ("Horizontal");
		bool jump = Input.GetButtonDown ("Jump");

		if(Move != null)
			Move (horizontal, jump, isGrounded);

		if (Input.GetButtonDown ("Moonlight") && OnMoonLight != null)
		{
			moonlightEnable = !moonlightEnable;
			OnMoonLight (moonlightEnable);
		}

		if (Input.GetButtonDown ("Action") && OnAction != null)
			OnAction ();
	}
}

