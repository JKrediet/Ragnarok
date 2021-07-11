using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemList : MonoBehaviour
{
    public static List<GameObject> itemListUi;
    public List<GameObject> itemListIdUi;

    public static List<GameObject> itemListIngame;
    public List<GameObject> itemListIdIngame;

    private void Awake()
    {
        itemListIngame = new List<GameObject>(itemListIdIngame);
        itemListUi = new List<GameObject>(itemListIdUi);
    }
}
