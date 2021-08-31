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
    public int stackAmount, maxStackAmount;
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
                    if(GetComponent<PhotonView>().Owner != PhotonNetwork.MasterClient)
                    {
                        GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);
                    }
                    GetComponent<PhotonView>().RPC("DestroyWorldItem", RpcTarget.MasterClient);
                    Invoke("SecLater", 0.1f);
                }
            }
        }
    }
    void SecLater()
    {
        GetComponent<PhotonView>().RPC("DestroyWorldItem", RpcTarget.MasterClient);
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
                //ctrl click move items to other inv
                if (Input.GetButton("ControlClick"))
                {
                    //wip
                    transform.SetParent(inv.BeginDrag());
                    transform.position = Input.mousePosition;
                }
                //shift click for half stack
                else if (Input.GetButton("Sprint"))
                {
                    if(stackAmount > 1)
                    {
                        GameObject temp = Instantiate(ItemList.itemListUi[itemId], inv.BeginDrag());
                        stackAmount /= 2;
                        temp.GetComponent<Item>().SetUp(stackAmount, -1, inv);
                        SetUp(stackAmount, oldSlotNumber, inv);
                        temp.transform.position = Input.mousePosition;
                    }
                    else
                    {
                        //get mouse pos
                        transform.SetParent(inv.BeginDrag());
                        transform.position = Input.mousePosition;
                    }
                }
                //normal pickup
                else
                {
                    //get mouse pos
                    transform.SetParent(inv.BeginDrag());
                    transform.position = Input.mousePosition;
                }
            }
            else
            {
                inv.itemBeingDragged = false;
                inv.AddItemToInventoryList(-1, -1, true);
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
