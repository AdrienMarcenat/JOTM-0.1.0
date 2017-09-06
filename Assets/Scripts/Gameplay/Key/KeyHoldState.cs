using UnityEngine;
using System.Collections;

public class KeyHoldState : FSMState
{
	private Transform holderTransform;

	protected override void Awake()
	{
		ID = (int)KeyStates.ID.Hold;
		base.Awake ();

		holderTransform = GameObject.FindGameObjectWithTag ("Player").transform;
	}

	public override bool UpdateState ()
	{
		transform.position = new Vector3(holderTransform.position.x, holderTransform.position.y, transform.position.z);
		if (Input.GetButtonDown ("Action"))
			Unhold ();
		
		return false;
	}

	private void Unhold()
	{
		requestStackPop ();
		requestStackPush ((int) KeyStates.ID.Unhold);
	}
}

