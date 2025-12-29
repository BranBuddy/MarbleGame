using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class WinCondition : MonoBehaviour
{
    private GameObject currentWinner;
    [SerializeField] private TMP_Text winnerText;

    private void CleanName(GameObject obj)
    {
        string cleanName = obj.name;
        if (cleanName.Contains("(Clone)"))
        {
            cleanName = cleanName.Replace("(Clone)", "").Trim();
        }
        obj.name = cleanName;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Marble"))
        {
            if(currentWinner == null)
            {
                currentWinner = other.gameObject;
                CleanName(currentWinner);
                winnerText.gameObject.SetActive(true);
                winnerText.text = $"Winner is: {currentWinner.name}";
            }
        }
    }
}
