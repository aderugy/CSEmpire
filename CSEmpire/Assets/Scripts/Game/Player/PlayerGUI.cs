using Game.Player.Item.Weapons;
using MenuHandling;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Player
{
    /// <summary>
    /// Handles all the interactions with the player's GUI.
    /// </summary>
    public class PlayerGUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI healthCount;
        [SerializeField] private Image healthBar;
        [SerializeField] private TextMeshProUGUI goldCount;
        [SerializeField] private TextMeshProUGUI roundCount;

        [SerializeField] private TextMeshProUGUI ammoLeftGUI;
        [SerializeField] private TextMeshProUGUI ammoTotalGUI;

        [SerializeField] private TextMeshProUGUI terroristScoreGUI;
        [SerializeField] private TextMeshProUGUI counterTerroristScoreGUI;

        [SerializeField] private TextMeshProUGUI roundTimerValueGUI;

        // Game Over part
        
        
        /// <summary>
        /// Updates the value and level of the health bar
        /// </summary>
        public void UpdateHealth(int health)
        {
            healthBar.fillAmount = (float)health / PlayerBody.MaxHealth;
            healthCount.text = health.ToString();

            // part of the game over screen
            
          
        }

        /// <summary>
        /// Updates the value and level of the health bar
        /// </summary>
        public void UpdateGold(PhotonView target, int gold)
        {
            if (target.IsMine)
                goldCount.text = gold.ToString();
        }

        public void UpdateScore(int terroristScore, int counterTerroristScore)
        {
            terroristScoreGUI.text = terroristScore.ToString();
            counterTerroristScoreGUI.text = counterTerroristScore.ToString();
        }

        public void UpdateRoundTimer(int seconds)
        {
            int minutes = seconds / 60;
            seconds %= 60;

            roundTimerValueGUI.text = $"{minutes}:{(seconds < 10 ? "0" + seconds : seconds)}";
        }

        public void UpdateRounds(int round)
        {
            if (roundCount is null) return;

            roundCount.text = round != -1 ? round.ToString() : "SOLO";
        }

        public void ReloadAmmoInfo(Item.ItemInstance item)
        {
            if (item is GunInstance gun)
            {
                ammoLeftGUI.text = gun.ammoLeftInCurrentClip.ToString();
                ammoTotalGUI.text = gun.ammoTotal.ToString();
            }
            else
            {
                ammoLeftGUI.text = "-";
                ammoTotalGUI.text = "-";
            }
        }
    }
}
