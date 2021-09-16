using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
	public State curentState;
	public GameObject[] players;
	public GameObject hitboxPos;
	public GameObject target;
	public AttackState[] attackStates;
	public Animator anim;
	public Vector3 delayedPos;
	public float targetUpdateTime;
	public float targetDelay;
	public float triggerRange;
	public float attackRange;
	public float idleRange=2f;
	public float attackMovementSpeed;
	public float movementSpeed;
	public int currentAttack;
	public bool doAttack;
	public bool spawned;
	public bool isDead;
	private bool gettingTarget;
	private bool hitboxActive;
	private bool doingDamage;
	private Health hp;
	private Collider[] hitColliders;
	private float hitboxRadius;
	private void Start()
	{
		players = GameObject.FindGameObjectsWithTag("Player");
		target = players[0];
	}
	private void Update()
	{
		if (isDead)
		{
			return;
		}
		if (hitboxActive)
		{
			hitColliders=Physics.OverlapSphere(hitboxPos.transform.position, hitboxRadius);
			for (int i = 0; i < hitColliders.Length; i++)
			{
				if (hitColliders[i].transform.tag == "Player")
				{
					if (!doingDamage)
					{
						StartCoroutine(DoDamge(i));
					}
				}
			}
		}
		RunStateMachine();
		if (!gettingTarget)
		{
			StartCoroutine(GetTarget());
		}
		new WaitForSeconds(targetDelay);
		delayedPos= target.transform.position;
	}
	public IEnumerator DoDamge(int i)
	{
		doingDamage = true;
		hitColliders[i].transform.GetComponent<PlayerHealth>().Health_Damage(attackStates[currentAttack].damage);
		yield return new WaitForSeconds(5);
		doingDamage = false;
	}
	private void RunStateMachine()
	{
		State nextState = curentState?.RunCurrentState();

		if(nextState != null)
		{
			SwitchOnNextState(nextState);
		}
	}
	private void SwitchOnNextState(State nextState)
	{
		curentState = nextState;
	}
	public IEnumerator GetTarget()
	{
		gettingTarget = true;

		for (int i = 0; i < players.Length; i++)
		{
			float dis = Vector3.Distance(transform.position, players[i].transform.position);
			float targetDis = Vector3.Distance(transform.position, target.transform.position);
			if (dis < targetDis)
			{
				if (target != players[i])
				{
					target = players[i];
				}
			}
		}
		yield return new WaitForSeconds(targetUpdateTime);
		gettingTarget = false;
	}
	public void ResetAnim()
	{
		for (int i = 0; i < attackStates.Length; i++)
		{
			anim.SetBool(attackStates[i].animationName, false);
		}
		anim.SetBool("IsWalking", false);		
	}
	public void SetHitBoxActive(float radius)
	{
		hitboxRadius = radius;
		hitboxActive = true;
	}
	public void SetHitBoxFalse()
	{
		hitboxActive = false;
		hitColliders = null;
	}
}