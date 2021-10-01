using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChestScript : ProbScript
{
    public int cost;
    public GameObject itemSpawnPos;
    public enum ChestType { small=0,medium =1, big =2, golden =3}
    public ChestType type;
    public int chestId;
    public Animator anim;
    private GameManager gm;
    public List<GameObject> players;
    private GameObject isMyPlayer;
    
	public override void Interaction()
	{
        players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
		for (int i = 0; i < players.Count; i++)
		{
			if (players[i].GetComponent<PhotonView>().IsMine)
			{
                isMyPlayer = players[i];
			}
		}
		if (isMyPlayer.GetComponent<PlayerController>().playerBalance >= cost)
        { 
            gm=FindObjectOfType<GameManager>();
            StartCoroutine("GetRandomItem");
            canInteract = false;
		}
		else
		{
            //doet sound dat die te weinig geld heeft
		}
    }
	public IEnumerator GetRandomItem()
    { 
        gm.OpenChest(chestId);
        yield return new WaitForSeconds(0.5f);
        gm.SpawnItem(itemSpawnPos.transform.position, RaretyChance());
    }
    public int RaretyChance()
    {
        float randomNum = Random.Range(0.00f, 100.00f);
        int itemRarity = 0;
   
        if (type==ChestType.small)
        {
            if (randomNum <= 68.5f){//common
                itemRarity = 0;}
            else if (randomNum > 68.5f && randomNum <= 93.5f){//rare
                itemRarity = 1;}
            else if (randomNum > 93.5f && randomNum <= 98.5f){//epic
                itemRarity = 2;}
            else if (randomNum > 98.5f && randomNum <= 99.5f){//legendary
                itemRarity = 3;}
            else if (randomNum > 99.5f){//mythic
                itemRarity = 4;}
        }
        else if (type == ChestType.medium)
        {
            if (randomNum <= 27.5f){//common
                itemRarity = 0;}
            else if (randomNum > 27.5f && randomNum <= 82.5f){//rare
                itemRarity = 1;}
            else if (randomNum > 82.5f && randomNum <= 92.5f){//epic
                itemRarity = 2;}
            else if (randomNum > 92.5f && randomNum <= 97.5f){//legendary
                itemRarity = 3;}
            else if (randomNum > 97.5f){//mythic
                itemRarity = 4;}
        }
        else if (type == ChestType.big)
        {
            if (randomNum <= 24.2f){//common
                itemRarity = 0;}
            else if (randomNum > 24.2f && randomNum <= 60.5f){//rare
                itemRarity = 1;}
            else if (randomNum > 60.5f && randomNum <= 80){//epic
                itemRarity = 2;}
            else if (randomNum > 80 && randomNum <= 94){//legendary
                itemRarity = 3;}
            else if (randomNum > 94){//mythic
                itemRarity = 4;}
        }
        else if (type == ChestType.golden)
        {
            if (randomNum <= 5){//common
                itemRarity = 0;}
            else if (randomNum > 5 && randomNum <= 25){//rare
                itemRarity = 1;}
            else if (randomNum > 25 && randomNum <= 65){//epic
                itemRarity = 2;}
            else if (randomNum > 65 && randomNum <= 85){//legendary
                itemRarity = 3;}
            else if (randomNum > 85){//mythic
                itemRarity = 4;}
        }
        //if(itemRarity == 0){
        //    int roll = Random.Range(0, 5);
        //    return roll;}
        //else if (itemRarity == 1){
        //    int roll = Random.Range(6, 11);
        //    return roll;}
        //else if (itemRarity == 2){
        //    int roll = Random.Range(12, 17);
        //    return roll;}
        //else if (itemRarity == 3){
        //    int roll = Random.Range(18, 22);
        //    return roll;}
        //else if (itemRarity == 4){
        //    int roll = Random.Range(23, 25);
        //    return roll;}
        return itemRarity;
    }
}
