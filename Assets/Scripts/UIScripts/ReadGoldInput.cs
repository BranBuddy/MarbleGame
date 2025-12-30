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

        if(gamblingScreen.goldAmount >= int.Parse(playerGoldBetInput))
        {
            gamblingScreen.goldAmount -= int.Parse(playerGoldBetInput);
            insufficientGoldWarningImage.gameObject.SetActive(false);

            var betAmountToInt = int.Parse(playerGoldBetInput);
            BetAmountValue += betAmountToInt;
            BetAmountText.text = BetAmountValue.ToString();

            uniqueBet = true;
        } 
        else
        {
            insufficientGoldWarningImage.gameObject.SetActive(true);
        }

    }
}
