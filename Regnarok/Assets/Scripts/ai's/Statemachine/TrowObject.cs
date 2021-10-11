using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TrowObject : MonoBehaviour
{
	public float throwPower;
	public float damage;
	public bool activated;
	public GameObject target;
	public GameObject curvepos;
	public GameObject hand;
	private float returnTime;
	private bool doingDamage;
	private bool goToPlayer;
	private Vector3 pos;
	public void LookAtPlayer()
	{
		transform.LookAt(pos);
	}
	public IEnumerator GotActived()
	{
		if (PhotonNetwork.IsMasterClient)
		{ 
			pos = target.transform.position + new Vector3(0, 0.1f, 0);
			yield return new WaitForSeconds(1f);
			goToPlayer = true;
			GetComponent<MeshRenderer>().enabled = true;
			Invoke("DestroyMe", 5);
		}
	}
	public void Update()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			//GetComponent<MeshRenderer>().enabled = true;
			if (activated)
			{
				if (goToPlayer)
				{
					if (returnTime < 1)
					{
						transform.position = GetQuadraticCurvePoint(returnTime, hand.transform.position, curvepos.transform.position, pos);
					}
					returnTime += Time.deltaTime * 1.5f;
					LookAtPlayer();
				}
			}
		}
	}
	private void OnCollisionEnter(Collision collision)
    {
		if (collision.transform.tag == "Player")
		{
			if (!doingDamage)
			{
				DoDamage(collision.transform);
				Invoke("DestroyMe", 2.5f);
			}
		}
		else
		{
			activated = false;
			GetComponent<Rigidbody>().isKinematic = true;
			GetComponent<Rigidbody>().useGravity = false;
			GetComponent<Rigidbody>().Sleep();
			GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			Invoke("DestroyMe", 2.5f);
		}
    }
	public void DoDamage(Transform trans)
	{
		doingDamage = true;
		trans.GetComponent<PlayerHealth>().TakeDamage(damage, false,0,0,0, trans.transform.position);
	}
	public Vector3 GetQuadraticCurvePoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
	{
		float u = 1 - t;
		float tt = t * t;
		float uu = u * u;
		return (uu * p0) + (2 * u * t * p1) + (tt * p2);
	}
	public void DestroyMe()
	{
		GetComponent<PhotonView>().RPC("SyncedDestoy", RpcTarget.MasterClient);
	}
	public void SyncedDestoy()
	{
		PhotonNetwork.Destroy(gameObject);
	}
}