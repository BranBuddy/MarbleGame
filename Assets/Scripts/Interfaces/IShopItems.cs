using UnityEngine;

public interface IShopItems
{
    public ShopItemsSO shopItemData {get; set;}
    public bool isPurchased {get; set;}
    public int price {get; set;}
    public string itemName {get; set;}

    

}
