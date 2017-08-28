﻿using UnityEngine;
using System.Collections;

/**
 * A state is simply a component the issue orders to the FSM,
 * thus his purpose is to manipulate other components on the
 * gameobject change his behaviour. Three method are provided :
 * 	- Enter() should be used to activate/deactivate component
 * 	- Update() should be used when constant logic are required
 * 	- Exit() should be used to perform some cleaning before leaving
 **/
public abstract class FSMState : MonoBehaviour
{
	protected FSM fsm;
	protected int ID;

	protected virtual void Awake()
	{
		fsm = GetComponent<FSM>();
	}

	// Registration shouldn't be done before start since the FSM may be not initialized. 
	protected virtual void Start()
	{
		fsm.RegisterState (ID, this);
	}

	// Enter is called when the state is pushed.
	public virtual void Enter () {}

	// Update is called when the state is in the stack and no higher state return true.
	public virtual bool Update () { return false; }

	// Exit is called when the state is popped
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

