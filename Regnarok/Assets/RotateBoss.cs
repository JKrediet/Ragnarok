using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBoss : MonoBehaviour
{
	public GameObject objectTorated;
	public void RotateObj(float amount)
	{
		objectTorated.transform.rotation = Quaternion.Euler(objectTorated.transform.eulerAngles.x, amount, objectTorated.transform.eulerAngles.z);
	}
}
