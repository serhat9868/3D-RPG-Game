using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Animation Effect", menuName = "Ability/Effects/New Animation Effect", order = 0)]
    public class TriggerAnimationEffect : EffectStrategy
    {
        [SerializeField] string AnimationTriggerName;
        public override void StartEffect(AbilityData data, Action finished)
        {
            data.GetUser().transform.LookAt(data.GetTargetedPoint());
                Animator animator = data.GetUser().GetComponent<Animator>();
            animator.SetTrigger(AnimationTriggerName);
            finished(); 
        }
       
    }
}
