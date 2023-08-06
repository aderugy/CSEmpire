using System.Collections;
using System.Collections.Generic;
using System.IO;
using Game.EventManagement;
using Game.Player;
using Game.SpawnManagement;
using Photon.Pun;
using UnityEngine;
using Random = System.Random;

namespace Game.GameModes
{
    public class SoloGame : Game
    {
        [SerializeField] private GameObject botPrefab;

        private GameObject bot;
        
        private List<Transform> spawnPoints = new();
        private PlayerManager playerManager;

        public override void StartGame()
        {
            spawnPoints = SpawnManager.Instance.GetSpawnPointList(SpawnManager.Bot);
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 2) return;
            
            playerManager = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero,
                Quaternion.identity).GetComponent<PlayerManager>();
            
            Utils.RaiseEvent(EventCodes.SpawnPlayers, new Dictionary<byte, object>
            {
                { (byte) DataCodes.CurrentRound, -1 },
                { (byte) DataCodes.SetUntouchable, true }
            });

            StartCoroutine(WaitForBodyToBeCreated());
        }

        private IEnumerator WaitForBodyToBeCreated()
        {
            yield return new WaitUntil(() => playerManager.playerBody is not null);
            SpawnBot();
        }

        private void SpawnBot()
        {
            Transform spawnPoint = spawnPoints[new Random().Next(spawnPoints.Count)];
            bot = Instantiate(botPrefab, spawnPoint.position, Quaternion.identity);
            bot.transform.LookAt(playerManager.playerBody.transform);
        }

        public override void EndGame()
        {
            QuitGame();
        }

        public override void OnPlayerKilled(PhotonView killer, PhotonView killed)
        {
            SpawnBot();
            Debug.Log("Killed a bot.");
        }
    }
}