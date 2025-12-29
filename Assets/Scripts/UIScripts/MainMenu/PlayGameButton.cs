using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayGameButton : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    public void OnPlayGameButtonPressed()
    {
        SceneManager.LoadScene(sceneToLoad);
        Debug.Log("Play Game button pressed. Loading scene: " + sceneToLoad);
    }

}
