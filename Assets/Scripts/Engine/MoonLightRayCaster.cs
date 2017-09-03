using System.Collections;
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
		Debug.DrawRay (transform.position, maxDistance*lightDirection, Color.red, 0.01f);

		if(hitNumber > 0)
			for (int i = 0; i < 1; i++) 
			{
				RaycastHit2D raycast = results [i];
				raycast.collider.gameObject.SendMessage ("OnMoonlight");
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
