using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class DropdownLinker : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown numberOfPlayersDropdown;

    [SerializeField] private TMP_Dropdown dropdownPlayer1;
    [SerializeField] private TMP_Dropdown dropdownPlayer2;
    [SerializeField] private TMP_Dropdown dropdownPlayer3;
    [SerializeField] private TMP_Dropdown dropdownPlayer4;

    [SerializeField] private List<Image> playerImages;

    [SerializeField] private bool allowDuplicateSelections = false;

    private List<string> marbleOptions;

    private List<int> noOfPlayersOptions;
    private bool isUpdating = false; // Prevents infinite loop

    private void Start()
    {
        InitializeDropdowns();
    }

    private void InitializeDropdowns()
    {
        // Make sure start-line data is initialized before we read it.
        if (MarbleManager.Instance != null)
        {
            MarbleManager.Instance.EnsurePoolsInitialized();
        }

        // Check if poolOfMarbles is populated
        if (MarbleManager.Instance == null || MarbleManager.Instance.poolOfMarbles == null || MarbleManager.Instance.poolOfMarbles.Count == 0)
        {
            Debug.LogError("DropdownLinker: StartLineManager.poolOfMarbles is empty or not initialized!");
            return;
        }

        // Only use unlocked marbles (pool entries marked true) for dropdown options.
        var sourceUnlocked = MarbleManager.Instance.unlockedMarbles ?? new List<GameObject>();
        if (sourceUnlocked.Count == 0)
        {
            sourceUnlocked = MarbleManager.Instance.poolOfMarbles
                .Where(kvp => kvp.Value && kvp.Key != null)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        marbleOptions = sourceUnlocked
            .Where(go => go != null)
            .Select(go => go.name)
            .Where(n => !string.IsNullOrEmpty(n))
            .Distinct()
            .ToList();
        Debug.Log($"DropdownLinker: Loaded {marbleOptions.Count} marble options: {string.Join(", ", marbleOptions)}");

        noOfPlayersOptions = new List<int> { 1, 2, 3, 4 };

        if (numberOfPlayersDropdown == null)
        {
            Debug.LogError("DropdownLinker: numberOfPlayersDropdown is not assigned in the Inspector!");
            return;
        }

        numberOfPlayersDropdown.ClearOptions();
        numberOfPlayersDropdown.AddOptions(noOfPlayersOptions.Select(i => i.ToString()).ToList());

        List<TMP_Dropdown> dropdowns = new List<TMP_Dropdown> { dropdownPlayer1, dropdownPlayer2, dropdownPlayer3, dropdownPlayer4 };

        for (int i = 0; i < dropdowns.Count; i++)
        {
            var dropdown = dropdowns[i];
            if (dropdown == null)
            {
                Debug.LogError($"DropdownLinker: dropdownPlayer{i+1} is not assigned in the Inspector!");
                continue;
            }
            dropdown.ClearOptions();
            dropdown.AddOptions(marbleOptions);
            Debug.Log($"DropdownLinker: Added {marbleOptions.Count} options to dropdownPlayer{i+1}");
            
            // Set different default value for each dropdown
            if (i < marbleOptions.Count)
            {
                dropdown.value = i;
            }
            
            dropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(); });
        }
        
        // Setup numberOfPlayersDropdown listener
        numberOfPlayersDropdown.onValueChanged.AddListener(delegate { UpdatePlayerDropdownsVisibility(); });
        
        // Update all dropdowns to reflect the different selections
        OnDropdownValueChanged();
        UpdatePlayerDropdownsVisibility();
    }

    private void OnDropdownValueChanged()
    {
        if(allowDuplicateSelections)
        {
            isUpdating = false;
            UpdatePlayerImages();
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

        EditNumberOfPlayersDropdown();
        UpdatePlayerImages();

        isUpdating = false;
    }

    private void UpdatePlayerDropdownsVisibility()
    {
        // Get selected number of players (value is 0-indexed, so add 1)
        int selectedPlayers = numberOfPlayersDropdown.value + 1;
        
        List<TMP_Dropdown> dropdowns = new List<TMP_Dropdown> { dropdownPlayer1, dropdownPlayer2, dropdownPlayer3, dropdownPlayer4 };
        
        for (int i = 0; i < dropdowns.Count; i++)
        {
            if (dropdowns[i] != null)
            {
                // Show dropdown if its index is less than selected number of players
                bool shouldShow = i < selectedPlayers;
                dropdowns[i].gameObject.SetActive(shouldShow);
            }
        }

        UpdatePlayerImages();
    }
    
    private void EditNumberOfPlayersDropdown()
    {
        List<int> availableNoOfPlayersOptions = noOfPlayersOptions.GetRange(0, Mathf.Min(marbleOptions.Count, noOfPlayersOptions.Count));
        PopulateDropdown(numberOfPlayersDropdown, availableNoOfPlayersOptions.ConvertAll(i => i.ToString()), (numberOfPlayersDropdown.value + 1).ToString());
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
        // For numberOfPlayersDropdown, preserve its listener
        bool isNumberOfPlayersDropdown = (dropdown == numberOfPlayersDropdown);
        
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
        
        // Re-add appropriate listener
        if (isNumberOfPlayersDropdown)
        {
            dropdown.onValueChanged.AddListener(delegate { UpdatePlayerDropdownsVisibility(); });
        }
        else
        {
            dropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(); });
        }
    }
    
    internal List<string> GetSelectedMarbles()
    {
        List<string> selectedMarbles = new List<string>();
        List<TMP_Dropdown> dropdowns = new List<TMP_Dropdown> { dropdownPlayer1, dropdownPlayer2, dropdownPlayer3, dropdownPlayer4 };

        foreach (var dropdown in dropdowns)
        {
            // Only add if the dropdown is active (visible) and has options
            if (dropdown.gameObject.activeSelf && dropdown.options.Count > 0)
            {
                selectedMarbles.Add(dropdown.options[dropdown.value].text);
            }
        }

        return selectedMarbles;
    }

    private void UpdatePlayerImages()
    {
        if (playerImages == null || playerImages.Count == 0)
        {
            Debug.LogWarning("DropdownLinker: playerImages list is empty or not assigned!");
            return;
        }

        List<TMP_Dropdown> dropdowns = new List<TMP_Dropdown> { dropdownPlayer1, dropdownPlayer2, dropdownPlayer3, dropdownPlayer4 };

        for (int i = 0; i < playerImages.Count; i++)
        {
            if (i < dropdowns.Count && dropdowns[i] != null && dropdowns[i].gameObject.activeSelf && dropdowns[i].options.Count > 0)
            {
                string marbleName = dropdowns[i].options[dropdowns[i].value].text;
                Sprite marbleSprite = GetMarbleSpriteByName(marbleName);

                if (playerImages[i] != null)
                {
                    playerImages[i].sprite = marbleSprite;
                }
            }
        }
    }

    private Sprite GetMarbleSpriteByName(string marbleName)
    {
        // Try to get the marble prefab from the pool and find its MarbleSO reference
        if (MarbleManager.Instance != null && MarbleManager.Instance.poolOfMarbles != null)
        {
            foreach (var marblePrefab in MarbleManager.Instance.poolOfMarbles.Keys)
            {
                if (marblePrefab != null && marblePrefab.name == marbleName)
                {
                    // Look for MarbleDataReference component
                    MarbleDataReference dataRef = marblePrefab.GetComponent<MarbleDataReference>();
                    if (dataRef != null && dataRef.marbleSO != null && dataRef.marbleSO.marbleSprite != null)
                    {
                        return dataRef.marbleSO.marbleSprite;
                    }

                    Debug.LogWarning($"DropdownLinker: Marble '{marbleName}' prefab is missing MarbleDataReference component or MarbleSO sprite is not assigned.");
                    break;
                }
            }
        }

        Debug.LogWarning($"DropdownLinker: Could not find marble '{marbleName}' in pool.");
        return null;
    }
}
