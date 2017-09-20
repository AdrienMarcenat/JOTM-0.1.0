using UnityEngine;
using System.Collections;

public class PlayerInputManager : MonoBehaviour
{
	public delegate void MoonlightAction(bool enable);
	public static event MoonlightAction OnMoonLight;

	public delegate void PressButtonAction();
	public event PressButtonAction OnAction;

	public delegate void MoveAction(float x, bool jump, bool isGrounded, Vector2 platformVelocity);
	public event MoveAction Move;

	[SerializeField] private float raycastFeetDistance = 0.25f;

	private LayerMask whatIsGround;
	private Vector2 raycastFeetDirection = new Vector2 (0, -1);
	private Vector2 platformVelocity = Vector2.zero;
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
		whatIsGround = Physics2D.GetLayerCollisionMask (groundCheck.gameObject.layer);

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject && colliders[i].gameObject.layer != LayerMask.NameToLayer("Light"))
				isGrounded = true;
			if (colliders [i].tag == "MovingPlatform")
				platformVelocity = colliders [i].GetComponent<Rigidbody2D> ().velocity;
		}
	}

	protected void Update () 
	{
		if (GameManager.pause)
			return;

		float horizontal = Input.GetAxisRaw ("Horizontal");
		bool jump = Input.GetButtonDown ("Jump");

		if(Move != null)
			Move (horizontal, jump, isGrounded, platformVelocity);

		if (Input.GetButtonDown ("Moonlight") && OnMoonLight != null)
		{
			moonlightEnable = !moonlightEnable;
			OnMoonLight (moonlightEnable);
		}

		if (Input.GetButtonDown ("Action") && OnAction != null)
			OnAction ();
	}
}

