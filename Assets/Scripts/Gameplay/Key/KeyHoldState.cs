using UnityEngine;
using System.Collections;

public class KeyHoldState : FSMState
{
	private Transform holderTransform;
	private Rigidbody2D body;
	private Rigidbody2D playerBody;
	[SerializeField] DistanceJoint2D chain;

	protected override void Awake()
	{
		ID = (int)KeyStates.ID.Hold;
		base.Awake ();

		holderTransform = GameObject.FindGameObjectWithTag ("Player").transform;
		playerBody = GameObject.FindGameObjectWithTag ("Player").GetComponent<Rigidbody2D> ();
		body = GetComponent<Rigidbody2D> ();
	}

	public override void Enter ()
	{
		transform.parent = holderTransform;
		body.isKinematic = true;
		if (chain != null)
			chain.connectedBody = playerBody;
	}
		
	public override bool UpdateState ()
	{
		if (Input.GetButtonDown ("Action"))
			Unhold ();
		
		return false;
	}

	public override void Exit ()
	{
		transform.parent = null;
		body.isKinematic = false;
		if (chain != null)
			chain.connectedBody = body;
	}
		
	private void Unhold()
	{
		requestStackPop ();
		requestStackPush ((int) KeyStates.ID.Unhold);
	}
}

