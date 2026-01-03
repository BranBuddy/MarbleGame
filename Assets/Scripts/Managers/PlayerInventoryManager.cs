using UnityEngine;
using Singletons;
using System.Collections.Generic;
using System;

public class PlayerInventoryManager : Singleton<PlayerInventoryManager>
{
    [SerializeField] internal float goldAmount;
    public List<String> ownedItems; // List of item IDs owned by the player

    internal void AddItemToInventory(ShopItemsSO item)
    {
        ownedItems.Add(item.itemName);
    }

    internal void DeductGold(int amount)
    {
        goldAmount -= amount;
    }
}
