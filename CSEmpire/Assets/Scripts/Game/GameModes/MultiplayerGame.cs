using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ExitGames.Client.Photon;
using Game.EventManagement;
using Game.Player;
using Photon.Pun;
using UnityEngine;

namespace Game.GameModes
{
    /// <summary>
    /// Handles the behaviours of a Multiplayer Game:
    /// - Round system
    /// - Gold system
    /// - Attacker / Defender spawn system
    /// Remark: Singleton class
    /// </summary>
    public class MultiplayerGame : Game
    {
        private const int RoundCount = 9;
        private const int RoundDurationSeconds = 90;
        [SerializeField] public int currentRound;

        /// <summary>
        /// When starting the game, spawning the PlayerManager,
        /// initiating the 'currentRound' field and starting the first round.
        /// </summary>
        public override void StartGame()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1) return;
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero,
                Quaternion.identity).GetComponent<PlayerManager>();
            currentRound = 1;
            
            if (!PhotonNetwork.IsMasterClient || !photonView.IsMine) return;

            AddGoldToAllPlayers(800);
            StartRound();
        }

        private void StartRound()
        {
            Dictionary<byte, object> spawnEventData = new()
            {
                { (byte)DataCodes.CurrentRound, currentRound }, 
                { (byte) DataCodes.SetUntouchable, false }
            };
            
            Utils.RaiseEvent(EventCodes.SpawnPlayers, spawnEventData);

            const int seconds = 5;
            Dictionary<byte, object> timerData = new()
            {
                { (byte)DataCodes.TimerTitle, "SPAWNING IN" },
                { (byte)DataCodes.TimerCount, seconds },
            };
            
            Utils.RaiseEvent(EventCodes.StartTimer, timerData);
            StartCoroutine(StartOfRoundTimer(seconds));
        }

        private void BeginRound()
        {
            Dictionary<byte, object> timerData = new()
            {
                { (byte)DataCodes.TimerTitle, "SPAWNING IN" },
                { (byte)DataCodes.TimerCount, RoundDurationSeconds }
            };
            Utils.RaiseEvent(EventCodes.StartTimer, timerData);
            StartCoroutine(RoundTimer(RoundDurationSeconds));
        }
        
        private IEnumerator StartOfRoundTimer(int timer)
        {
            AllowUsingItems(false);
            FreezePosition(true);
            
            while (timer > 0)
            {
                yield return new WaitForSeconds(1);
                timer--;
            }
            
            AllowUsingItems(true);
            FreezePosition(false);
            BeginRound();
        }

        private IEnumerator RoundTimer(int timer)
        {
            while (timer > 0)
            {
                yield return new WaitForSeconds(1);
                timer--;
            }
            
            EndRound();
        }

        public void EndRoundTimer(bool endRound)
        {
            StopCoroutine(nameof(RoundTimer));
            
            if (endRound)
                EndRound();
        }

        /// <summary>
        /// End of round logic.
        /// Ends the game if over, else going to next round
        /// and distributing gold.
        /// </summary>
        private void EndRound()
        {
            photonView.RPC(nameof(RPC_EndRound), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_EndRound()
        {
            if (currentRound == RoundCount)
            {
                EndGame();
                return;
            }

            const int seconds = 5;
            Dictionary<byte, object> timerData = new()
            {
                { (byte)DataCodes.TimerTitle, "RESTARTING IN" },
                { (byte)DataCodes.TimerCount, seconds },
            };

            Utils.RaiseEvent(EventCodes.StartTimer, timerData);
            StartCoroutine(EndOfRoundTimer(5));
        }

        private IEnumerator EndOfRoundTimer(int timer)
        {
            while (timer > 0)
            {
                yield return new WaitForSeconds(1);
                timer--;
            }

            currentRound++;
            UpdateRounds();

            if (!PhotonNetwork.IsMasterClient || !photonView.IsMine) yield break;
            AddGoldToAllPlayers(1000);
            StartRound();
        }

        /// <summary>
        /// Raises an event that is sent to all the PlayerManager
        /// instances on all the clients.
        /// Contains the amount of gold to add as Data.
        /// Key is DataCodes.GoldAmount
        /// </summary>
        /// <param name="amount">The amount of gold to be added.</param>
        private static void AddGoldToAllPlayers(int amount) {
            Dictionary<byte, object> data = new() { { (byte) DataCodes.GoldAmount, amount} };
            Utils.RaiseEvent(EventCodes.AddGoldToAllPlayers, data);
        }

        /// <summary>
        /// Asks all the clients to update the number of the round (on the gui).
        /// Contains the updated number of the round as data, with key being
        /// DataCodes.CurrentRound
        /// </summary>
        private void UpdateRounds() {
            Dictionary<byte, object> data = new() { { (byte) DataCodes.CurrentRound, currentRound} };
            Utils.RaiseEvent(EventCodes.UpdateRounds, data);
        }

        /// <summary>
        /// Behaviour at the end of the game.
        /// </summary>
        public override void EndGame()
        {
            QuitGame();
        }

        public override void OnPlayerKilled(PhotonView killerPhotonView, PhotonView killedPhotonView)
        {
            if (killerPhotonView.ViewID != killedPhotonView.ViewID)
                killerPhotonView.RPC(nameof(PlayerManager.RPC_Kill), RpcTarget.All, 500);
            
            killedPhotonView.RPC(nameof(PlayerManager.RPC_Die), RpcTarget.All);
                        
            EndRoundTimer(true);
        }
    }
}
