using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAttack : MonoBehaviour
{
    public void DoneAttack()
    {
        GetComponentInParent<PlayerController>().DoneAttacking();
    }
    public void DoDamage()
    {
        GetComponentInParent<PlayerController>().Attack();
    }
}
