using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T instance;

	protected void Awake () 
	{
		if (instance == null)
			instance = this.gameObject.GetComponent<T>();
		else if (instance != this.gameObject.GetComponent<T>())
			Destroy (this.gameObject);

		DontDestroyOnLoad (gameObject);
	}
}

