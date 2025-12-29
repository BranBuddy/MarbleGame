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
    [SerializeField] private int currentCameraIndex = 0;
    [SerializeField] private Button leftSwapButton;
    [SerializeField] private Button rightSwapButton;
    [SerializeField] private TMP_Text nowViewingText;

    public void InitializeCameraList()
    {
        cameras = new List<CinemachineCamera>();

        var foundHere = GetComponentsInChildren<CinemachineCamera>(true);
        if (foundHere != null && foundHere.Length > 0)
        {
            cameras.AddRange(foundHere);
        }

        foreach (var marble in StartLineManager.Instance.availableMarbles)
        {
            var cam = marble.GetComponentInChildren<CinemachineCamera>(true);
            if (cam != null && !cameras.Contains(cam))
            {
                cameras.Add(cam);
            }
        }

        if (cameras.Count == 0)
        {
            Debug.LogWarning("CameraSwapper.InitializeCameraList: No CinemachineCamera found. Ensure cameras exist and are not of a different Cinemachine version.");
            return;
        }

        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].Priority = 0;
        }

        currentCameraIndex = 0;
        cameras[currentCameraIndex].Priority = 10; // Set initial camera priority
        if (StartLineManager.Instance.availableMarbles != null && StartLineManager.Instance.availableMarbles.Count > 0)
        {
            nowViewingText.text = "Now Viewing: " + CleanName(StartLineManager.Instance.availableMarbles[0].name);
        }
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
