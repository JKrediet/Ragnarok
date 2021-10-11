using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class SnakeBehavour : MonoBehaviour
{
    public List<GameObject> targets;
    public float attackTime;
	public float attackCooldown;
	public float targetCooldown;
	public ParticleSystem ps;
	public GameObject player;
	private bool isGettingTarget;
	private bool isAttacking;
	private bool cooldownIsOn;
	private bool hasAttackCooldown;
	public Animator anim;
	private NavMeshAgent agent;
	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		Invoke("KillMe",300f);
	}
	void Update()
	{
		if (!cooldownIsOn)
		{
			if (targets.Count != 0)
			{
				if (!isAttacking && !hasAttackCooldown)
				{
					StartCoroutine(Attack());
					transform.LookAt(targets[0].transform.position);
				}
				float dis = Vector3.Distance(transform.position, player.transform.position);
				if (dis > 4)
				{
					agent.destination = targets[0].transform.position;
				}
				else
				{
					agent.destination = transform.position;
				}
			}
			else
			{
				float dis = Vector3.Distance(transform.position, player.transform.position);
				if (dis > 4)
				{
					agent.destination = player.transform.position;
				}
				else
				{
					agent.destination = transform.position;
				}
			}
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.GetComponent<EnemieHealth>())
		{
            targets.Add(other.gameObject);
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.transform.GetComponent<EnemieHealth>())
		{
			targets.Remove(other.gameObject);
		}
	}
	public void SetFalse()
	{
		cooldownIsOn = false;
	}
	public IEnumerator Attack()
	{
		isAttacking = true;
		ps.Play();
		anim.SetBool("IsAttacking", true);
		yield return new WaitForSeconds(attackTime);
		ps.Stop();
		anim.SetBool("IsAttacking", false);
		isAttacking = false;
		StartCoroutine(AttackCooldown());
	}
	public IEnumerator AttackCooldown()
	{
		hasAttackCooldown = true;
		yield return new WaitForSeconds(attackCooldown);
		hasAttackCooldown = false;
	}
	public void KillMe()
	{
		PhotonNetwork.Destroy(gameObject);
	}
}