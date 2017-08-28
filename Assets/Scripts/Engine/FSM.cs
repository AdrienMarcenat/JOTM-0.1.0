using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * A FSM is a component, any FSMState attached to the same 
 * gameobject will register himself to the FSM, and can send
 * orders to it. The FSM manages the states with a stack.
 **/
public class FSM : MonoBehaviour
{
	// The states will send orders to the stack
	public enum Action
	{
		Push,
		Pop,
		Clear,
	};

	/**
	 * An order has the following structure.
	 * Orders are stored in a list and applied
	 * when each states have been updated.
	 **/
	private struct PendingChange
	{
		public Action action;
		public int stateID;

		public PendingChange(Action a, int ID = 0)
		{
			action = a;
			stateID = ID;
		}
	};

	private Stack<FSMState> stateStack;
	private List<PendingChange> pendingList;
	/**
	 * The hastable maps a state ID with an instance of a state,
	 * so a state is only instanciated at the beginning of the scene.
	 **/
	private Hashtable factories;


	protected virtual void Awake()
	{
		stateStack = new Stack<FSMState> ();
		pendingList = new List<PendingChange> ();
		factories = new Hashtable ();
	}

	public void RegisterState(int stateID, FSMState state)
	{
		factories.Add(stateID, state);
	}

	// If a state return true, the lower states will not be updated.
	public void Update()
	{
		foreach(FSMState state in stateStack)
		{
			if (state.Update ())
				break;
		}
		ApplyPendingChanges ();
	}

	public void PushState(int stateID)
	{
		pendingList.Add(new PendingChange(Action.Push, stateID));
	}

	public void PopState()
	{
		if (IsEmpty())
			return;
		pendingList.Add(new PendingChange(Action.Pop));
	}

	public void ClearStates()
	{
		pendingList.Add(new PendingChange(Action.Clear));
	}

	public bool IsEmpty()
	{
		return stateStack.Count == 0;
	}

	// Used when a Push order is sent to the stack to find the corresponding state.
	private FSMState FindState(int stateID)
	{
		return (FSMState) factories [stateID];
	}

	private void ApplyPendingChanges()
	{
		foreach (PendingChange change in pendingList)
		{
			switch (change.action)
			{
			case Action.Push:
				FSMState pushState = FindState (change.stateID);
				pushState.Enter ();
				stateStack.Push(pushState);
				break;
			case Action.Pop:
				FSMState popState = stateStack.Pop ();
				popState.Exit();
				break;
			case Action.Clear:
				stateStack.Clear ();
				break;
			}
		}
		pendingList.Clear();
	}
}

