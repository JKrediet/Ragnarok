using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager menuSwitch;
    public List<Menu> menus;

    private void Awake()
    {
        menuSwitch = this;
        ChangeMenu("Loading");
    }
    public void ChangeMenu(string _name)
    {
        for (int i = 0; i < menus.Count; i++)
        {
            menus[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < menus.Count; i++)
        {
            if (menus[i].name == _name)
            {
                menus[i].gameObject.SetActive(true);
            }
        }
    }
    public void Quit()
    {
        Application.Quit();
    }
}
