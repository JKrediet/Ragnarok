using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SnakeBehavour : MonoBehaviour
{
    public List<GameObject> targets;
    public float attackTime;
	public float targetCooldown;
	public ParticleSystem ps;
	private bool isGettingTarget;
	private bool isAttacking;
	private bool cooldownIsOn;
	private GameObject target;
	private Animator anim;
	private NavMeshAgent agent;
	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
		ps.Play();
	}
	void Update()
	{
		if (!isGettingTarget)
		{
			StartCoroutine(GetTarget());
		}
		if (!cooldownIsOn)
		{
			if (target != null)
			{
				if (!isAttacking)
				{
					StartCoroutine(Attack());
				}
			}
			agent.destination = target.transform.position;
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Enemy"))
		{
            targets.Add(other.gameObject);
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Enemy"))
		{
			targets.Remove(other.gameObject);
		}
	}
	public IEnumerator GetTarget()
	{
		isGettingTarget = true;
		cooldownIsOn = true;
		Invoke("SetFalse", targetCooldown);
		for (int i = 0; i < targets.Count; i++)
		{
			if (targets[i] != null)
			{
				float dis = Vector3.Distance(targets[i].transform.position, transform.position);
				if (dis < Vector3.Distance(target.transform.position, transform.position))
				{
					target = targets[i];
				}
				else if (target == null)
				{
					target = targets[i];
				}
			}
			else
			{
				targets.Remove(targets[i]);
			}
		}
		yield return new WaitForSeconds(targetCooldown/2);
		isGettingTarget = false;
	}
	public void SetFalse()
	{
		cooldownIsOn = false;
	}
	public IEnumerator Attack()
	{
		isAttacking = true;
		yield return new WaitForSeconds(attackTime);
		isAttacking = false;
	}
}