using UnityEngine;
using System.Collections;

/**
 * Any monobehaviour inheriting this class will become a singleton
 * and will persist between scenes, this is useful for some "god"
 * objects like SoundManager or GameManager, but should be avoided
 * in most cases.
 **/
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

