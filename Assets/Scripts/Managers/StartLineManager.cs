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
        for (int i = 0; i < poolOfMarbles.Count && i < startLinePositions.Length; i++)
        {
            GameObject marble = Instantiate(poolOfMarbles[i], startLinePositions[i].position, startLinePositions[i].rotation);
            availableMarbles.Add(marble);
        }
    }
}
