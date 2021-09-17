﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IdleState : State
{
    public TriggerState trigger;
    public AttackingState attack;
    public StateManager sm;
    [Space(5)]
    public NavMeshAgent agent;
    public float behindEnemieDivtation = 3;
    public float idleWalkTime=7.5f;
    public LayerMask grasslayer;
    private bool isIdleMoving;
    private bool isWalking;
    private Vector3 idleDes;

    public override State RunCurrentState()
	{
        if (sm.isDead)
        {
            return this;
        }
        float dist = Vector3.Distance(transform.position, sm.target.transform.position);
        float disIdlePos = Vector3.Distance(transform.position, idleDes);
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 toOther = sm.target.transform.position - transform.position;
        if (dist <= sm.triggerRange&& Vector3.Dot(forward, toOther) > 0
            || dist <= sm.triggerRange/behindEnemieDivtation && Vector3.Dot(forward, toOther) < 0)
        {
            if (dist <= sm.attackRange)
            {
                if (sm.spawned)
                {
                    int randomI = Random.Range(0, sm.attackStates.Length);
                    sm.currentAttack = randomI;
                    return sm.attackStates[randomI];
                }
            }
            else
            {         
                return trigger;
            }
        }
        if(disIdlePos<= sm.idleRange)
		{
            isWalking=false;
            sm.idleRange = 2f;
        }
		else
		{
            isWalking = true;
        }
		if(isWalking)
		{
            sm.anim.SetBool("IsWalking", true);
			if (sm.spawned)
			{
                agent.destination = idleDes;
            }
        }
		if (sm.spawned&&!isWalking)
		{
            agent.destination = transform.position;
            sm.ResetAnim();
        }
		if (!isIdleMoving&& !isWalking)
		{
            StartCoroutine(RandomIdlePos());
        }
        return this;
	}
    public IEnumerator RandomIdlePos()
    {
        if (!isIdleMoving)
        {
            idleDes = transform.position + new Vector3(Random.Range(-10, 10), 100, Random.Range(-10, 10));
            Ray ray = new Ray(idleDes, -transform.up);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo,1000,grasslayer))
            {
                if (hitInfo.transform.tag == "Mesh")
                {
                    idleDes = hitInfo.point;
                }
                else
                {
                    Vector3 rotation = transform.eulerAngles;
                    transform.eulerAngles = rotation+new Vector3(0, 15, 0);
                    agent.destination = transform.position;
                    sm.ResetAnim();
                    isWalking = false;
                }
            }
        }
        isIdleMoving = true;
        isWalking = true;
        yield return new WaitForSeconds(idleWalkTime);
        isIdleMoving = false;
    }
}