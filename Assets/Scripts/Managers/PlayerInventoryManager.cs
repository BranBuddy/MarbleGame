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

    private void Start()
    {
        // Validate after data has been loaded
        ValidateIfItemOwned();
    }

    private void ValidateIfItemOwned()
    {
        var shopItems = Resources.LoadAll<ShopItemsSO>("ShopItems");
        
        Debug.Log($"PlayerInventoryManager: ValidateIfItemOwned - Checking {shopItems.Length} shop items against {ownedItems.Count} owned items");
        Debug.Log($"PlayerInventoryManager: Owned items in inventory: {string.Join(", ", ownedItems)}");

        foreach (var item in shopItems)
        {
            bool isOwned = ownedItems.Contains(item.itemName);
            if (isOwned)
            {
                item.isPurchased = true;
                Debug.Log($"PlayerInventoryManager: Item '{item.itemName}' is marked as purchased.");
            }
            else
            {
                item.isPurchased = false;
                Debug.Log($"PlayerInventoryManager: Item '{item.itemName}' (type: {item.itemType}) is NOT owned. Checking inventory: {string.Join(", ", ownedItems)}");
            }
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
        
        // Validate after data has been loaded
        ValidateIfItemOwned();
    }

    /// <summary>
    /// Debug method: Unlocks all shop items and adds them to the player inventory.
    /// </summary>
    public void UnlockAllItems()
    {
        var shopItems = Resources.LoadAll<ShopItemsSO>("ShopItems");
        
        Debug.Log($"[DEBUG] Unlocking all {shopItems.Length} items!");
        
        foreach (var item in shopItems)
        {
            if (!ownedItems.Contains(item.itemName))
            {
                ownedItems.Add(item.itemName);
                item.isPurchased = true;
                Debug.Log($"[DEBUG] Unlocked: {item.itemName}");
            }
        }
        
        Debug.Log($"[DEBUG] All items unlocked! Total owned items: {ownedItems.Count}");
    }
}