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
    public float behindEnemieDivtation = 3;
    public float idleRange=2f;
    public float idleWalkTime=7.5f;
    public LayerMask grasslayer;
    private bool isIdleMoving;
    private bool isWalking;
    private Vector3 idleDes;

    public override State RunCurrentState()
	{
        float dist = Vector3.Distance(transform.position, sm.target.transform.position);
        float disIdlePos = Vector3.Distance(transform.position, idleDes);
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 toOther = sm.target.transform.position - transform.position;

            if (dist <= sm.triggerRange&& Vector3.Dot(forward, toOther) > 0
                || dist <= sm.triggerRange/behindEnemieDivtation && Vector3.Dot(forward, toOther) < 0)
            {
            if (dist <= sm.attackRange)
            {
                transform.LookAt(new Vector3(agent.destination.z, 0, agent.destination.z));
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