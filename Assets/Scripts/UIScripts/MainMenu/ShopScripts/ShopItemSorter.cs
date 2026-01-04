using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ShopItemSorter : MonoBehaviour
{
    [SerializeField] private GameObject shopContent;
    private Dictionary<string, GameObject> shopItems = new Dictionary<string, GameObject>();
    private List<GameObject> originalOrder = new List<GameObject>(); // Store original order

    private void Start()
    {
        // Items may be loading dynamically, so wait a frame then gather
        StartCoroutine(GatherItemsDelayed());
    }

    private System.Collections.IEnumerator GatherItemsDelayed()
    {
        yield return null; // Wait one frame for items to populate
        GatherAllItems();
    }

    private void GatherAllItems()
    {
        shopItems.Clear();
        originalOrder.Clear();
        
        if (shopContent == null)
        {
            Debug.LogError("ShopItemSorter: shopContent is not assigned in the Inspector!");
            return;
        }
        
        foreach(Transform item in shopContent.transform)
        {
            shopItems[item.name] = item.gameObject;
            originalOrder.Add(item.gameObject); // Store in original order
            Debug.Log("ShopItemSorter: Added shop item - " + item.name);
        }
        
        Debug.Log($"ShopItemSorter: Total items gathered - {shopItems.Count}");
    }

    public void SortByAll()
    {
        // Reset all items to original order and state
        for (int i = 0; i < originalOrder.Count; i++)
        {
            originalOrder[i].transform.SetSiblingIndex(i);
            originalOrder[i].gameObject.SetActive(true);
        }
        Debug.Log("ShopItemSorter: Reset to original order");
    }

    private void ResetAllItems()
    {
        // Activate all items and reset sibling indices
        for (int i = 0; i < originalOrder.Count; i++)
        {
            originalOrder[i].gameObject.SetActive(true);
            originalOrder[i].transform.SetSiblingIndex(i);
        }
    }

    public void SortIfHat()
    {
        // Reset all items first to ensure proper state
        ResetAllItems();
        List<GameObject> visibleItems = new List<GameObject>();
        foreach(var item in originalOrder)
        {
            ShopItem shopItem = item.GetComponent<ShopItem>();
            if(shopItem != null && shopItem.shopItemData != null && shopItem.shopItemData.itemType == ShopItemsSO.ItemType.Hat)
            {
                visibleItems.Add(item);
            }
        }
        
        // Reorder visible items
        for (int i = 0; i < visibleItems.Count; i++)
        {
            visibleItems[i].transform.SetSiblingIndex(i);
            Debug.Log($"ShopItemSorter: Hat item '{visibleItems[i].name}' at index {i}");
        }
        
        // Hide non-hat items
        foreach(var item in originalOrder)
        {
            ShopItem shopItem = item.GetComponent<ShopItem>();
            if(shopItem == null || shopItem.shopItemData == null || shopItem.shopItemData.itemType != ShopItemsSO.ItemType.Hat)
            {
                item.gameObject.SetActive(false);
                Debug.Log($"ShopItemSorter: Hiding non-hat item '{item.name}'");
            }
        }
    }

    public void SortIfSale()
    {
        // Reset all items first to ensure proper state
        ResetAllItems();
        List<GameObject> visibleItems = new List<GameObject>();
        foreach(var item in originalOrder)
        {
            ShopItem shopItem = item.GetComponent<ShopItem>();
            if(shopItem != null && shopItem.shopItemData != null && shopItem.shopItemData.isOnSale)
            {
                visibleItems.Add(item);
            }
        }
        
        // Reorder visible items
        for (int i = 0; i < visibleItems.Count; i++)
        {
            visibleItems[i].transform.SetSiblingIndex(i);
            Debug.Log($"ShopItemSorter: Sale item '{visibleItems[i].name}' at index {i}");
        }
        
        // Hide non-sale items
        foreach(var item in originalOrder)
        {
            ShopItem shopItem = item.GetComponent<ShopItem>();
            if(shopItem == null || shopItem.shopItemData == null || !shopItem.shopItemData.isOnSale)
            {
                item.gameObject.SetActive(false);
                Debug.Log($"ShopItemSorter: Hiding non-sale item '{item.name}'");
            }
        }
    }

    public void SortIfFeatured()
    {
        // Reset all items first to ensure proper state
        ResetAllItems();
        List<GameObject> visibleItems = new List<GameObject>();
        foreach(var item in originalOrder)
        {
            ShopItem shopItem = item.GetComponent<ShopItem>();
            if(shopItem != null && shopItem.shopItemData != null && shopItem.shopItemData.isFeatured)
            {
                visibleItems.Add(item);
            }
        }
        
        // Reorder visible items
        for (int i = 0; i < visibleItems.Count; i++)
        {
            visibleItems[i].transform.SetSiblingIndex(i);
            Debug.Log($"ShopItemSorter: Featured item '{visibleItems[i].name}' at index {i}");
        }
        
        // Hide non-featured items
        foreach(var item in originalOrder)
        {
            ShopItem shopItem = item.GetComponent<ShopItem>();
            if(shopItem == null || shopItem.shopItemData == null || !shopItem.shopItemData.isFeatured)
            {
                item.gameObject.SetActive(false);
                Debug.Log($"ShopItemSorter: Hiding non-featured item '{item.name}'");
            }
        }
    }

    public void SortIfMarble()
    {
        // Reset all items first to ensure proper state
        ResetAllItems();
        List<GameObject> visibleItems = new List<GameObject>();
        foreach(var item in originalOrder)
        {
            ShopItem shopItem = item.GetComponent<ShopItem>();
            if(shopItem != null && shopItem.shopItemData != null && shopItem.shopItemData.itemType == ShopItemsSO.ItemType.Marble)
            {
                visibleItems.Add(item);
            }
        }
        
        // Reorder visible items
        for (int i = 0; i < visibleItems.Count; i++)
        {
            visibleItems[i].transform.SetSiblingIndex(i);
            Debug.Log($"ShopItemSorter: Marble item '{visibleItems[i].name}' at index {i}");
        }
        
        // Hide non-marble items
        foreach(var item in originalOrder)
        {
            ShopItem shopItem = item.GetComponent<ShopItem>();
            if(shopItem == null || shopItem.shopItemData == null || shopItem.shopItemData.itemType != ShopItemsSO.ItemType.Marble)
            {
                item.gameObject.SetActive(false);
                Debug.Log($"ShopItemSorter: Hiding non-marble item '{item.name}'");
            }
        }
    }
}
