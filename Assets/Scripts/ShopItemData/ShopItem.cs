using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

public class ShopItem : MonoBehaviour, IShopItems
{
    public ShopItemsSO shopItemData { get; set; }
    public bool isPurchased { get; set; }
    public int price { get; set; }
    public string itemName { get; set; }

    [SerializeField] private ShopItemsSO shopItemsSO;
    [SerializeField] private GameObject alreadyPurchasedIndicator;
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemPriceText;
    private TMP_Text goldText;

    private void Awake()
    {
        goldText = transform.parent?.parent?.parent?.parent?.GetComponentInChildren<TMP_Text>();

        if (goldText == null)
        {
            Debug.LogWarning("ShopItem: Could not find TMP_Text in great great grandparent.");
        }
        else
        {
            UpdateGoldText();
        }
        
        InitializeShopItem();
    }

    private void UpdateGoldText()
    {
        if (goldText != null)
        {
            goldText.text = "Gold Amount: " + PlayerInventoryManager.Instance.goldAmount.ToString();
        }
    }

    private void InitializeShopItem()
    {
        shopItemData = shopItemsSO;
        isPurchased = shopItemData.isPurchased;
        price = shopItemData.itemPrice;
        itemName = shopItemData.itemName;
        WriteShopItemUI();
    }

    private void WriteShopItemUI()
    {
        if (itemIconImage != null)
            itemIconImage.sprite = shopItemData.itemIcon;

        if (itemNameText != null)
            itemNameText.text = shopItemData.itemName;

        if (itemPriceText != null)
            itemPriceText.text = isPurchased ? "Purchased" : shopItemData.itemPrice.ToString();

        if(alreadyPurchasedIndicator != null)
            alreadyPurchasedIndicator.SetActive(isPurchased);
    }

    public void PurchaseItem()
    {
        if (!isPurchased && PlayerInventoryManager.Instance.goldAmount >= price)
        {
            isPurchased = true;
            shopItemData.isPurchased = true;
            UpdatePurchaseIndicator();
            PlayerInventoryManager.Instance.AddItemToInventory(shopItemData);
            PlayerInventoryManager.Instance.DeductGold(price);
            UpdateGoldText();
        } 
        else
        {
            Debug.LogWarning("ShopItem: Not enough gold to purchase item or item already purchased.");
        }
    }


    private void UpdatePurchaseIndicator()
    {
        if (alreadyPurchasedIndicator != null)
        {
            alreadyPurchasedIndicator.SetActive(isPurchased);
            itemPriceText.text = "Purchased";
        }
    }
}
