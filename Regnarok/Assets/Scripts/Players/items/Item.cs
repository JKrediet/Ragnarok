using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Item : MonoBehaviour
{
    public int itemType; //0=default / 1= tool / 2=equipment
    public int itemId;
    public int itemDamage;
    public bool isEquipment, stackAble;
    public int stackAmount;
    public int oldSlotNumber;
    [HideInInspector] public Inventory inv;

    private bool isInInventory, mayBePickedUp;

    public TextMeshProUGUI stackAmountText;

    private bool isBeingDragged;

    public void Awake()
    {
        //inv = FindObjectOfType<Inventory>();
    }
    private void Start()
    {
        mayBePickedUp = false;
        Invoke("Cooldown", 0.5f);
    }
    public void Cooldown()
    {
        mayBePickedUp = true;
    }

    public void ToInventory(bool value)
    {
        isInInventory = value;
    }
    protected void OnTriggerEnter(Collider other)
    {
        if (mayBePickedUp)
        {
            if (!isInInventory)
            {
                if (other.CompareTag("Player"))
                {
                    other.GetComponent<Inventory>().AddItemFromOutsideOfInventory(itemId, stackAmount);
                    GetComponent<PhotonView>().RPC("DestroyWorldItem", RpcTarget.MasterClient);
                }
            }
        }
    }
    [PunRPC]
    public void DestroyWorldItem()
    {
        PhotonNetwork.Destroy(gameObject);
    }
    public void SetUp(int numberOfItems, int slot, Inventory _inv)
    {
        GetComponent<Image>().raycastTarget = true;
        ToInventory(true);
        stackAmount = numberOfItems;
        oldSlotNumber = slot;
        inv = _inv;
        if (stackAmountText != null)
        {
            stackAmountText.gameObject.SetActive(true);
            stackAmountText.text = stackAmount.ToString();
        }
        
    }
    public void BeginDrag()
    {
        if (isInInventory)
        {
            if (!inv.itemBeingDragged)
            {
                inv.itemBeingDragged = true;
                //get mouse pos
                transform.SetParent(inv.BeginDrag());
                transform.position = Input.mousePosition;
            }
            else
            {
                inv.itemBeingDragged = false;
                inv.AddItemToInventoryList(-1, -1, true, -1);
            }
        }
        //end drag in inventory-script
    }
    private void Update()
    {
        if(isInInventory)
        {
            if (inv.itemBeingDragged)
            {
                if (GetComponent<Image>())
                {
                    GetComponent<Image>().raycastTarget = !inv.itemBeingDragged;
                }
            }
        }
    }
}
