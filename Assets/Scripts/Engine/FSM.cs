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
	public delegate void FSMAction(PendingChange order);
	public event FSMAction FSMChange;

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
	public struct PendingChange
	{
		public Action action;
		public int stateID;

		public PendingChange(Action a, int ID = 0)
		{
			action = a;
			stateID = ID;	
		}
	};

	private Stack<FSMState> stateStack = new Stack<FSMState> ();
	private List<PendingChange> pendingList = new List<PendingChange> ();
	/**
	 * The hastable maps a state ID with an instance of a state,
	 * so a state is only instanciated at the beginning of the scene.
	 **/
	private Hashtable factories = new Hashtable ();


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
			if (state.UpdateState ())
				break;
		}
		ApplyPendingChanges ();
	}

	public void FixedUpdate()
	{
		foreach (FSMState state in stateStack)
			state.FixedUpdateState ();
	}
		
	public void PushState(int stateID)
	{
		AddChange(new PendingChange(Action.Push, stateID));
	}

	public void PopState()
	{
		if (IsEmpty())
			return;
		AddChange(new PendingChange(Action.Pop));
	}

	public void ClearStates()
	{
		AddChange(new PendingChange(Action.Clear));
	}

	public void AddChange(PendingChange change)
	{
		pendingList.Add (change);
		if(FSMChange != null)
			FSMChange (change);
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
				SafePush (change.stateID);
				break;
			case Action.Pop:
				SafePop ();
				break;
			case Action.Clear:
				SafeClear ();
				break;
			}
		}
		pendingList.Clear();
	}

	public Hashtable GetFactory()
	{
		return factories;
	}

	public Stack<FSMState> GetStack()
	{
		return stateStack;
	}

	public List<PendingChange> GetPendingList()
	{
		return pendingList;
	}

	/**
	 * The three following methods make sure that Exit() and Enter() methods are called
	 * when pushing or poping a state, this is super important to avoid "ghost listener"
	 * as unregistering is usually done in Exit().
	 **/
	private void SafeClear()
	{
		foreach(FSMState state in stateStack)
			state.Exit ();
		stateStack.Clear ();
	}

	private void SafePop()
	{
		FSMState popState = stateStack.Pop ();
		popState.Exit();
	}

	private void SafePush(int stateID)
	{
		FSMState pushState = FindState (stateID);
		pushState.Enter ();
		stateStack.Push (pushState);
	}

	// Use when the player goes in the past to override the present states
	public void CopyState(FSM fsm)
	{
		foreach (FSMState state in fsm.GetFactory().Values)
			FindState (state.GetID ()).Copy (state);

		SafeClear ();
		FSMState[] states = fsm.GetStack ().ToArray ();
		for (int i = states.Length - 1; i >= 0; i--)
			SafePush (states [i].GetID ());

		pendingList.Clear ();
		foreach (PendingChange change in fsm.GetPendingList())
			pendingList.Add (change);
	}
}

