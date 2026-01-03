using UnityEngine;
using Singletons;
using System.Collections.Generic;
using System;

public class PlayerInventoryManager : Singleton<PlayerInventoryManager>, IDataPeristenceManager
{
    [SerializeField] internal float goldAmount;
    public List<String> ownedItems; // List of item IDs owned by the player

    protected override void Awake()
    {
        base.Awake();
        
        // Initialize ownedItems if null
        if (ownedItems == null)
        {
            ownedItems = new List<string>();
        }
    }

    internal void AddItemToInventory(ShopItemsSO item)
    {
        if (ownedItems == null)
        {
            ownedItems = new List<string>();
        }
        
        Debug.Log($"PlayerInventoryManager: Adding '{item.itemName}' to inventory. Current owned items count: {ownedItems.Count}");
        ownedItems.Add(item.itemName);
        Debug.Log($"PlayerInventoryManager: After adding, owned items count: {ownedItems.Count}. Items: {string.Join(", ", ownedItems)}");
    }

    internal void DeductGold(int amount)
    {
        Debug.Log($"PlayerInventoryManager: Deducting {amount} gold. Current: {goldAmount}");
        goldAmount -= amount;
        Debug.Log($"PlayerInventoryManager: After deduction, gold: {goldAmount}");
        
        // Save after transaction is complete
        DataPersistenceManager.Instance.SaveGame();
    }

    public void SaveData(GameData data)
    {
        data.playerGold = Mathf.FloorToInt(goldAmount);
        
        // Create a new list copy to avoid reference issues
        if (ownedItems != null)
        {
            data.ownedItems = new List<string>(ownedItems);
        }
        else
        {
            data.ownedItems = new List<string>();
        }
        
        Debug.Log($"PlayerInventoryManager: SaveData called. Saving gold: {data.playerGold}, owned items count: {data.ownedItems.Count}");
    }

    public void LoadData(GameData data)
    {
        goldAmount = data.playerGold;
        
        // Create a new list copy to avoid reference issues
        if (data.ownedItems != null)
        {
            ownedItems = new List<string>(data.ownedItems);
        }
        else
        {
            ownedItems = new List<string>();
        }
        
        Debug.Log($"PlayerInventoryManager: LoadData called. Loaded gold: {goldAmount}, owned items count: {ownedItems.Count}");
    }
}