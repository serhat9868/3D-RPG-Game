using Game.Saving;
using System;
using UnityEngine;
namespace RPG.Inventories
{
    public class Purse : MonoBehaviour,ISaveable
    {
        [SerializeField] float startingBalance;
        float balance = 0;
        public event Action onchange;
        private void Awake()
        {
            balance = startingBalance;
        }
        public float GetBalance()
        {
            return balance;
        }

        public void UpdateBalance(float amount)
        {
            balance += amount;
            if(onchange != null) 
            {
                onchange();
            }
          
        }
        public object CaptureState()
        {
            return balance;
        }

        public void RestoreState(object state)
        {
            balance = (float)state;
        }
    }
}