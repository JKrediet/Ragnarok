using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TriggerState : State
{
    public IdleState idle;
    [Space(5)]
    public StateManager sm;
    public NavMeshAgent agent;

    public override State RunCurrentState()
	{
        if (sm.isDead)
        {
            return this;
        }
        float dist = Vector3.Distance(transform.position, sm.target.transform.position);
        if (dist <= sm.triggerRange)
        {
            if (dist <= sm.attackRange)
            {
                Vector3 lookat = sm.delayedPos;
                lookat.y = 0;

                transform.LookAt(lookat);

                if (!sm.doAttack)
                {

                    Vector3 forward = transform.TransformDirection(Vector3.forward);
                    Vector3 toOther = sm.target.transform.position - transform.position;
                    if (Vector3.Dot(forward, toOther) > 0)
					{
						int randomI = Random.Range(0, sm.attackStates.Length);
                        sm.currentAttack = randomI;
                        sm.ResetAnim();
                        return sm.attackStates[randomI];
					}
				}
            }
            else if(sm.hasRangedAtt&&!sm.trowCoolDown)
			{
                Vector3 lookat = sm.delayedPos;
                lookat.y = 0;

                transform.LookAt(lookat);

                if (!sm.doAttack)
                {
                    Vector3 forward = transform.TransformDirection(Vector3.forward);
                    Vector3 toOther = sm.target.transform.position - transform.position;
                    if (Vector3.Dot(forward, toOther) > 0)
                    {
                        int randomI = Random.Range(0, sm.rangedStates.Length);
                        sm.currentAttack = randomI;
                        sm.ResetAnim();
                        return sm.rangedStates[randomI];
                    }
                }
            }
            else
            {
                if (!sm.doAttack)
                {
					if (sm.spawned)
					{
                        agent.destination = sm.delayedPos;
                    }
                }
            }
        }
		else
		{
            sm.idleRange = 10000f;
            return idle;
		}
        sm.ResetAnim();
        sm.anim.SetBool("IsWalking", true);
        return this;
	}
    public bool Chances()
	{
		if(Random.Range(0, 100)<45)
		{
            return true;
		}
		else
		{
            return false;
		}
	}
}