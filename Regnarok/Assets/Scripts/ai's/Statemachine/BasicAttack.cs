using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class BasicAttack : AttackState
{
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
                    if (Vector3.Dot(forward, toOther) >0)
                    {
                        int randomI = Random.Range(0, sm.attackStates.Length);
                        DoAttack();
                        return this;
                    }
                }
            }
            else
            {
                if (!sm.doAttack)
                {
                    if (sm.spawned)
                    {
                        if (!sm.anim.GetCurrentAnimatorStateInfo(0).IsName(animationName))
                        {
                            agent.speed = sm.movementSpeed;
                            sm.idleRange = 1000f;
                            return trigger;                   
                        }
                    }
                }
            }
        }
        else
        {
            if (!sm.anim.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            {
                agent.speed = sm.movementSpeed;
                sm.idleRange = 1000f;
                return idle;
            }
        }
        return this;
    }
    public void DoAttack()
	{
        sm.ResetAnim();
        sm.anim.SetBool(animationName, true);
        agent.speed=sm.attackMovementSpeed;
    }
}