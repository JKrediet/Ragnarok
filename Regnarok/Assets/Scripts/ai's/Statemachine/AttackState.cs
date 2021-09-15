using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackState : State
{
    public IdleState idle;
    public TriggerState trigger;
    [Space(5)]
    public StateManager sm;
    public NavMeshAgent agent;
    public int attackAmount;
	public float[] attackCoolDown;
	private bool isAttacking = false;
    public override State RunCurrentState()
    {
        float dist = Vector3.Distance(transform.position, sm.target.transform.position);
        if (dist <= sm.triggerRange)
        {
            if (dist <= sm.attackRange)
            {
                Vector3 lookat = new Vector3(sm.target.transform.position.x, 0, sm.target.transform.position.z);//player pos without height

                transform.LookAt(lookat);

                if (!isAttacking)
                {
                    Attack();
                }
            }
            else
            {
                if (!isAttacking)
                {
                    sm.doAttack = false;
                    return trigger;
                }
            }
        }
        else
        {
            return idle;
        }
        return this;
    }
    public void Attack()
	{
        isAttacking = true;
        sm.doAttack = true;
        int RandomInt = Random.Range(1, attackAmount+1);
        string attackString = "Attack" + RandomInt.ToString();
        sm.anim.SetBool(attackString,true);
        StartCoroutine(DoDamage(attackCoolDown[RandomInt]));
    }
    public IEnumerator DoDamage(float waittime)
	{
       yield return new WaitForSeconds(waittime);
        isAttacking = false;
        sm.doAttack = false;
    }
}