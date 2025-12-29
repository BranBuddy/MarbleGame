using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DropdownLinker : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdownPlayer1;
    [SerializeField] private TMP_Dropdown dropdownPlayer2;
    [SerializeField] private TMP_Dropdown dropdownPlayer3;
    [SerializeField] private TMP_Dropdown dropdownPlayer4;

    [SerializeField] private bool allowDuplicateSelections = false;

    private List<string> marbleOptions;
    private bool isUpdating = false; // Prevents infinite loop

    private void Start()
    {
        InitializeDropdowns();
    }

    private void InitializeDropdowns()
    {
        marbleOptions = StartLineManager.Instance.poolOfMarbles.ConvertAll(marble => marble.name);

        List<TMP_Dropdown> dropdowns = new List<TMP_Dropdown> { dropdownPlayer1, dropdownPlayer2, dropdownPlayer3, dropdownPlayer4 };

        for (int i = 0; i < dropdowns.Count; i++)
        {
            var dropdown = dropdowns[i];
            dropdown.ClearOptions();
            dropdown.AddOptions(marbleOptions);
            
            // Set different default value for each dropdown
            if (i < marbleOptions.Count)
            {
                dropdown.value = i;
            }
            
            dropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(); });
        }
        
        // Update all dropdowns to reflect the different selections
        OnDropdownValueChanged();
    }

    private void OnDropdownValueChanged()
    {
        if(allowDuplicateSelections)
        {
            isUpdating = false;
            return; // No need to update options if duplicates are allowed
        }

        if (isUpdating) return; // Prevent infinite loop
        isUpdating = true;

        string selected1 = (dropdownPlayer1.options.Count > 0) ? dropdownPlayer1.options[dropdownPlayer1.value].text : "";
        string selected2 = (dropdownPlayer2.options.Count > 0) ? dropdownPlayer2.options[dropdownPlayer2.value].text : "";
        string selected3 = (dropdownPlayer3.options.Count > 0) ? dropdownPlayer3.options[dropdownPlayer3.value].text : "";
        string selected4 = (dropdownPlayer4.options.Count > 0) ? dropdownPlayer4.options[dropdownPlayer4.value].text : "";

        
        List<string> availableOptionsFor1 = GetAvailableOptions(selected1, selected2, selected3, selected4);
        List<string> availableOptionsFor2 = GetAvailableOptions(selected2, selected1, selected3, selected4);
        List<string> availableOptionsFor3 = GetAvailableOptions(selected3, selected1, selected2, selected4);
        List<string> availableOptionsFor4 = GetAvailableOptions(selected4, selected1, selected2, selected3);

        PopulateDropdown(dropdownPlayer1, availableOptionsFor1, selected1);
        PopulateDropdown(dropdownPlayer2, availableOptionsFor2, selected2);
        PopulateDropdown(dropdownPlayer3, availableOptionsFor3, selected3);
        PopulateDropdown(dropdownPlayer4, availableOptionsFor4, selected4);

        isUpdating = false;
    }

    List<string> GetAvailableOptions(string currentSelection, params string[] otherSelections)
    {
        List<string> available = new List<string>(marbleOptions);

        // Remove selections from OTHER dropdowns, but keep the current one
        foreach (var sel in otherSelections)
        {
            if (!string.IsNullOrEmpty(sel) && available.Contains(sel))
            {
                available.Remove(sel);
            }
        }

        return available;
    }

    private void PopulateDropdown(TMP_Dropdown dropdown, List<string> options, string currentSelection)
    {
        // Remove listener to prevent triggering during update
        dropdown.onValueChanged.RemoveAllListeners();
        
        dropdown.ClearOptions();
        dropdown.AddOptions(options);

        int newIndex = options.IndexOf(currentSelection);
        if(newIndex != -1)
        {
            dropdown.value = newIndex;
        }
        else if (options.Count > 0)
        {
            dropdown.value = 0;
        }
        dropdown.RefreshShownValue();
        
        // Re-add listener
        dropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(); });
    }
    
    internal List<string> GetSelectedMarbles()
    {
        List<string> selectedMarbles = new List<string>();

        if (dropdownPlayer1.options.Count > 0)
            selectedMarbles.Add(dropdownPlayer1.options[dropdownPlayer1.value].text);
        if (dropdownPlayer2.options.Count > 0)
            selectedMarbles.Add(dropdownPlayer2.options[dropdownPlayer2.value].text);
        if (dropdownPlayer3.options.Count > 0)
            selectedMarbles.Add(dropdownPlayer3.options[dropdownPlayer3.value].text);
        if (dropdownPlayer4.options.Count > 0)
            selectedMarbles.Add(dropdownPlayer4.options[dropdownPlayer4.value].text);

        return selectedMarbles;
    }
}
