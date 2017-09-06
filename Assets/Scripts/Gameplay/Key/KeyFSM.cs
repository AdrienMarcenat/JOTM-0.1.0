using UnityEngine;
using System.Collections;

namespace KeyStates
{
	public enum ID
	{
		Unhold = 0,
		Hold = 1,
	}
}

public class KeyFSM : FSM
{
	protected override void Awake()
	{
		base.Awake ();
		PushState ((int) KeyStates.ID.Unhold);
	}
}

