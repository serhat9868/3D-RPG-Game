using Game.Inventories;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Abilities
{

    public class CooldownStore : MonoBehaviour
    {
        Dictionary<InventoryItem, float> cooldownTimers = new Dictionary<InventoryItem, float>();
        Dictionary<InventoryItem, float> firstCooldownTimers = new Dictionary<InventoryItem, float>();

        private void Update()
        {
            var key = new List<InventoryItem>(cooldownTimers.Keys);
            foreach(InventoryItem ability in key)
            {
                cooldownTimers[ability] -= Time.deltaTime;

                if(cooldownTimers[ability] < 0)
                {
                    cooldownTimers.Remove(ability);
                    firstCooldownTimers.Remove(ability);
                }
            }
        }
        public void StartCooldown(InventoryItem ability,float cooldownTime)
        {
            cooldownTimers[ability] = cooldownTime;
            firstCooldownTimers[ability] = cooldownTime;
        }
        public float GetTimeRemaining(InventoryItem ability)
        {
            if (!cooldownTimers.ContainsKey(ability))
            {
                return 0;
            }
            
            return cooldownTimers[ability];
        }

        public float GetFractionRemaining(InventoryItem ability)
        {
            if(ability == null)
            {
                return 0;
            }

            if (!cooldownTimers.ContainsKey(ability))
            {
                return 0;
            }

            return cooldownTimers[ability] / firstCooldownTimers[ability];
        }
    }

}