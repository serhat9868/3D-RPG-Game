using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Inventories
{
    [CreateAssetMenu(fileName = "Abilitiy", menuName = "Ability/Create New Ability", order = 0)]
    public class AbilitySO : ScriptableObject
    {
        [SerializeField] InventoryItem item;
        [SerializeField] ActionItem actionItem;

        }
}