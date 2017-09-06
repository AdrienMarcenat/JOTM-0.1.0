using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour
{
	public delegate void SwitchAction();
	public event SwitchAction SwitchEvent;

	private bool isPlayerInTrigger = false;

	void Update ()
	{
		if (isPlayerInTrigger && Input.GetButtonDown ("Action") && SwitchEvent != null)
			SwitchEvent ();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
			isPlayerInTrigger = true;
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Player")
			isPlayerInTrigger = false;
	}
}

