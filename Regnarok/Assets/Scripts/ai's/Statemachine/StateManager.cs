using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
	public State curentState;
	public GameObject[] players;
	public GameObject target;
	public Animator anim;
	public Vector3 delayedPos;
	public float targetUpdateTime;
	public float targetDelay;
	public float triggerRange;
	public float attackRange;
	public bool doAttack;
	public bool spawned;
	private bool gettingTarget;
	private void Start()
	{
		players = GameObject.FindGameObjectsWithTag("Player");
		target = players[0];
	}
	private void Update()
	{
		RunStateMachine();
		if (!gettingTarget)
		{
			StartCoroutine(GetTarget());
		}
		new WaitForSeconds(targetDelay);
		delayedPos= target.transform.position;
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
		anim.SetBool("IsWalking", false);
		anim.SetBool("IsDying", false);
		anim.SetBool("Attack1", false);
		anim.SetBool("Attack2", false);
		anim.SetBool("Attack3", false);
	}
}