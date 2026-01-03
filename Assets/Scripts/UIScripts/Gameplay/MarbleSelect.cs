using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

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
        LoadInSelectedMarbles();
    }

    public void LoadInSelectedMarbles()
    {
       foreach(var marbleName in dropdownLinker.GetSelectedMarbles())
       {
            var match = MarbleManager.Instance.poolOfMarbles
                .FirstOrDefault(pair => pair.Key != null && pair.Key.name == marbleName);
            GameObject prefab = match.Key;

            if (prefab != null)
            {
                GameObject clone = Instantiate(prefab);
                MarbleManager.Instance.availableMarbles.Add(clone);
            }
       }

        MarbleManager.Instance.PlaceMarblesAtStartLine();
        Time.timeScale = 0;
    }
}
