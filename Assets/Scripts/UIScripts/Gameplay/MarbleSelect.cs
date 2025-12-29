using System;
using UnityEngine;

public class MarbleSelect : MonoBehaviour
{
    [SerializeField] private GameObject selectMarbleScreen;
    [SerializeField] private DropdownLinker dropdownLinker;

    public void Start()
    {
        selectMarbleScreen.SetActive(true);
        dropdownLinker = GetComponent<DropdownLinker>();
    }

    public void OnStartGameButtonPressed()
    {
        selectMarbleScreen.SetActive(false);
        LoadInSelectedMarbles();
    }

    public void LoadInSelectedMarbles()
    {
       foreach(var marbleName in dropdownLinker.GetSelectedMarbles())
       {
            GameObject prefab = StartLineManager.Instance.poolOfMarbles.Find(m => m.name == marbleName);

            if (prefab != null)
            {
                GameObject clone = Instantiate(prefab);
                StartLineManager.Instance.availableMarbles.Add(clone);
            }
       }

        StartLineManager.Instance.PlaceMarblesAtStartLine();
    }
}
