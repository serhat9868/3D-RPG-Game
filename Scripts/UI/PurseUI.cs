using RPG.Inventories;
using TMPro;
using UnityEngine;
namespace RPG.UI {
    public class PurseUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI moneyUI;

        Purse playerPurse = null;
       
        private void Start()
        {
            playerPurse = GameObject.FindGameObjectWithTag("Player").GetComponent<Purse>();

            if (playerPurse != null)
            {
                playerPurse.onchange += RefreshUI;
            }
            RefreshUI();
          
        }
       private void RefreshUI()
        {
            moneyUI.text = $"${playerPurse.GetBalance():N2}";

        }
    } 
}
