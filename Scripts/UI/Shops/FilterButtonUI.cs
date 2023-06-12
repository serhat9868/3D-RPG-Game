using Game.Inventories;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Shops.UI
{
    public class FilterButtonUI : MonoBehaviour
    {
        [SerializeField] ItemCategory itemCategory = ItemCategory.None;
        Button button;
        Shop currentShop;
        
        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(SelectFilter);
        }
        public void SetShop(Shop currentShop)
        {
            this.currentShop = currentShop;
        }
        public void RefreshUI()
        {
            button.interactable = currentShop.GetFilter() != itemCategory;
        }
        private void SelectFilter()
        {
            currentShop.SelectFilter(itemCategory);
        }
    }
}