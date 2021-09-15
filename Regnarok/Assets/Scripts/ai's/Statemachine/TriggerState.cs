using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TriggerState : State
{
    public IdleState idle;
    public AttackState attack;
    [Space(5)]
    public StateManager sm;
    public NavMeshAgent agent;
    private bool gettingTarget;
    private bool doAttack;

    public override State RunCurrentState()
	{
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
                    sm.doAttack = true;
                    return attack;
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
            return idle;
		}
        sm.ResetAnim();
        sm.anim.SetBool("IsWalking", true);
        return this;
	}
}