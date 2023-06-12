using Game.Inventories;
using RPG.Attributes;
using RPG.Core;
using UnityEngine;
namespace RPG.Abilities
{

    [CreateAssetMenu(fileName = "Abilitiy", menuName = "Ability/Create New Ability", order = 0)]
    public class Ability : ActionItem
    {
        [SerializeField] TargetingStrategy targetingStrategy;
        [SerializeField] FilteringStrategy[] filteringStrategies;
        [SerializeField] EffectStrategy[] effectStrategies;
        [SerializeField] float cooldownTime;
        [SerializeField] float manaRequired = 100f;

        public override void Use(GameObject user)
        {
            Mana mana = user.GetComponent<Mana>();
            if (manaRequired > mana.GetMana())
            {
                return;
            }
            CooldownStore cooldownStore = user.GetComponent<CooldownStore>();
            if (cooldownStore.GetTimeRemaining(this) > 0)
            {
                return;
            }
            AbilityData data = new AbilityData(user);

          ActionScheduler actionScheduler = user.GetComponent<ActionScheduler>();
            actionScheduler.StartAction(data);
            targetingStrategy.StartTargeting(data,
                () => {
                    TargetAquired(data);
                });
        }
        private void TargetAquired(AbilityData data)
        {
            if (data.IsCancelled()) return;
            Mana mana = data.GetUser().GetComponent<Mana>();
            if (!mana.UseMana(manaRequired)) return;

            CooldownStore cooldownStore = data.GetUser().GetComponent<CooldownStore>();
            cooldownStore.StartCooldown(this, cooldownTime);

            foreach (var filterStrategy in filteringStrategies)
            {
                data.SetTargets(filterStrategy.Filter(data.GetTargets()));
            }
            
            foreach (var effect in effectStrategies)
            {
                effect.StartEffect(data, EffectFinished);
            }
        }
        private void EffectFinished()
        {

        }
        public float GetCooldownTime()
        {
            return cooldownTime;
        }
        public Ability GetAbility()
        {
            return this;
        }
    }
}

