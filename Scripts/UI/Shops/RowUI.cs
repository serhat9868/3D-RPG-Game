using Game.Inventories;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Shops.UI
{
    public class RowUI : MonoBehaviour
    {
        [SerializeField] Image iconField;
        [SerializeField] TextMeshProUGUI nameField;
        [SerializeField] TextMeshProUGUI price;
        [SerializeField] TextMeshProUGUI availability;
        [SerializeField] TextMeshProUGUI quantity;
        Shop currentShop = null;
        ShopItem item = null;
        public void Setup(Shop currentShop,ShopItem shopItem)
        {
            this.currentShop = currentShop;
            this.item = shopItem;

            iconField.sprite = shopItem.GetIcon();
            nameField.text = shopItem.GetName();
            price.text = $"{shopItem.GetPrice():N2}";
            availability.text = shopItem.GetAvailability().ToString();
            quantity.text = shopItem.GetQuantityInTransaction().ToString();
            
        }
        public void Add()
        {         
           currentShop.AddToTransaction(item.GetInventoryItem(),+1);
        }
        public void Remove()
        {
            currentShop.AddToTransaction(item.GetInventoryItem(), -1);
        }
    }
}