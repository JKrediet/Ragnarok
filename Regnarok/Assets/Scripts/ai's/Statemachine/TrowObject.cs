using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrowObject : MonoBehaviour
{
	public float throwPower;
	public float damage;
	public bool activated;
	private bool doingDamage;
	public void LookAtPlayer(Vector3 pos)
	{
		transform.LookAt(pos);
	}
	public void Update()
	{
		if (activated)
		{
			gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * throwPower + transform.up * 4, ForceMode.Impulse);
		}
	}
	private void OnCollisionEnter(Collision collision)
    {
		if (collision.transform.tag == "Player")
		{
			if (!doingDamage)
			{
				DoDamage(collision.transform);
			}
		}
		else
		{
			activated = false;
			GetComponent<Rigidbody>().isKinematic = true;
			GetComponent<Rigidbody>().useGravity = false;
			GetComponent<Rigidbody>().Sleep();
		}
    }
	public void DoDamage(Transform trans)
	{
		doingDamage = true;
		trans.GetComponent<PlayerHealth>().Health_Damage(damage, false);
	}
}