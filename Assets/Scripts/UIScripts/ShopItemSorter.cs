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
        // Show all items first
        foreach(var item in shopItems.Values)
        {
            item.gameObject.SetActive(true);
        }
        
        // Restore to original order
        for (int i = 0; i < originalOrder.Count; i++)
        {
            originalOrder[i].transform.SetSiblingIndex(i);
        }
        Debug.Log("ShopItemSorter: Reset to original order");
    }

    public void SortIfHat()
    {
        // Show all items first
        foreach(var item in shopItems.Values)
        {
            item.gameObject.SetActive(true);
        }
        
        foreach(var item in shopItems)
        {
            if(item.Value.GetComponent<ShopItem>().shopItemData.itemType == ShopItemsSO.ItemType.Hat)
            {
                item.Value.transform.SetAsFirstSibling();
                Debug.Log("ShopItemSorter: Moved hat item to top - " + item.Key);
            } 
            else 
            {
                item.Value.transform.SetAsLastSibling();
                item.Value.gameObject.SetActive(false);
                Debug.Log("ShopItemSorter: Moved non-hat item to bottom and deactivated - " + item.Key);
            }
        }
    }

    public void SortIfMarble()
    {
        // Show all items first
        foreach(var item in shopItems.Values)
        {
            item.gameObject.SetActive(true);
        }
        
        foreach(var item in shopItems)
        {
            if(item.Value.GetComponent<ShopItem>().shopItemData.itemType == ShopItemsSO.ItemType.Marble)
            {
                item.Value.transform.SetAsFirstSibling();
                Debug.Log("ShopItemSorter: Moved marble item to top - " + item.Key);
            } 
            else 
            {
                item.Value.transform.SetAsLastSibling();
                item.Value.gameObject.SetActive(false);
                Debug.Log("ShopItemSorter: Moved non-marble item to bottom and deactivated - " + item.Key);
            }
        }
    }
}
