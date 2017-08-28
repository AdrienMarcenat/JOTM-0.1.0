using UnityEngine;
using System.Collections;

namespace PlayerStates
{
	public enum ID
	{
		Normal = 0,
		GameOver = 1,
	}
}

public class PlayerFSM : FSM
{
	protected override void Awake()
	{
		base.Awake ();
		PushState ((int) PlayerStates.ID.Normal);
	}
}

