using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class MarbleGamblingProfile : MonoBehaviour
{
    private string playerGoldBetInput;
    private GamblingScreen gamblingScreen;

    [SerializeField] private Image insufficientGoldWarningImage;
    [SerializeField] private TMP_Text BetAmountText;
    [SerializeField] private Image marbleProfileImage;
    [SerializeField] private TMP_Text marbleNameText;
    internal int BetAmountValue;

    [SerializeField] internal bool uniqueBet = false;

    private string marbleName;

    private void Start()
    {
        GameObject grandparentGameObject = transform.parent?.parent?.gameObject;

        if (grandparentGameObject != null)
        {
            gamblingScreen = grandparentGameObject.GetComponent<GamblingScreen>();
        }
        else
        {
            Debug.LogError("MarbleGamblingProfile: Could not find grandparent GameObject.");
        }
    }

    /// <summary>
    /// Sets the marble name for this profile and updates the display
    /// </summary>
    public void SetMarbleName(string name)
    {
        marbleName = name;
        
        // Update the name text if reference exists
        if (marbleNameText != null)
        {
            marbleNameText.text = marbleName;
        }
        else
        {
            Debug.LogWarning("MarbleGamblingProfile: marbleNameText is not assigned in Inspector!");
        }
        
        // Update the marble image
        UpdateMarbleImage();
    }

    private void UpdateMarbleImage()
    {
        if (marbleProfileImage == null)
        {
            Debug.LogWarning("MarbleGamblingProfile: marbleProfileImage is not assigned.");
            return;
        }

        if (string.IsNullOrEmpty(marbleName))
        {
            return;
        }

        Sprite marbleSprite = GetMarbleSpriteByName(marbleName);

        if (marbleSprite != null)
        {
            marbleProfileImage.sprite = marbleSprite;
        }
        else
        {
            Debug.LogWarning($"MarbleGamblingProfile: Failed to get sprite for '{marbleName}'");
        }
    }

    private Sprite GetMarbleSpriteByName(string name)
    {
        if (MarbleManager.Instance == null || MarbleManager.Instance.poolOfMarbles == null)
        {
            return null;
        }

        foreach (var marblePrefab in MarbleManager.Instance.poolOfMarbles.Keys)
        {
            if (marblePrefab != null && marblePrefab.name == name)
            {
                MarbleDataReference dataRef = marblePrefab.GetComponent<MarbleDataReference>();
                if (dataRef != null && dataRef.marbleSO != null && dataRef.marbleSO.marbleSprite != null)
                {
                    return dataRef.marbleSO.marbleSprite;
                }
                return null;
            }
        }

        return null;
    }

    public void ReadGoldBetInput(string input)
    {
        playerGoldBetInput = input;
        Debug.Log("Player Gold Bet Input: " + playerGoldBetInput);

        if (!int.TryParse(playerGoldBetInput, out var betAmount) || betAmount < 0)
        {
            Debug.LogWarning($"MarbleGamblingProfile: Invalid bet input '{playerGoldBetInput}'. Expected a non-negative integer.");
            insufficientGoldWarningImage.gameObject.SetActive(true);
            return;
        }

        if (gamblingScreen == null)
        {
            Debug.LogWarning("MarbleGamblingProfile: GamblingScreen reference missing; cannot process bet.");
            insufficientGoldWarningImage.gameObject.SetActive(true);
            return;
        }

        var inventory = PlayerInventoryManager.Instance;
        if (inventory == null)
        {
            Debug.LogWarning("MarbleGamblingProfile: PlayerInventoryManager instance missing; cannot process bet.");
            insufficientGoldWarningImage.gameObject.SetActive(true);
            return;
        }

        if (inventory.goldAmount >= betAmount)
        {
            inventory.goldAmount -= betAmount;
            insufficientGoldWarningImage.gameObject.SetActive(false);

            BetAmountValue += betAmount;
            BetAmountText.text = BetAmountValue.ToString();

            uniqueBet = true;
        }
        else
        {
            insufficientGoldWarningImage.gameObject.SetActive(true);
        }

    }
}
