using System.Collections.Generic;
using UnityEngine;

public class StartLineManager : MonoBehaviour
{
    public List<GameObject> availableMarbles;
    public Transform[] startLinePositions;

    private void Start()
    {
        PlaceMarblesAtStartLine();
    }

    public void PlaceMarblesAtStartLine()
    {
        for (int i = 0; i < availableMarbles.Count && i < startLinePositions.Length; i++)
        {
            GameObject marble = Instantiate(availableMarbles[i], startLinePositions[i].position, startLinePositions[i].rotation);
        }
    }
}
