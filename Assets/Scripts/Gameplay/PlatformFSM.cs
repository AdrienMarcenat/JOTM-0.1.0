using UnityEngine;
using System.Collections;

namespace PlatformStates
{
	public enum ID
	{
		Idle = 0,
		Moving = 1,
	}
}

public class PlatformFSM : FSM
{
	protected override void Awake()
	{
		base.Awake ();
		PushState ((int) PlatformStates.ID.Idle);
	}
}

