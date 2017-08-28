﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonLightRayCaster : MonoBehaviour
{
	[SerializeField] private float maxDistance;
	[SerializeField] private Vector2 lightDirection;
	[SerializeField] private ContactFilter2D contactFilter;

	private bool isOn = false;

	private void ShootLight()
	{
		RaycastHit2D[] results = new RaycastHit2D[5];
		int hitNumber = Physics2D.Raycast (transform.position, lightDirection, contactFilter, results, maxDistance);
		Debug.DrawRay (transform.position, 10*lightDirection, Color.red, 1.0f);

		for (int i = 0; i < hitNumber; i++) 
		{
			RaycastHit2D raycast = results [i];
			raycast.collider.gameObject.SendMessage ("OnMoonLight");
		}
	}

	void Update () 
	{
		if (isOn)
			ShootLight ();
	}

	public void Enable()
	{
		this.isOn = !isOn;
	}
}
