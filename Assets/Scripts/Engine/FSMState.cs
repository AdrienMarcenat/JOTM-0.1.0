using UnityEngine;
using System.Collections;

public abstract class FSMState : MonoBehaviour
{
	protected FSM fsm;
	protected int ID;

	protected virtual void Awake()
	{
		fsm = GetComponent<FSM>();
	}

	protected virtual void Start()
	{
		fsm.RegisterState (ID, this);
	}

	public virtual void Enter () {}
	public virtual bool Update () { return false; }
	public virtual void Exit () {}

	protected void requestStackPush(int stateID)
	{
		fsm.PushState (stateID);
	}

	protected void requestStackPop()
	{
		fsm.PopState ();
	}

	protected void requestStateClear()
	{
		fsm.ClearStates ();
	}
}

