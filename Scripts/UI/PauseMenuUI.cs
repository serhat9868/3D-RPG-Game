using RPG.Control;
using RPG.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        PlayerController playerController;

        void Awake()
        {
            playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        }
        private void OnEnable()
        {
            if (playerController == null) return;
            //when timeScale is 0,time is stopped and time speed is 0 during game.
            Time.timeScale = 0;
            playerController.enabled = false;
        }
        private void OnDisable()
        {
            if (playerController == null) return;
            //Time scale is speed of time during game.when it is 1,time is default speed.
            Time.timeScale = 1;
            playerController.enabled = true;
        }
        public void Save()
        {
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();
        }

        public void SaveAndQuit()
        {
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();
            savingWrapper.LoadMenu();
        }
   
    }
}