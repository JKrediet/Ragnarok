using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] Image image;
    public int slotID;

    private Color normalColor = Color.white;
    private Color disabledColor = new Color(1, 1, 1, 0);

    private Item _item;
    public Item item
    {
        get { return _item; }
        set {
            _item = value;

            if(_item == null)
            {
                image.color = disabledColor;
            }
            else
            {
                image.sprite = _item.icon;
                image.color = normalColor;
            }
        }
    }

    protected virtual void OnValidate()
    {
        if(image == null)
        {
            image = GetComponent<Image>();
        }
    }

    public virtual bool CanRecieveItem(Item item)
    {
        return true;
    }
}
