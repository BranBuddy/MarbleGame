using UnityEngine.UI;
using UnityEngine;

[CreateAssetMenu(fileName = "New ShopItem", menuName = "Shop/ShopItem", order = 1)]
public class ShopItemsSO : ScriptableObject
{
    public enum ItemType
    {
        Marble,
        Hat,
    }

    [Header("Shop Item Properties")]
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public int itemPrice;
    public bool isPurchased;
    public bool isFeatured;
    public bool isOnSale;
    
    [Range(0f, 100f)]
    [Tooltip("Discount percentage (0-100)")]
    [HideInInspector]
    public int saleDiscountPercentage;
}
