using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

/// <summary>
/// Manages win condition logic when a marble reaches the finish line.
/// Declares the winner, calculates rewards, and updates UI.
/// </summary>
public class WinCondition : MonoBehaviour
{
    private GameObject currentWinner;
    private bool winnerDeclared = false;
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private GameObject resetButton;
    [SerializeField] private GamblingScreen gamblingScreen;

    internal Dictionary<string, int> AggregateBets = new Dictionary<string, int>();

    public void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Marble"))
            return;

        // Prevent multiple winners
        if (currentWinner != null || winnerDeclared)
            return;

        SetMarbleAsGameOver(other.gameObject);
        DeclareWinner(other.gameObject);
    }

    /// <summary>
    /// Marks the marble as game over by setting its IMarbles component flag.
    /// </summary>
    private void SetMarbleAsGameOver(GameObject marble)
    {
        var marbleComponent = marble.GetComponent<IMarbles>();
        if (marbleComponent != null)
        {
            marbleComponent.isGameOver = true;
        }
        else
        {
            Debug.LogError($"WinCondition: IMarbles not found on {marble.name}");
        }
    }

    /// <summary>
    /// Declares the marble as the winner and displays the reward.
    /// </summary>
    private void DeclareWinner(GameObject winnerMarble)
    {
        winnerDeclared = true;
        currentWinner = winnerMarble;
        CleanMarbleName(currentWinner);
        
        ShowResetButton();
        DisplayWinnerInfo();
    }

    /// <summary>
    /// Displays the winner text and gold reward on screen.
    /// </summary>
    private void DisplayWinnerInfo()
    {
        if (winnerText == null)
        {
            Debug.LogError("WinCondition: winnerText is not assigned in the Inspector.");
            return;
        }

        int goldReward = GetRewardForWinner(currentWinner.name);
        string winMessage = FormatWinMessage(currentWinner.name, goldReward);
        
        winnerText.gameObject.SetActive(true);
        winnerText.text = winMessage;
        
        AddRewardToGamblingScreen(goldReward);
    }

    /// <summary>
    /// Retrieves the gold reward amount for the winner marble.
    /// </summary>
    private int GetRewardForWinner(string marbleName)
    {
        return AggregateBets.ContainsKey(marbleName) ? AggregateBets[marbleName] : 0;
    }

    /// <summary>
    /// Formats the winner announcement message.
    /// </summary>
    private string FormatWinMessage(string marbleName, int goldReward)
    {
        return $"Winner is: {marbleName}\n You earned {goldReward * gamblingScreen.betMultiplierValue} Gold!";
    }

    /// <summary>
    /// Adds the reward gold to the gambling screen total.
    /// </summary>
    private void AddRewardToGamblingScreen(int goldReward)
    {
        if (gamblingScreen != null)
        {
            gamblingScreen.goldAmount += goldReward * gamblingScreen.betMultiplierValue;
        }
        else
        {
            Debug.LogWarning("WinCondition: GamblingScreen reference not assigned.");
        }
    }

    /// <summary>
    /// Removes the "(Clone)" suffix from marble names and updates the GameObject name.
    /// </summary>
    private void CleanMarbleName(GameObject marble)
    {
        string cleanName = marble.name;
        if (cleanName.Contains("(Clone)"))
        {
            cleanName = cleanName.Replace("(Clone)", "").Trim();
            marble.name = cleanName;
        }
    }

    /// <summary>
    /// Shows the reset button on the UI.
    /// </summary>
    private void ShowResetButton()
    {
        if (resetButton != null)
        {
            resetButton.SetActive(true);
        }
        else
        {
            Debug.LogWarning("WinCondition: resetButton is not assigned in the Inspector.");
        }
    }

    /// <summary>
    /// Resets the win condition state for a new round.
    /// Called by ResetGame when the player restarts.
    /// </summary>
    public void ResetWinState()
    {
        currentWinner = null;
        winnerDeclared = false;
        AggregateBets.Clear();
        
        if (winnerText != null)
        {
            winnerText.gameObject.SetActive(false);
        }
        
        if (resetButton != null)
        {
            resetButton.SetActive(false);
        }
        
        Debug.Log("WinCondition: Win state reset for new round.");
    }
}
