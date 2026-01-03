using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuVisibility : MonoBehaviour
{
    [SerializeField] private List<GameObject> visibleMenus;
    [SerializeField] private GameObject startingMenu;

    private void Start()
    {
        InitliazeMenuList();
    }

    private void InitliazeMenuList()
    {
        visibleMenus = new List<GameObject>();
        visibleMenus.Add(startingMenu);
        visibleMenus[0].SetActive(true);
    }

    public void AddMenuToList(GameObject menu)
    {
        if (!visibleMenus.Contains(menu))
        {
            visibleMenus.Add(menu);
            menu.SetActive(true);
        }
    }

    public void RemoveMenuFromList()
    {
        visibleMenus[visibleMenus.Count - 1].SetActive(false);
        visibleMenus.RemoveAt(visibleMenus.Count - 1);
    }

    public void ClearMenuList()
    {
        foreach (var menu in visibleMenus)
        {
            menu.SetActive(false);
        }
        visibleMenus.Clear();
    }
}
