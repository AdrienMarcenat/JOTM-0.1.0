using UnityEngine;
using System.Collections;

/**
 * SoundManager allow to play sound effect and music,
 * using two AudioSources (should be child of the 
 * SoundManager's gameobject).
 **/
public class SoundManager : Singleton<SoundManager>
{
	[SerializeField] AudioSource efxSource;              
	[SerializeField] AudioSource musicSource;

	// Play only one sound effect at a time
	static public void PlaySingle(AudioClip clip)
	{
		instance.efxSource.clip = clip;
		instance.efxSource.Play ();
	}

	// Allow to play multiple sound effect at the same time
	static public void PlayMultiple(AudioClip clip)
	{
		instance.efxSource.PlayOneShot(clip);
	}

	static public void PlayMusic(AudioClip clip)
	{
		if (instance.musicSource.clip != clip) 
		{
			instance.musicSource.clip = clip;
			instance.musicSource.Play ();
			instance.musicSource.loop = true;
		}
	}

	static public void PauseMusic(bool pause)
	{
		if(pause)
			instance.musicSource.Pause ();
		else
			instance.musicSource.Play ();
	}

	static public void StopMusic(bool pause)
	{
		instance.musicSource.Stop ();
	}
}