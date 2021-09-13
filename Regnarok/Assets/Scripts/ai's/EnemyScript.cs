using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyScript : MonoBehaviour
{
    public GameObject[] players;
    public Animator anim;
    public float targetDelay=0.3f;
    public float damage;
    public float jumpSpeed;
    public float health;
    public float triggerRange;
    public float attackRange;
    public float targetUpdateTime = 0.5f;
    [Header("Inumerator times")]
    public float deathAnimDuration;
    public float attackCoolDown;
    NavMeshAgent agent;
    private GameScaler gs;
    private Rigidbody rb;
    private GameObject target;
    private bool doAttack;
    private bool gettingTarget;
    private bool isSpawning;
    private bool isIdleWalking;
    private Vector3 idleDes;
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
        if (isSpawning)
        {
            return;
        }
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

                if (!doAttack)
                {
                    StartCoroutine("DoAttack");
                }
            }
            else
            {
                if (!doAttack)
                {
                    Vector3 delayedPos = target.transform.position;
                     new WaitForSeconds(targetDelay);
                    agent.destination = delayedPos;
                    ResetAnim();
                    anim.SetBool("IsWalking", true);
                }
            }
        }
        else
        {
            float dis = Vector3.Distance(transform.position, agent.destination);
            if (dis < 5)
            {
                isIdleWalking = false;
            }
            else
            {
                isIdleWalking = true;
            }

            if (isIdleWalking)
            {
                anim.SetBool("IsWalking", true);
                agent.destination = idleDes;
            }
            else
            {
                anim.SetBool("IsWalking", false);
                agent.destination = transform.position;
                StartCoroutine("RandomRotation");
                StartCoroutine("RandomIdlePos");
            }
        }
        if (health <= 0)
        {
            StartCoroutine("Death");
        }
    }
    public IEnumerator RandomRotation()
    {
        float newRotation = transform.eulerAngles.y;
        newRotation += Random.Range(1.00f, 3.00f);
        transform.rotation = Quaternion.Euler(0, newRotation, 0);
        yield return new WaitForSeconds(2.5f);
    }
    public IEnumerator RandomIdlePos()
    {
        if (!isIdleWalking)
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
        isIdleWalking = true;
        yield return new WaitForSeconds(10);
    }
    public void CallAgain()
    {
        StartCoroutine("RandomIdlePos");
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
        rb.AddForce(Vector3.up * jumpSpeed * 3);
        isSpawning = true;
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
        isSpawning = false;
    }
    public IEnumerator DoAttack()
    {
        doAttack = true;
        float dis = Vector3.Distance(transform.position, target.transform.position);
		if (dis > attackRange)
		{
            Ray ray = new Ray(transform.position, -transform.up);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, attackRange))
            {
				if (hitInfo.transform.tag == "Player")
				{
                    ResetAnim();
                    hitInfo.transform.GetComponent<PlayerHealth>().Health_Damage(damage);
                    anim.SetBool("IsAttacking", true);
                }
            }
        }
        yield return new WaitForSeconds(attackCoolDown);
        doAttack = false;
    }
    public IEnumerator Death()
    {
        anim.SetBool("isDying", true);
        GiveReward();
        yield return new WaitForSeconds(deathAnimDuration);
        Destroy(gameObject);
    }
    public void ResetAnim()
    {
        anim.SetBool("IsAttacking", false);
        anim.SetBool("IsWalking", false);
    }
    public void GiveReward()
    {

    }
}