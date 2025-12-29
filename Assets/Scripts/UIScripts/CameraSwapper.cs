using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CameraSwapper : MonoBehaviour
{
    [SerializeField] private List<CinemachineCamera> cameras;
    private int currentCameraIndex = 0;
    [SerializeField] private Button leftSwapButton;
    [SerializeField] private Button rightSwapButton;
    [SerializeField] private TMP_Text nowViewingText;

    public void InitializeCameraList()
    {
        cameras = new List<CinemachineCamera>(GetComponentsInChildren<CinemachineCamera>());

        foreach(var marbles in StartLineManager.Instance.availableMarbles)
        {
            var cam = marbles.GetComponentInChildren<CinemachineCamera>();
            if(cam != null && !cameras.Contains(cam))
            {
                cameras.Add(cam);
                cam.Priority = 0;
            }
        }

        cameras[0].Priority = 10; // Set initial camera priority
        nowViewingText.text = "Now Viewing: " + CleanName(StartLineManager.Instance.availableMarbles[0].name);
    }

    // Remove common Unity cloning suffixes for display
    private string CleanName(string name)
    {
        if (string.IsNullOrEmpty(name)) return "";
        return name.Replace(" (Clone)", "").Replace("(Clone)", "").Trim();
    }

    public void LeftSwapCamera(int index)
    {
        index = currentCameraIndex - 1;
        if(index < 0)
        {
            index = cameras.Count - 1;
        }
        cameras[currentCameraIndex].Priority = 0;
        cameras[index].Priority = 10;
        currentCameraIndex = index;

        nowViewingText.text = "Now Viewing: " + CleanName(StartLineManager.Instance.availableMarbles[currentCameraIndex].name);
    }

    public void RightSwapCamera(int index)
    {
        index = currentCameraIndex + 1;
        if (index >= cameras.Count)
        {
            index = 0;
        }
        cameras[currentCameraIndex].Priority = 0;
        cameras[index].Priority = 10;
        currentCameraIndex = index;

        nowViewingText.text = "Now Viewing: " + CleanName(StartLineManager.Instance.availableMarbles[currentCameraIndex].name);
    }
}
