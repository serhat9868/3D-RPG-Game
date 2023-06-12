using Game.Inventories;
using Game.Saving;
using RPG.Control;
using RPG.Inventories;
using RPG.Stats;
using RPG.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour,IRaycastable,ISaveable
    {
        [SerializeField] string shopName;

        [Range(0, 100)]
        [SerializeField] float sellingPercentage = 0;
        [SerializeField] float maxBarterDiscount = 80f;
        [SerializeField] StockItemConfig[] stockItemConfigs;
      
        Shopper currentShopper = null;
        bool isBuyingMode = true;

        ItemCategory filter = ItemCategory.None;
        [System.Serializable]
        public class StockItemConfig
        {
            public InventoryItem item;
            public int initialStock;
            [Range(0,100)]
            public float buyingDiscountPercentage;
            public int levelToUnlock = 0;
        }
        Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
        Dictionary<InventoryItem, int> stockSold = new Dictionary<InventoryItem, int>();

        public event Action onChange;
        public void SetShopper(Shopper shopper)
        {
            currentShopper = shopper;
        }
        public IEnumerable<ShopItem> GetFilteredItems()
        {
            foreach(ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();

                if (filter == ItemCategory.None || item.GetItemCategory() == filter)
                {
                    yield return shopItem;  
                }
            }
        }
        public IEnumerable<ShopItem> GetAllItems()
        {
            Dictionary<InventoryItem, float> prices = GetPrices();
            Dictionary<InventoryItem, int> Availabilities = GetAvailabilities();

            foreach (InventoryItem item in Availabilities.Keys)
            {
                if (Availabilities[item] <= 0) continue;

                float price = prices[item];    
                int quantityInTransaction = 0;
                transaction.TryGetValue(item, out quantityInTransaction);
                int availability = Availabilities[item];
                yield return new ShopItem(item, availability, price, quantityInTransaction);
            }
        }
        private Dictionary<InventoryItem, int> GetAvailabilities()
        {
            Dictionary<InventoryItem, int> availabilities = new Dictionary<InventoryItem, int>();

            foreach(var config in GetAvailableConfigs())
            {
                if (isBuyingMode)
                {
                    if (!availabilities.ContainsKey(config.item))
                    {
                        int sold = 0;
                        stockSold.TryGetValue(config.item, out sold);
                        availabilities[config.item] = -sold;
                    }
                    availabilities[config.item] += config.initialStock;
                }
                else
                {
                    availabilities[config.item] = CountItemsInInventory(config.item);
                }
                }
            return availabilities;
        }
        private Dictionary<InventoryItem, float> GetPrices()
        {
            Dictionary<InventoryItem, float> prices = new Dictionary<InventoryItem, float>();

            foreach(var config in stockItemConfigs)
            {
                if (isBuyingMode)
                {
                    if (!prices.ContainsKey(config.item))
                    {
                        prices[config.item] = config.item.GetPrice()*GetBarterDiscount();
                    }

                    prices[config.item] *= (1 - (config.buyingDiscountPercentage / 100));
                }
                else
                {
                    prices[config.item] = config.item.GetPrice() * (1 - (sellingPercentage / 100));
                }
                }
            return prices;
        }

      

        private IEnumerable<StockItemConfig> GetAvailableConfigs() 
        {
            int shopperLevel = GetShopperLevel();

            foreach(var config in stockItemConfigs)
            {
                if (shopperLevel > config.levelToUnlock) continue;

                yield return config;
            }
        }
        private int CountItemsInInventory(InventoryItem item)
        {
            int count = 0;
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            if (shopperInventory == null) return 0;
            for(int i = 0; i < shopperInventory.GetSize(); i++)
            {
               if(shopperInventory.GetItemInSlot(i) == item)
                {
                    count += shopperInventory.GetNumberInSlot(i);
                }
            }
            return count;
        }
        public void SelectFilter(ItemCategory category)
        {
            filter = category;
            if (onChange != null)
            {
                onChange();
            }
        }
        public ItemCategory GetFilter() 
        { 
            return filter; 
        }
        public void SelectMode(bool isBuying) 
        {
            isBuyingMode = isBuying;
            if(onChange != null)
            {
                onChange();
            }
        }
        public bool IsBuyingMode()
        {
            return isBuyingMode;
            
        }
        public bool CanTransact() 
        {
            if (isBuyingMode)
            {
                if (IsTransactionEmpty()) return false;
                if (!HasSufficientFunds()) return false;
                if (!HasInventorySpace()) return false;
                return true;
            }
            else
            {
                if (IsTransactionEmpty()) return false;
                return true;
            }
        }
        public bool HasSufficientFunds()
        {
            Purse purse = currentShopper.GetComponent<Purse>();
            if (purse == null) return false;
            return purse.GetBalance() >= TransactionTotal();
        }
        public bool IsTransactionEmpty()
        {
            return transaction.Count == 0;
        }
        public bool HasInventorySpace()
        {
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            if (shopperInventory == null) return false;
            List<InventoryItem> flatItems = new List<InventoryItem>();
            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityInTransaction();
                for (int i = 0; i < quantity; i++)
                {
                    flatItems.Add(item);
                }
            }
            return shopperInventory.HasSpaceFor(flatItems);
        }
        public void ConfirmTransaction()
        {
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            Purse shopperPurse = currentShopper.GetComponent<Purse>();
            if (shopperInventory == null || shopperPurse == null) return;
            // Transfer to or from the inventory
            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityInTransaction();
                float price = shopItem.GetPrice();
                for (int i = 0; i < quantity; i++)
                {
                    if (isBuyingMode)
                    {
                        BuyItem(shopperInventory, shopperPurse, item, quantity, price);
                    }
                    else
                    {
                        SellItem(shopperInventory, shopperPurse, item, quantity, price);
                    }
                }
            }
            // Removal from transaction
            // Debting or Crediting of funds
            if (onChange != null)
            {
                onChange();
            }
        }

        private void SellItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, int quantity, float price)
        {
            int slot = FindFirstItemSlot(shopperInventory, item);
            if (slot == -1) return;

            AddToTransaction(item, -1);
            shopperInventory.RemoveFromSlot(slot, 1);
            if (!stockSold.ContainsKey(item))
            {
                stockSold[item] = 0;
            }
            stockSold[item]--;
            shopperPurse.UpdateBalance(price);
        }

        private void BuyItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, int quantity, float price)
        {
                if (shopperPurse.GetBalance() < price) return;

                bool success = shopperInventory.AddToFirstEmptySlot(item, 1);
                if (success)
                {
                    AddToTransaction(item, -1);
                if (!stockSold.ContainsKey(item))
                {
                    stockSold[item] = 0;    
                }
                    stockSold[item]++;
                    shopperPurse.UpdateBalance(-price);
                }
        }
        private int FindFirstItemSlot(Inventory shopperInventory, InventoryItem item)
        {
            for (int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if (shopperInventory.GetItemInSlot(i) == item)
                {
                    return i;
                }
            }
            return 0;
        }
        public float TransactionTotal() 
        {
            float total = 0;
            foreach(ShopItem item in GetAllItems())
            {
                total += item.GetPrice() * item.GetQuantityInTransaction();
            }
            return total;
        }
        public void AddToTransaction(InventoryItem item,int quantity) 
        {
            if (!transaction.ContainsKey(item))
            {
                transaction[item] = 0;
            }

            var availabilities = GetAvailabilities();
            int availability = availabilities[item];

            if (transaction[item] + quantity > availability)
            {
                transaction[item] = availability;
            }
            else
            {
                transaction[item] += quantity;
            }

            if (transaction[item] <= 0)
            {
                transaction.Remove(item);
            }
            if (onChange != null)
            {
                onChange();
            }
        }
      
        public string GetShopName() { return shopName; }

        public CursorType GetCursorType()
        {
            return CursorType.Shop;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<Shopper>().SetActiveShop(this);  
            }
            return true;
        }
        private int GetShopperLevel()
        {
            BaseStats shopperStat = currentShopper.GetComponent<BaseStats>();
            if (shopperStat == null) return 0;

            return shopperStat.GetLevel();
        }
        public object CaptureState()
        {
            Dictionary<String, int> saveObject = new Dictionary<string, int>();
            foreach(var pair in stockSold)
            {
                saveObject[pair.Key.GetItemID()] = pair.Value;
            }

            return saveObject;
        }

        public void RestoreState(object state)
        {
            Dictionary<String, int> saveObject = new Dictionary<string, int>();
            stockSold.Clear();

            foreach(var pair in saveObject)
            {
                stockSold[InventoryItem.GetFromID(pair.Key)] = pair.Value;
            }
        }
        private float GetBarterDiscount()
        {
            BaseStats baseStats = currentShopper.GetComponent<BaseStats>();
            float discount = baseStats.GetStat(Stat.BuyingDiscountPercentage);
            return (1 - (Math.Min(discount, maxBarterDiscount) / 100));
        }

      
    }
}