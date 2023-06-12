using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class ManaDisplay : MonoBehaviour
    {
        Mana mana;
        GameObject player;
        void Awake()
        {
            player = GameObject.FindWithTag("Player");
            mana = player.GetComponent<Mana>();
        }
        void Update()
        {
            GetComponent<Text>().text = String.Format("{0:0}/{1:0}", mana.GetMana(), mana.GetMaxMana());
        }
    }
}