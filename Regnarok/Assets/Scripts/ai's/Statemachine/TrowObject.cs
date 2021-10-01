using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrowObject : MonoBehaviour
{
	public float throwPower;
	public float damage;
	public bool activated;
	public GameObject target;
	private bool doingDamage;
	private bool goToPlayer;
	private Vector3 pos;
	public void LookAtPlayer()
	{
		transform.LookAt(pos);
	}
	public IEnumerator GotActived()
	{
		pos= target.transform.position+new Vector3(0,0,0);
		yield return new WaitForSeconds(1f);
		goToPlayer = true;
	}
	public void Update()
	{
		if (activated)
		{
			gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * throwPower + transform.up * 4, ForceMode.Impulse);
			if (goToPlayer)
			{
				pos = target.transform.position;
			}
			LookAtPlayer();
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
		trans.GetComponent<PlayerHealth>().Health_Damage(damage, false, trans.transform.position);
	}
}