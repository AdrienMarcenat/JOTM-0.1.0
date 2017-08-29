using UnityEngine;
using System.Collections;

namespace SwitchState
{
	public enum ID
	{
		Off = 0,
		On = 1,
	}
}

public class SwitchFSM : FSM
{
	protected override void Awake()
	{
		base.Awake ();
		PushState ((int) SwitchState.ID.Off);
	}
}

