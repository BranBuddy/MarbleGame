using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResetGame : MonoBehaviour
{
    [SerializeField] private GameObject gamblingScreenUI;
    [SerializeField] private GameObject bettingContent;
    [SerializeField] private GameObject MarbleSelectionUI;
    [SerializeField] private GameObject winConditionUI;
    [SerializeField] private WinCondition winCondition;

    void Start()
    {
        // If not assigned in Inspector, try to find it on this object or parent
        if (gamblingScreenUI == null)
        {
            gamblingScreenUI = GetComponent<GamblingScreen>()?.gameObject;
        }
        
    }

    /// <summary>
    /// Destroys all marble GameObjects with the Marble tag.
    /// </summary>
    private void DestroyAllMarbles()
    {
        foreach(var marble in GameObject.FindGameObjectsWithTag("Marble"))
        {
            Destroy(marble);
        }
    }

    /// <summary>
    /// Destroys all betting UI element children under bettingContent.
    /// </summary>
    private void DestroyAllBettingElements()
    {
        if (bettingContent == null)
        {
            Debug.LogWarning("ResetGame: bettingContent not assigned.");
            return;
        }

        foreach(Transform child in bettingContent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnDestroy()
    {
        // Optional: Additional cleanup when script is destroyed
    }

    public void OnResetGameButtonPressed()
    {
        Debug.Log("OnResetGameButtonPressed called");

        // Clear marble data and bets
        MarbleManager.Instance.availableMarbles.Clear();
        
        // Destroy all marbles and betting UI elements
        DestroyAllMarbles();
        DestroyAllBettingElements();

        // Reset the win condition state for next round
        if (winCondition != null)
        {
            winCondition.ResetWinState();
        }
        else
        {
            Debug.LogWarning("ResetGame: WinCondition not assigned.");
        }

        // Reset time
        Time.timeScale = 0f;

        // Deactivate win condition UI
        if (winConditionUI != null)
        {
            winConditionUI.SetActive(false);
        }

        // Deactivate this reset button handler
        this.gameObject.SetActive(false);
    }

}
