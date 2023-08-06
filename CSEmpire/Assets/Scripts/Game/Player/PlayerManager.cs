using System.Collections;
using System.Collections.Generic;
using System.IO;
using ExitGames.Client.Photon;
using Game.EventManagement;
using Game.SpawnManagement;
using Photon.Pun;
using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Handles the creation of the player when joining the room.
    /// </summary>
    public class PlayerManager : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// The role of the current player ('Attacker'/'Defender'/'Bot')
        /// </summary>
        private string _role;
        
        /// <summary>
        /// Reference to the Player's GameObject.
        /// </summary>
        public PlayerBody playerBody;
        
        /// <summary>
        /// The amount of gold owned by the current player.
        /// </summary>
        private int _gold;

        /// <summary>
        /// Reference to the script handling the gui of the player.
        /// </summary>
        [SerializeField] private PlayerGUI playerGUI;
        
        
        [SerializeField] private PlayerInventory inventory;

        /// <summary>
        /// Number of kills since the beginning of the game.
        /// </summary>
        private int _killCount;
        
        /// <summary>
        /// Number of deaths since the beginning of the game.
        /// </summary>
        private int _deathCount;

        private void Start()
        {
            if (!photonView.IsMine)
                Destroy(playerGUI.gameObject);
        }

        /// <summary>
        /// Retrieves the spawn point corresponding to the role of the player.
        /// If it has not been instantiated, it creates the Player.
        /// Else it moves it to the SpawnPoint, makes it visible and resets its stats.
        /// </summary>
        /// <param name="attacker">True if the player is an 'Attacker', false if 'Defender'</param>
        private void CreateController(bool attacker)
        {
            _role  = attacker ? SpawnManager.Attacker : SpawnManager.Defender;
            Transform spawnPoint = SpawnManager.Instance.GetRandomSpawnPoint(_role);

            if (playerBody is null) {
                playerBody = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), 
                    spawnPoint.position, spawnPoint.rotation, 0, new object[] { photonView.ViewID }).GetComponent<PlayerBody>();
                playerBody.playerGUI = playerGUI;
                
                inventory.SetDefaultInventory(true);
                RefreshPlayerBodyItemHeld();
                
                if (photonView.IsMine)
                    playerGUI.UpdateHealth(100);
            } else {
                Transform spawnPointTransform = spawnPoint.transform;
                Transform playerBodyTransform = playerBody.transform;
                
                playerBodyTransform.position = spawnPointTransform.position;
                playerBodyTransform.rotation = spawnPointTransform.rotation;
            }
            
            playerBody.Spawn();
        }

        private void SetUntouchable(bool value)
        {
            playerBody.isUntouchable = value;
        }

        private void RefreshPlayerBodyItemHeld()
        {
            playerBody.EquipItem(inventory.GetCurrentItem());
        }

        /// <summary>
        /// Creates the player and attributes the role depending on the current round number.
        /// </summary>
        /// <param name="round">Current round number.</param>
        private void Spawn(int round)
        {
            if (!photonView.IsMine)
                return;

            int terroristScore = _role == "Attacker" ? _killCount : _deathCount;
            int counterTerroristScore = _role != "Attacker" ? _killCount : _deathCount;
            
            Debug.Log(_killCount);
            Debug.Log(_deathCount);
            
            playerGUI.UpdateScore(terroristScore, counterTerroristScore);
            playerGUI.UpdateRounds(round);
            CreateController((PhotonNetwork.IsMasterClient && round <= 5) || (!PhotonNetwork.IsMasterClient && round > 5) || round == -1);
        }

        /// <summary>
        /// Called on a player that just died.
        /// Makes it invisible, increases '_deathCount' and updates '_alive'.
        /// If there is a killer, it notifies it.
        /// Then raises an event containing the ViewIDs of the killer and the killed.
        /// </summary>
        /// <param name="attackerViewID">The Photon ViewID of the attacker.</param>
        public void Die(int attackerViewID)
        {
            PhotonView killerPhotonView = attackerViewID != -1 ? PhotonView.Find(attackerViewID) : photonView;
            GameModes.Game.Instance.OnPlayerKilled(killerPhotonView, photonView);
        }

        private void OnStartTimerEventReceived(EventData eventData)
        {
            if (playerGUI is null || eventData.Code != (byte)EventCodes.StartTimer) return;

            int value = Utils.GetValueFromEventData<int>(eventData, DataCodes.TimerCount);

            if (photonView.IsMine)
                StartCoroutine(StartTimerInSeconds(value));
        }

        private IEnumerator StartTimerInSeconds(int seconds)
        {
            playerGUI.UpdateRoundTimer(seconds);
            while (seconds > 0)
            {
                playerGUI.UpdateRoundTimer(seconds);
                seconds--;
                yield return new WaitForSeconds(1);
            }
            playerGUI.UpdateRoundTimer(seconds);
        }

        [PunRPC]
        public void RPC_Die()
        {
            _deathCount++;
        }

        [PunRPC]
        public void RPC_Kill(int gold)
        {
            _killCount++;
            _gold += gold;
        }
        
        [PunRPC]
        private void RPC_AddGold(int amount)
        {
            _gold += amount;

            if (playerGUI is not null)
                playerGUI.UpdateGold(photonView, _gold);
        }

        [PunRPC]
        private void RPC_UpdateRounds(int currentRound) {
            if (playerGUI is not null)
                playerGUI.UpdateRounds(currentRound);
        }

        /// <summary>
        /// Adds gold to all the players when the corresponding event is raised.
        /// </summary>
        /// <param name="eventData"></param>
        private void OnAddGoldToAllPlayersEventReceived(EventData eventData)
        {
            if (eventData.Code != (byte)EventCodes.AddGoldToAllPlayers || !photonView.IsMine) return;
            
            Debug.Log($"Received event with event code {eventData.Code.ToString()}");
            Dictionary<byte, object> data = (Dictionary<byte, object>) eventData.CustomData;
            photonView.RPC(nameof(RPC_AddGold), RpcTarget.All, (int) data[(byte) DataCodes.GoldAmount]);
        }

        /// <summary>
        /// Updates the number of the round on the gui of all the clients when the corresponding event is raised.
        /// </summary>
        /// <param name="eventData"></param>
        private void OnUpdateRoundsEventReceived(EventData eventData) {
            if (eventData.Code != (byte)EventCodes.UpdateRounds) return;
            
            Debug.Log($"Received event with event code {eventData.Code.ToString()}");
            photonView.RPC(nameof(RPC_UpdateRounds), RpcTarget.All, 
                Utils.GetValueFromEventData<int>(eventData, DataCodes.CurrentRound));
        }

        private void OnSpawnPlayersEventReceived(EventData eventData)
        {
            if (eventData.Code != (byte)EventCodes.SpawnPlayers) return;
            
            Debug.Log($"Received event with event code {eventData.Code.ToString()}");
            Spawn(Utils.GetValueFromEventData<int>(eventData, DataCodes.CurrentRound));
            SetUntouchable(Utils.GetValueFromEventData<bool>(eventData, DataCodes.SetUntouchable));
        }
        
        /// <summary>
        /// Adds a listener for the events when active.
        /// </summary>
        public override void OnEnable() {
            base.OnEnable();
            PhotonNetwork.NetworkingClient.EventReceived += OnAddGoldToAllPlayersEventReceived;
            PhotonNetwork.NetworkingClient.EventReceived += OnUpdateRoundsEventReceived;
            PhotonNetwork.NetworkingClient.EventReceived += OnSpawnPlayersEventReceived;
            PhotonNetwork.NetworkingClient.EventReceived += OnStartTimerEventReceived;
        }

        /// <summary>
        /// Removes the listeners when inactive.
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.NetworkingClient.EventReceived -= OnAddGoldToAllPlayersEventReceived;
            PhotonNetwork.NetworkingClient.EventReceived -= OnUpdateRoundsEventReceived;
            PhotonNetwork.NetworkingClient.EventReceived -= OnSpawnPlayersEventReceived;
            PhotonNetwork.NetworkingClient.EventReceived -= OnStartTimerEventReceived;
        }
        
        private void Update()
        {
            for (int i = 0; i < 5; i++)
            {
                if (!Input.GetKeyDown((i + 1).ToString())) continue;

                if (photonView.IsMine)
                {
                    bool changed = inventory.EquipItemByIndex(i);
                    
                    if (changed)
                        playerBody.EquipItem(inventory.GetCurrentItem());
                }

                break;
            }
        
            if (Input.GetMouseButton(0) && photonView.IsMine && GameModes.Game.Instance.canUseItems)
            {
                playerBody.UseItemHeld(photonView, !Input.GetMouseButtonDown(0));
            }
        }
    }
}
