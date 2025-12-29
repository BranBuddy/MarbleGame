using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void OnQuitButtonPressed()
    {
        Application.Quit();
        Debug.Log("Quit button pressed. Application is closing.");
    }
}
