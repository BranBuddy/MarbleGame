
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int playerGold;
    public List<GameObject> poolOfMarbles;

    public GameData()
    {
        playerGold = 0; // Default starting gold
        poolOfMarbles = new List<GameObject>();
    }
}
