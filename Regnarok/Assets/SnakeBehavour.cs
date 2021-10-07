using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBehavour : MonoBehaviour
{
    public List<GameObject> targets;
    public float attackTime;
	public float targetCooldown;
	public ParticleSystem ps;
	private bool isGettingTarget;
	private bool isAttacking;
	private GameObject target;
	private Animator anim;
	private void Start()
	{
		anim = GetComponent<Animator>();
		ps.Play();
	}
	//void Update()
 //   {
	//	if (!isGettingTarget)
	//	{
	//		StartCoroutine(GetTarget());
	//	}
	//	if (!isAttacking)
	//	{
	//		if (target != null)
	//		{
	//			StartCoroutine(Attack());
	//		}
	//	}
 //   }
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
		yield return new WaitForSeconds(targetCooldown);
		isGettingTarget = false;
	}
	public IEnumerator Attack()
	{
		isAttacking = true;
		yield return new WaitForSeconds(attackTime);
		isAttacking = false;
	}
}