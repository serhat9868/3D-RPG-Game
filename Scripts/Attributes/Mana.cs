using Game.Saving;
using Game.Utils;
using RPG.Stats;
using UnityEngine;
namespace RPG.Attributes
{
    public class Mana : MonoBehaviour,ISaveable
    {
        [SerializeField] float maxMana = 200f;
        [SerializeField] float regenerateRatio = 0.1f;
        [SerializeField] float regenerateRatio2 = 0.15f;
        [SerializeField] float regenerateRatio3 = 0.25f;
       LazyValue<float> mana;
        float second;

        private void Awake()
        {
            mana = new LazyValue<float>(GetInitialMana);
        }
        private float GetInitialMana()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Mana);
        }
        private void Update()
        {
            if (mana.value == GetMaxMana())
            {
                second = 0;
            }
            second += Time.deltaTime;
            if (second <= 3)
            {
                mana.value += Time.deltaTime * 60 * regenerateRatio;
                if (mana.value > GetMaxMana())
                {
                    mana.value = GetMaxMana();
                }
            }
            if (second > 3)
            {
                if (second < 6)
                {
                    mana.value += Time.deltaTime * 60 * regenerateRatio2;
                    if (mana.value > GetMaxMana())
                    {
                        mana.value = GetMaxMana();
                    }
                }
                if (second > 10)
                {
                    mana.value += Time.deltaTime * 60 * regenerateRatio3;
                    if (mana.value > GetMaxMana())
                    {
                        mana.value = GetMaxMana();
                    }
                }

            }
        }
        public float GetMana()
        {
            return mana.value;
        }

        public float GetMaxMana()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Mana);
        }
        
        public bool UseMana(float manaToUse)
        {
            if(manaToUse > mana.value)
            {
                return false;
            }
            mana.value -= manaToUse;
            return true;
        }

        public object CaptureState()
        {
            return mana.value;
        }

        public void RestoreState(object state)
        {
            mana.value = (float)state;
        }
    }
}