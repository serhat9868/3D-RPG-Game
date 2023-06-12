using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName ="Targeting",menuName ="Ability/Targeting/Create New Targeting",order = 0)]
    public class DemoTargeting : TargetingStrategy
    {
        public override void StartTargeting(AbilityData data,Action finished)
        {
            Debug.Log("Demo targeting started ! ");
            
        }
    }
}