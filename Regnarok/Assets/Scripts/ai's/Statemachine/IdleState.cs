using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IdleState : State
{
    public TriggerState trigger;
    public AttackState attack;
    public StateManager sm;
    [Space(5)]
    public NavMeshAgent agent;
    public float idleRange=2f;
    public float idleWalkTime=7.5f;
    private bool isIdleMoving;
    private bool isWalking;
    private Vector3 idleDes;

    public override State RunCurrentState()
	{
        float dist = Vector3.Distance(transform.position, sm.target.transform.position);
        float disIdlePos = Vector3.Distance(transform.position, idleDes);
        if (dist <= sm.triggerRange)
        {
            if (dist <= sm.attackRange)
            {
                Vector3 lookat = agent.destination;
                lookat.y = 0;
                transform.LookAt(lookat);
                return attack;
            }
            else
            {             
                return trigger;
            }
        }
        if(disIdlePos<= idleRange)
		{
            isWalking=false;
        }
		else
		{
            isWalking = true;
        }
		if(isWalking)
		{
            sm.anim.SetBool("IsWalking", true);
            agent.destination = idleDes;
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
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.transform.tag == "Mesh")
                {
                    idleDes = hitInfo.point;
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);
                    CallAgain();
                }
            }
        }
        isIdleMoving = true;
        isWalking = true;
        yield return new WaitForSeconds(idleWalkTime);
        isIdleMoving = false;
    }
    public void CallAgain()
    {
        StartCoroutine("RandomIdlePos");
    }
}