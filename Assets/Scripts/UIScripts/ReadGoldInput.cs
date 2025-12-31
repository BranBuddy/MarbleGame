using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class ReadGoldInput : MonoBehaviour
{
    private string playerGoldBetInput;
    private GamblingScreen gamblingScreen;

    [SerializeField] private Image insufficientGoldWarningImage;
    [SerializeField] private TMP_Text BetAmountText;
    internal int BetAmountValue;

    [SerializeField] internal bool uniqueBet = false;

    

    private void Start()
    {
        GameObject grandparentGameObject = transform.parent?.parent?.gameObject;

        if (grandparentGameObject != null)
        {
            gamblingScreen = grandparentGameObject.GetComponent<GamblingScreen>();
        }
        else
        {
            Debug.LogError("ReadGoldInput: Could not find grandparent GameObject.");
        }
    }


    public void ReadGoldBetInput(string input)
    {
        playerGoldBetInput = input;
        Debug.Log("Player Gold Bet Input: " + playerGoldBetInput);

        if (!int.TryParse(playerGoldBetInput, out var betAmount) || betAmount < 0)
        {
            Debug.LogWarning($"ReadGoldInput: Invalid bet input '{playerGoldBetInput}'. Expected a non-negative integer.");
            insufficientGoldWarningImage.gameObject.SetActive(true);
            return;
        }

        if (gamblingScreen == null)
        {
            Debug.LogWarning("ReadGoldInput: GamblingScreen reference missing; cannot process bet.");
            insufficientGoldWarningImage.gameObject.SetActive(true);
            return;
        }

        if (gamblingScreen.goldAmount >= betAmount)
        {
            gamblingScreen.goldAmount -= betAmount;
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
