using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Chest : ProbScript
{
    private GameObject listHolder;
    
    public void Awake()
    {
        //pak hier de list holder
    }
    public override void Interaction()
    {
        GetRandomItem();
    }
    public void GetRandomItem()
    {
        CheckSpawnItem();
    }
    public void CheckSpawnItem()
    {
        Vector3 spawnpint = new Vector3(0,1,0);
        SpawnItem(spawnpint);
    }
    public void SpawnItem(Vector3 spawnPoint)
    {

    }
}
