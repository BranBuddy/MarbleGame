using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Manages the gambling/betting UI screen for the marble game.
/// Handles bet placement, gold tracking, multiplier calculations, and reward display.
/// </summary>
public class GamblingScreen : MonoBehaviour
{
    private string input;
    [SerializeField] private TMP_Text goldAmountText;
    [SerializeField] private GameObject bettingContent;
    [SerializeField] private GameObject bettingSlotPrefab; // Add this field to assign the betting slot prefab
    [SerializeField] private DropdownLinker dropdownLinker;
    [SerializeField] private TMP_Text betMultiplierText;
    [SerializeField] private TMP_Text highestBetRewardText;
    private float highestBetRewardValue = 0.0f;
    internal float betMultiplierValue = 2.0f;

    // Update is called once per frame
    void Update()
    {
        goldAmountText.text = PlayerInventoryManager.Instance.goldAmount.ToString();
        UpdateBetMultiplier();
        HighestBetReward();
    }

    /// <summary>
    /// Aggregates bet amounts by player/marble name.
    /// Sums all bets placed on the same marble across multiple UI slots.
    /// </summary>
    public Dictionary<string, int> AggregateBets()
    {
        Dictionary<string, int> aggregatedBets = new Dictionary<string, int>();

        foreach(Transform child in bettingContent.transform)
        {
            MarbleGamblingProfile readGoldInput = child.GetComponentInChildren<MarbleGamblingProfile>();
            if (readGoldInput != null)
            {
                string playerName = child.GetComponentInChildren<TMP_Text>().text;
                int betAmount = readGoldInput.BetAmountValue;

                if (aggregatedBets.ContainsKey(playerName))
                {
                    aggregatedBets[playerName] += betAmount;
                }
                else
                {
                    aggregatedBets[playerName] = betAmount;
                }
            }
        }

        return aggregatedBets;
    }

    /// <summary>
    /// Calculates and displays the highest potential reward.
    /// Finds the largest single bet and multiplies it by the current multiplier.
    /// Bug fixes: Handles empty bets (returns 0) and uses single largest bet instead of aggregating duplicates.
    /// </summary>
    private void HighestBetReward()
    {
        float maxBet = 0.0f;

        foreach(Transform child in bettingContent.transform)
        {
            MarbleGamblingProfile readGoldInput = child.GetComponentInChildren<MarbleGamblingProfile>();
            if (readGoldInput != null)
            {
                // Track the largest single bet rather than aggregating duplicate names.
                maxBet = Mathf.Max(maxBet, readGoldInput.BetAmountValue);
            }
        }

        highestBetRewardValue = maxBet * betMultiplierValue;
        highestBetRewardText.text = highestBetRewardValue.ToString("0.##");
    }

    /// <summary>
    /// Updates the bet multiplier based on the number of unique bets placed.
    /// Counts unique bets and applies the appropriate multiplier from the lookup table.
    /// More unique bets = lower multiplier (risk/reward balance).
    /// </summary>
    private void UpdateBetMultiplier()
    {
        int totalUniqueBets = 0;

        foreach(Transform child in bettingContent.transform)
        {
            MarbleGamblingProfile readGoldInput = child.GetComponentInChildren<MarbleGamblingProfile>();
            if (readGoldInput != null && readGoldInput.uniqueBet)
            {
                totalUniqueBets++;
            }
        }

        UniqueBetTable(totalUniqueBets);
        int availableMarbles = MarbleManager.Instance.availableMarbles.Count;

        if (availableMarbles <= 0)
        {
            betMultiplierText.text = betMultiplierValue.ToString("0.##") + "x";
            return;
        }

        // More competition (more available marbles left unpicked) boosts multiplier;
        // spreading bets out (higher unique count) reduces it.
        float competitionBonus = 1f + Mathf.Max(0, availableMarbles - totalUniqueBets) / (float)availableMarbles;
        betMultiplierValue *= competitionBonus;

        // Cap the multiplier to the player/marble count (e.g., 2 players => max 2x, 3 players => max 3x).
        betMultiplierValue = Mathf.Min(betMultiplierValue, availableMarbles);

        betMultiplierText.text = betMultiplierValue.ToString("0.##") + "x";


    }

    /// <summary>
    /// Lookup table for bet multipliers based on number of unique bets.
    /// 1 unique bet = 2.0× (high risk, high reward)
    /// 2 unique bets = 1.75×
    /// 3 unique bets = 1.5×
    /// 4 unique bets = 1.25× (low risk, low reward)
    /// </summary>
    private void UniqueBetTable(int uniqueBets)
    {
        switch (uniqueBets)
        {
            case 1:
                betMultiplierValue = 2.0f;
                break;
            case 2:
                betMultiplierValue = 1.75f;
                break;
            case 3:
                betMultiplierValue = 1.5f;
                break;
            case 4:
                betMultiplierValue = 1.25f;
                break;
            default:
                betMultiplierValue = 2.0f;
                break;
        }

        
    }

    /// <summary>
    /// Initializes the gambling screen with selected marble names.
    /// Populates each betting slot with a marble name from the dropdown selector or available marbles.
    /// </summary>
    public void InitializeGamblingScreen()
    {

        if (bettingContent == null)
        {
            Debug.LogError("GamblingScreen: bettingContent is not assigned in the Inspector!");
            return;
        }
        
        // Get selected marble names
        List<string> selectedNames = new List<string>();
        if (dropdownLinker != null)
        {
            selectedNames.AddRange(dropdownLinker.GetSelectedMarbles());
        }
        else
        {
            if (MarbleManager.Instance != null && MarbleManager.Instance.availableMarbles != null)
            {
                for (int i = 0; i < MarbleManager.Instance.availableMarbles.Count; i++)
                {
                    selectedNames.Add(MarbleManager.Instance.availableMarbles[i].name);
                }
            }
            else
            {
                Debug.LogError("GamblingScreen: MarbleManager.Instance or availableMarbles is null!");
                return;
            }
        }
        
        BettingContentCleanup();
        
        // Create new slots based on selected marbles
        List<GameObject> marbleAccounts = new List<GameObject>();
        
        if (bettingSlotPrefab == null)
        {
            Debug.LogError("GamblingScreen: bettingSlotPrefab is not assigned!");
            return;
        }
        
        for (int i = 0; i < selectedNames.Count; i++)
        {
            GameObject newSlot = Instantiate(bettingSlotPrefab, bettingContent.transform);
            marbleAccounts.Add(newSlot);
        }
        
        // Assign marble names to slots
        int assignCount = Mathf.Min(marbleAccounts.Count, selectedNames.Count);
        
        for (int i = 0; i < assignCount; i++)
        {
            MarbleGamblingProfile profile = marbleAccounts[i].GetComponentInChildren<MarbleGamblingProfile>();
            if (profile != null)
            {
                profile.SetMarbleName(selectedNames[i]);
            }
            else
            {
                marbleAccounts[i].GetComponentInChildren<TMP_Text>().text = selectedNames[i];
            }
        }
        
    }

    private void BettingContentCleanup()
    {
        List<Transform> childrenToDestroy = new List<Transform>();
        foreach (Transform child in bettingContent.transform)
        {
            childrenToDestroy.Add(child);
        }

        foreach (Transform child in childrenToDestroy)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}
