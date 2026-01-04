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
    [SerializeField] private GameObject alreadyPurchasedIndicator;
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemPriceText;
    [SerializeField] private GameObject featuredBanner;
    [SerializeField] private TMP_Text saleDiscountText;
    [SerializeField] private TMP_Text originalPriceText;
    private TMP_Text goldText;

    private void Awake()
    {
        goldText = transform.parent?.parent?.parent?.parent?.GetComponentInChildren<TMP_Text>();

        if (goldText == null)
        {
            Debug.LogWarning("ShopItem: Could not find TMP_Text in great great grandparent.");
        }
    }

    private void Start()
    {
        // Initialize UI after shopItemData has been set
        if (shopItemData != null)
        {
            UpdateGoldText();
            SetupItemUI();
            DeactivateButton();
        }
        else
        {
            Debug.LogError("ShopItem: shopItemData is null. Make sure it's assigned before Start().");
        }
    }

    private void DeactivateButton()
    {
        Button itemButton = GetComponentInChildren<Button>();
        if (itemButton != null && isPurchased)
        {
            itemButton.interactable = false;
        }
    }

    private void SetupItemUI()
    {
        // Sync purchased state from shopItemData
        isPurchased = shopItemData.isPurchased;
        
        // Set item icon
        if (itemIconImage != null && shopItemData.itemIcon != null)
        {
            itemIconImage.sprite = shopItemData.itemIcon;
        }

        // Set item name
        if (itemNameText != null)
        {
            itemNameText.text = shopItemData.itemName;
        }

        IfItemIsFeatured();
        UpdatePurchaseIndicator();
        IfItemIsOnSale();
    }

    private void UpdateGoldText()
    {
        if (goldText != null)
        {
            goldText.text = "Gold Amount: " + PlayerInventoryManager.Instance.goldAmount.ToString();
        }
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

    private void IfItemIsOnSale()
    {
        if (shopItemData == null) return;
        
        if (shopItemData.isOnSale && !isPurchased)
        {
            float discountAmount = (shopItemData.saleDiscountPercentage / 100f) * shopItemData.itemPrice;
            int discountedPrice = Mathf.RoundToInt(shopItemData.itemPrice - discountAmount);

            originalPriceText.fontStyle = FontStyles.Strikethrough;
            var originalPriceLabel = "Original Price: ";

            saleDiscountText.text = discountedPrice.ToString() + " Gold";
            originalPriceText.text = $"{originalPriceLabel}{shopItemData.itemPrice.ToString()}";
            originalPriceText.gameObject.SetActive(true);
            saleDiscountText.gameObject.SetActive(true);
            itemPriceText.gameObject.SetActive(false);
        }
        else
        {
            originalPriceText.gameObject.SetActive(false);
            saleDiscountText.gameObject.SetActive(false);
        }
    }

    private void IfItemIsFeatured()
    {
        if (shopItemData == null) return;
        
        if (featuredBanner != null)
        {
            featuredBanner.SetActive(shopItemData.isFeatured);
        }
        else
        {
            Debug.LogWarning("ShopItem: Featured banner GameObject is not assigned.");
        }
    }

    private void UpdatePurchaseIndicator()
    {
        if (shopItemData == null) return;
        
        if (shopItemData.isPurchased || isPurchased)
        {
            if (alreadyPurchasedIndicator != null)
            {
                alreadyPurchasedIndicator.SetActive(true);
            }
            
            if (itemPriceText != null)
            {
                itemPriceText.text = "Purchased";
                itemPriceText.gameObject.SetActive(true);
            }
        }
        else
        {
            if (alreadyPurchasedIndicator != null)
            {
                alreadyPurchasedIndicator.SetActive(false);
            }
            
            if (itemPriceText != null && !shopItemData.isOnSale)
            {
                itemPriceText.text = shopItemData.itemPrice.ToString();
            }
        }
    }
}
