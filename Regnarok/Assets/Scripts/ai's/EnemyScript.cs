using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyScript : MonoBehaviour
{
    public GameObject player;
    public Animator anim;
    public float health;
    public float triggerRange;
    public float attackRange;
    [Header("Inumerator times")]
    public float deathAnimDuration;
    public float attackCoolDown;
    private bool doDamage;
    NavMeshAgent agent;
    private GameScaler gs;
    void Start()
    {
        gs = FindObjectOfType<GameScaler>();
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);
		if (dist <= triggerRange)
        {
			if (dist <= attackRange)
			{
                Vector3 lookat = new Vector3(player.transform.position.x, 0, player.transform.position.z);//player pos without height

                transform.LookAt(lookat);

				if (!doDamage)
				{
                    StartCoroutine("DoDamage");
                }
            }
			else
			{
				if (!doDamage)
				{
                    agent.destination = player.transform.position;
				}
			}
		}

		if (health <= 0)
		{
            StartCoroutine("Death");
        }
    }
    public IEnumerator DoDamage()
	{
        doDamage = true;
        GiveReward();
        yield return new WaitForSeconds(attackCoolDown);
        doDamage = false;
    }
    public IEnumerator Death()
	{
        anim.SetBool("isDying", true);
        yield return new WaitForSeconds(deathAnimDuration);
        Destroy(gameObject);
	}
    public void GiveReward()
	{

	}
}