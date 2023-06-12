using Game.Inventories;
using RPG.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Shops.UI
{
    public class ShopUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI shopName;
        [SerializeField] Transform listRoot;
        [SerializeField] RowUI rowPrefab;
        [SerializeField] TextMeshProUGUI totalText;
        [SerializeField] Button confirmButton;
        [SerializeField] Button switchSellingButton;
        Color originalTextColor;
        Shopper shopper = null;
        Shop currentShop = null;

        private void Start()
        {
            originalTextColor = totalText.color;
            shopper = GameObject.FindWithTag("Player").GetComponent<Shopper>();
            if (shopper == null) return;    

            shopper.activeShopChange += ShopChanged;
            confirmButton.onClick.AddListener(ConfirmTransaction);
            switchSellingButton.onClick.AddListener(SwitchMode);
            ShopChanged();
        }
        public void ShopChanged()
        {
            if(currentShop != null)
            {
                currentShop.onChange -= RefreshUI;
            }
            currentShop = shopper.GetActiveShop();
            gameObject.SetActive(currentShop != null);

            foreach(FilterButtonUI buttons in GetComponentsInChildren<FilterButtonUI>())
            {
                buttons.SetShop(currentShop);
            }

            if (currentShop == null) return;
            shopName.text = currentShop.GetShopName();

            currentShop.onChange += RefreshUI;
            RefreshUI();
        }
        public void RefreshUI()
        {
         foreach(Transform child in listRoot)
            {
                Destroy(child.gameObject);
            }

         foreach(ShopItem shopItem in currentShop.GetFilteredItems())
            {
               RowUI rowUI = Instantiate<RowUI>(rowPrefab,listRoot);
                rowUI.Setup(currentShop,shopItem);
            }
         
            totalText.text = $"Total: ${currentShop.TransactionTotal():N2}";
            totalText.color = currentShop.HasSufficientFunds() ? originalTextColor : Color.red;
            confirmButton.interactable = currentShop.CanTransact();
                
            TextMeshProUGUI buyingText = confirmButton.GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI switchText = switchSellingButton.GetComponentInChildren<TextMeshProUGUI>();

            if (currentShop.IsBuyingMode())
            {
                switchText.text = "Switch to Selling";
                buyingText.text = "Buy";
            }
            else
            {
                switchText.text = "Switch To Buying";
                buyingText.text = "Sell";
            }
            foreach(FilterButtonUI filterButton in GetComponentsInChildren<FilterButtonUI>())
            {
                filterButton.RefreshUI();
            }

            }
        public void Close()
        {
            shopper.SetActiveShop(null);
        }
   public void ConfirmTransaction()
        {
            currentShop.ConfirmTransaction();
        }
        private void SwitchMode()
        {
            currentShop.SelectMode(!currentShop.IsBuyingMode());
        }
    }
}