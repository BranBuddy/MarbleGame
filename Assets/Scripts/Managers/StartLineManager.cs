using System.Collections.Generic;
using UnityEngine;
using Singletons;

public class StartLineManager : Singleton<StartLineManager>
{
    public List<GameObject> poolOfMarbles;
    public List<GameObject> availableMarbles;
    public Transform[] startLinePositions;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        UpdateListOfAvailableMarbles();
    }

    public void UpdateListOfAvailableMarbles()
    {
        foreach (var marble in availableMarbles)
        {
            if(marble == null)
            {
                availableMarbles.Remove(marble);
                break;
            }
        }
    }

    internal void PlaceMarblesAtStartLine()
    {
        for (int i = 0; i < availableMarbles.Count && i < startLinePositions.Length; i++)
        {
            var entry = availableMarbles[i];
            if (entry == null) continue;

            // If it's still a prefab asset (not in a valid scene), instantiate and replace it
            if (!entry.scene.IsValid())
            {
                var inst = Instantiate(entry, startLinePositions[i].position, startLinePositions[i].rotation);
                availableMarbles[i] = inst;
            }
            else
            {
                entry.transform.SetPositionAndRotation(startLinePositions[i].position, startLinePositions[i].rotation);
                if (!entry.activeSelf) entry.SetActive(true);
            }
        }

        DisableStartLinePositions();

    }

    private void DisableStartLinePositions()
    {
        if(startLinePositions.Length > availableMarbles.Count)
        {
            foreach(var pos in startLinePositions)
            {
                if(System.Array.IndexOf(startLinePositions, pos) >= availableMarbles.Count)
                {
                    pos.gameObject.SetActive(false);
                }
            }
        } 
    }
}
