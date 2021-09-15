using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnState : State
{
    public StateManager sm;
    public State idleState;
    [Space(5)]
    public Rigidbody rb;
    public Animator anim;
    public NavMeshAgent agent;
    public float jumpSpeed;
    private bool isSpawning;
	public override State RunCurrentState()
	{
		if (isSpawning)
		{
            return this;
		}
		else
		{
            return idleState;
        }
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
        sm.spawned = true;
    }
}