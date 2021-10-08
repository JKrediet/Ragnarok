using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
	public State curentState;
	public List<GameObject> players;
	public GameObject hitboxPos;
	public GameObject target;
	public AttackState[] attackStates;
	public AttackState[] rangedStates;
	public Animator anim;
	public Vector3 delayedPos;
	public float targetUpdateTime;
	public float targetDelay;
	public float triggerRange;
	public float attackRange;
	public float idleRange=2f;
	public float attackMovementSpeed;
	public float movementSpeed;
	public float AttrotationSpeed;
	public int currentAttack;
	public bool doAttack;
	public bool spawned;
	public bool isDead;
	[Header("ranged")]
	public bool hasRangedAtt;
	public bool trowCoolDown;
	private bool gettingTarget;
	private bool hitboxActive;
	private bool doingDamage;
	private Health hp;
	private Collider[] hitColliders;
	private float hitboxRadius;
	private void Start()
	{
		players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
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
		hitColliders[i].transform.GetComponent<PlayerHealth>().TakeDamage(attackStates[currentAttack].damage,false,0, hitColliders[i].transform.position);
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

		for (int i = 0; i < players.Count; i++)
		{
			if (players[i] == null)
			{
				players.Remove(players[i]);
			}
			else if (players[i].GetComponent<PlayerHealth>().health <= 0)
			{
				players.Remove(players[i]);
			}
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
	public void Spawned()
	{
		spawned = true;
	}
	public void CoolDownTrow(float timeToWait)
	{
		trowCoolDown = true;
		Invoke("TurnOn", timeToWait);
		print(timeToWait);
	}
	public void TurnOn()
	{
		trowCoolDown = false;
	}
}