using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class TrowState : AttackState
{
    public GameObject trowObject;
    public TrowObject objectScript;
    public Rigidbody objectRb;
    public GameObject enemie;
    public string trowObjectName;
    public float trowCooldownTime;
    private GameObject newTrowable;
    public override State RunCurrentState()
    {
        if (sm.isDead)
        {
            return this;
        }
        float dist = Vector3.Distance(transform.position, sm.target.transform.position);
        if (dist > 0.35)
        {
            FaceTarget(sm.target.transform.position);
        }
        if (dist <= sm.triggerRange)
        {
            if (sm.hasRangedAtt && dist <= sm.attackRange * 3.5f && !sm.trowCoolDown)
            {
                if (!sm.doAttack)
                {
                    Vector3 forward = transform.TransformDirection(Vector3.forward);
                    Vector3 toOther = sm.target.transform.position - transform.position;
                    if (Vector3.Dot(forward, toOther) > 0)
                    {
                        int randomI = Random.Range(0, sm.attackStates.Length);
                        if (!sm.trowCoolDown)
                        {
                            DoAttack();
                        }
                        return this;
                    }
                }
                else
                {
                    sm.doAttack = false;
                    return trigger;
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
    private void FaceTarget(Vector3 destination)
    {
        Vector3 lookPos = destination - enemie.transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        enemie.transform.rotation = Quaternion.Slerp(transform.rotation, rotation, sm.AttrotationSpeed);
    }
    public void DoAttack()
    {
        if (sm.trowCoolDown)
        {
            sm.ResetAnim();
        }
        else
        {
            sm.StartCoroutine(sm.CoolDownTrow(trowCooldownTime));
            agent.destination = enemie.transform.position;
            sm.anim.SetBool(animationName, true);
            agent.speed = sm.attackMovementSpeed;
            sm.trowCoolDown = true;
        }
    }
    [PunRPC]
    public void SpawnNewHamer()
    {
        newTrowable = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", trowObjectName), trowObject.transform.position, trowObject.transform.rotation);
        newTrowable.transform.parent = trowObject.transform.parent;
    }
    public void Trow()
	{
        GetComponent<PhotonView>().RPC("SpawnNewHamer", RpcTarget.MasterClient);
        newTrowable.SetActive(false);
        objectScript.target = sm.target;
        objectRb.isKinematic = false;
        objectRb.useGravity = true;
        objectScript.activated = true;
        objectRb = newTrowable.transform.GetComponent<Rigidbody>();
        objectScript = newTrowable.transform.GetComponent<TrowObject>();
        trowObject.transform.parent = null;
        trowObject = newTrowable;
        newTrowable = null;
        Invoke("SetActive",1);
    }
    public void SetActive()
	{
        trowObject.SetActive(true);
    }
}
