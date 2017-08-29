using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour
{
	public delegate void SwitchAction();
	public event SwitchAction SwitchEvent;

	private bool isPlayerIn = false;

	void Update ()
	{
		if (isPlayerIn && Input.GetButtonDown ("Action") && SwitchEvent != null)
			SwitchEvent ();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
			isPlayerIn = true;
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Player")
			isPlayerIn = false;
	}
}

