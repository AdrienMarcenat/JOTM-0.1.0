using UnityEngine;
using System.Collections;

public static class ExtensionMethods
{
	public static void ResetTransformation(this Transform trans)
	{
		trans.position = Vector3.zero;
		trans.localRotation = Quaternion.identity;
		trans.localScale = new Vector3(1, 1, 1);
	}

	public static void SetX(this Transform trans, float x)
	{
		trans.position.SetX (x);
	}

	public static void SetY(this Transform trans, float y)
	{
		trans.position.SetY (y);
	}

	public static void SetZ(this Transform trans, float z)
	{
		trans.position.SetZ (z);
	}

	public static void SetX(this Vector3 vector, float x)
	{
		vector = new Vector3 (x, vector.y, vector.z);
	}

	public static void SetY(this Vector3 vector, float y)
	{
		vector = new Vector3 (vector.x, y, vector.z);
	}

	public static void SetZ(this Vector3 vector, float z)
	{
		vector = new Vector3 (vector.x, vector.y, z);
	}

	public static bool CompareTo(this Vector3 vector, Vector3 other, float precision)
	{
		return (vector.x - other.x) < precision && (vector.y - other.y) < precision && (vector.z - other.z) < precision;
	}
}