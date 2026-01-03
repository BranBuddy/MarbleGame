using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
public class ShopScrollView : MonoBehaviour
{
    private readonly Dictionary<GameObject, bool> shopItems = new Dictionary<GameObject, bool>();
    [SerializeField] private Transform scrollViewContent;


    void Start()
    {
        InitializeShopItems();
    }

    private void InitializeShopItems()
    {
        var prefabs = Resources.LoadAll<GameObject>("PrefabsToLoad/ShopItems");

        foreach(var prefab in prefabs)
        {
            shopItems.Add(prefab, UnlockShopItem(prefab));         
        }

        PopulateShopScrollView();
    }

    private void PopulateShopScrollView()
    {
        foreach(var item in shopItems)
        {
            GameObject itemInstance = Instantiate(item.Key, scrollViewContent);
        }
    }

    private bool UnlockShopItem(GameObject item)
    {
        if(item == null) return true;

        var behaviours = item.GetComponents<MonoBehaviour>();
        foreach (var behaviour in behaviours)
        {
            if(behaviour == null) continue;
            var shopItemSOField = behaviour.GetType().GetField("shopItemDataSO", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if(shopItemSOField != null && typeof(ShopItemsSO).IsAssignableFrom(shopItemSOField.FieldType))
            {
                var so = shopItemSOField.GetValue(behaviour) as ShopItemsSO;
                if(so != null)
                {
                    return so.isPurchased;
                }
            }
        }
        return false;
    }
}
