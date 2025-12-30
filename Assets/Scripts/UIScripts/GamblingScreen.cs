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
    [SerializeField] internal float goldAmount;
    [SerializeField] private TMP_Text goldAmountText;
    [SerializeField] private GameObject bettingContent;
    [SerializeField] private DropdownLinker dropdownLinker;
    [SerializeField] private TMP_Text betMultiplierText;
    [SerializeField] private TMP_Text highestBetRewardText;
    private float highestBetRewardValue = 0.0f;
    internal float betMultiplierValue = 2.0f;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        goldAmountText.text = goldAmount.ToString();
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

        Debug.Log($"bettingContent children count: {bettingContent.transform.childCount}");

        foreach(Transform child in bettingContent.transform)
        {
            ReadGoldInput readGoldInput = child.GetComponentInChildren<ReadGoldInput>();
            if (readGoldInput != null)
            {
                string playerName = child.GetComponentInChildren<TMP_Text>().text;
                int betAmount = readGoldInput.BetAmountValue;
                Debug.Log($"Found bet - Player: {playerName}, Amount: {betAmount}");

                if (aggregatedBets.ContainsKey(playerName))
                {
                    aggregatedBets[playerName] += betAmount;
                }
                else
                {
                    aggregatedBets[playerName] = betAmount;
                }
            }
            else
            {
                Debug.Log($"No ReadGoldInput found in child: {child.name}");
            }
        }

        Debug.Log($"Total aggregated bets: {aggregatedBets.Count}");
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
            ReadGoldInput readGoldInput = child.GetComponentInChildren<ReadGoldInput>();
            if (readGoldInput != null)
            {
                // Track the largest single bet rather than aggregating duplicate names.
                maxBet = Mathf.Max(maxBet, readGoldInput.BetAmountValue);
            }
        }

        highestBetRewardValue = maxBet * betMultiplierValue;
        highestBetRewardText.text = highestBetRewardValue.ToString();
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
            ReadGoldInput readGoldInput = child.GetComponentInChildren<ReadGoldInput>();
            if (readGoldInput != null && readGoldInput.uniqueBet)
            {
                totalUniqueBets++;
            }
        }

        Debug.Log("Total Unique Bets: " + totalUniqueBets);

        UniqueBetTable(totalUniqueBets);
        betMultiplierText.text = betMultiplierValue.ToString() + "x";

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
        List<GameObject> marbleAccounts = new List<GameObject>();

        foreach (Transform child in bettingContent.transform)
        {
            marbleAccounts.Add(child.gameObject);
        }

        List<string> selectedNames = new List<string>();
        if (dropdownLinker != null)
        {
            selectedNames.AddRange(dropdownLinker.GetSelectedMarbles());
        }
        else
        {
            for (int i = 0; i < StartLineManager.Instance.availableMarbles.Count; i++)
            {
                selectedNames.Add(StartLineManager.Instance.availableMarbles[i].name);
            }
        }

        int assignCount = Mathf.Min(marbleAccounts.Count, selectedNames.Count);
        for (int i = 0; i < assignCount; i++)
        {
            marbleAccounts[i].GetComponentInChildren<TMP_Text>().text = selectedNames[i];
        }
    }
}
