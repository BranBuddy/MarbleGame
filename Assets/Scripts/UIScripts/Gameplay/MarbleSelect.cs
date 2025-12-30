using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class MarbleSelect : MonoBehaviour
{
    [SerializeField] private GameObject selectMarbleScreen;
    [SerializeField] private DropdownLinker dropdownLinker;
    [SerializeField] private GameObject playerInformationPrefab;
    [SerializeField] private GameObject playerInformationContent;

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
            var match = StartLineManager.Instance.poolOfMarbles
                .FirstOrDefault(pair => pair.Key != null && pair.Key.name == marbleName);
            GameObject prefab = match.Key;

            if (prefab != null)
            {
                GameObject clone = Instantiate(prefab);
                StartLineManager.Instance.availableMarbles.Add(clone);
            }
       }

        StartLineManager.Instance.PlaceMarblesAtStartLine();
        Time.timeScale = 0;

        for(int i = 0; i < StartLineManager.Instance.availableMarbles.Count; i++)
        {
            GameObject playerInfoEntry = Instantiate(playerInformationPrefab, playerInformationContent.transform);
        }
    }
}
