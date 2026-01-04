using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
public class ShopScrollView : MonoBehaviour
{
    private readonly Dictionary<ShopItemsSO, bool> shopItems = new Dictionary<ShopItemsSO, bool>();
    [SerializeField] private Transform scrollViewContent;
    [SerializeField] private GameObject shopItemPrefab;

    void Start()
    {
        InitializeShopItems();
    }

    private void InitializeShopItems()
    {
        var prefabs = Resources.LoadAll<ShopItemsSO>("ShopItems");

        foreach(var prefab in prefabs)
        {
            shopItems.Add(prefab, UnlockShopItem(prefab));         
        }

        PopulateShopScrollView();
    }

    private void PopulateShopScrollView()
    {
        Debug.Log($"ShopScrollView: Player inventory ownedItems count: {PlayerInventoryManager.Instance?.ownedItems?.Count ?? 0}");
        if (PlayerInventoryManager.Instance?.ownedItems != null)
        {
            Debug.Log($"ShopScrollView: Owned items: {string.Join(", ", PlayerInventoryManager.Instance.ownedItems)}");
        }

        foreach(var item in shopItems)
        {
            GameObject itemInstance = Instantiate(shopItemPrefab, scrollViewContent);
            ShopItem shopItemComponent = itemInstance.GetComponent<ShopItem>();
            
            if (shopItemComponent != null)
            {
                shopItemComponent.shopItemData = item.Key;
                // Check if item is owned by checking player inventory, not just the ScriptableObject flag
                bool isOwned = PlayerInventoryManager.Instance != null && 
                               PlayerInventoryManager.Instance.ownedItems != null &&
                               PlayerInventoryManager.Instance.ownedItems.Contains(item.Key.itemName);
                
                Debug.Log($"ShopScrollView: Item '{item.Key.itemName}' - isOwned: {isOwned}");
                
                shopItemComponent.isPurchased = isOwned;
                shopItemComponent.price = item.Key.itemPrice;
                shopItemComponent.itemName = item.Key.itemName;
            }
        }
    }

    private bool UnlockShopItem(ShopItemsSO item)
    {
        if(item == null) return true;
        
        return item.isPurchased;
    }
}
