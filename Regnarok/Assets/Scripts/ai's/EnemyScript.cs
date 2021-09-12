using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyScript : MonoBehaviour
{
    public GameObject[] players;
    public Animator anim;
    public float jumpSpeed;
    public float health;
    public float triggerRange;
    public float attackRange;
    public float targetUpdateTime = 0.5f;
    [Header("Inumerator times")]
    public float deathAnimDuration;
    public float attackCoolDown;
    private bool doDamage;
    NavMeshAgent agent;
    private GameScaler gs;
    private Rigidbody rb;
    private GameObject target;
    private bool gettingTarget;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //gs = FindObjectOfType<GameScaler>();
        agent = GetComponent<NavMeshAgent>();
        players = GameObject.FindGameObjectsWithTag("Player");
        target = players[0];
    }
    void Update()
    {
		if (!agent.enabled)
		{
            return;
		}
		if (!gettingTarget)
		{
            StartCoroutine("GetTarget");
		} 
        float dist = Vector3.Distance(transform.position, target.transform.position);
		if (dist <= triggerRange)
        {
			if (dist <= attackRange)
			{
                Vector3 lookat = new Vector3(target.transform.position.x, 0, target.transform.position.z);//player pos without height

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
                    agent.destination = target.transform.position;
                    ResetAnim();
                    anim.SetBool("IsWalking",true);
                }
			}
		}
		if (health <= 0)
		{
            StartCoroutine("Death");
        }
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
    public void SpawnForce()
	{
        rb.AddForce(Vector3.up * jumpSpeed*3);
    }
    public void StopSpawnForce()
    {
        anim.applyRootMotion = false;
        agent.enabled = true;
        Invoke("TurnOffGravity", 0.5f);
    }
    public void TurnOffGravity()
	{
        rb.useGravity = false;
        rb.isKinematic = true;
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
    public void ResetAnim()
	{
        anim.SetBool("IsWalking", false);
	}
    public void GiveReward()
	{

	}
}