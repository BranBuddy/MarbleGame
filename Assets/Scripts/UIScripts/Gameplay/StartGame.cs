using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] private GamblingScreen gamblingScreenUI;
    private WinCondition winCondition;

    void Start()
    {
        // If not assigned in Inspector, try to find it on this object or parent
        if (gamblingScreenUI == null)
        {
            gamblingScreenUI = GetComponent<GamblingScreen>();
        }
        
        winCondition = GameObject.FindWithTag("WinCondition").GetComponent<WinCondition>();
    }

    public void OnStartGameButtonPressed()
    {
        Debug.Log("OnStartGameButtonPressed called");
        
        if (gamblingScreenUI != null)
        {
            gamblingScreenUI.gameObject.SetActive(false);
        }

        Time.timeScale = 1f;
        
        if (winCondition != null && gamblingScreenUI != null)
        {
            // Pass aggregated bets to WinCondition so it knows player rewards
            var bets = gamblingScreenUI.AggregateBets();
            Debug.Log($"Bets from GamblingScreen: {bets.Count} entries - {string.Join(", ", bets.Keys)}");
            winCondition.AggregateBets = bets;
            Debug.Log($"Bets passed to WinCondition: {winCondition.AggregateBets.Count} entries");
        }
        else
        {
            Debug.LogError($"Missing references - winCondition: {(winCondition != null)}, gamblingScreenUI: {(gamblingScreenUI != null)}");
        }

    }
}
