
using UnityEngine;

public class StartLineManager : MonoBehaviour
{
    
    public Transform[] startLinePositions;

    /// <summary>
    /// Instantiates or positions available marbles at the start line and disables unused positions.
    /// </summary>
    internal void PlaceMarblesAtStartLine()
    {
        for (int i = 0; i < MarbleManager.Instance.availableMarbles.Count && i < startLinePositions.Length; i++)
        {
            var entry = MarbleManager.Instance.availableMarbles[i];
            if (entry == null) continue;

            // If it's still a prefab asset (not in a valid scene), instantiate and replace it
            if (!entry.scene.IsValid())
            {
                var inst = Instantiate(entry, startLinePositions[i].position, startLinePositions[i].rotation);
                MarbleManager.Instance.availableMarbles[i] = inst;
            }
            else
            {
                entry.transform.SetPositionAndRotation(startLinePositions[i].position, startLinePositions[i].rotation);
                if (!entry.activeSelf) entry.SetActive(true);
            }
        }

        if(MarbleManager.Instance.availableMarbles.Count < startLinePositions.Length)
            DisableStartLinePositions();

    }

    /// <summary>
    /// Deactivates start line positions that exceed the available marbles count.
    /// </summary>
    private void DisableStartLinePositions()
    {
        foreach(var pos in startLinePositions)
        {
            if(System.Array.IndexOf(startLinePositions, pos) >= MarbleManager.Instance.availableMarbles.Count)
            {
                pos.gameObject.SetActive(false);
            }
        } 
    }

}
