using Game.Utils;
using RPG.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        LazyValue<SavingWrapper> savingWrapper;

        private void Awake()
        {
            savingWrapper = new LazyValue<SavingWrapper>(GetSavingWrapper);
        }
        public void ContinueGame()
        {
            savingWrapper.value.ContinueGame();
        }
        public void NewGame(TMP_InputField saveFile)
        {
       
            savingWrapper.value.NewGame(saveFile.text);
        }
        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
            private SavingWrapper GetSavingWrapper()
        {
            return FindObjectOfType<SavingWrapper>();
        }
    }
}
