using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.SpawnManagement
{
    public class SpawnManager : MonoBehaviour
    {
        public const string Attacker = "Attacker";
        public const string Defender = "Defender";
        public const string Bot = "Bot";
        
        public static SpawnManager Instance;

        private List<SpawnPoint> _attackers;
        private List<SpawnPoint> _defenders;
        private List<SpawnPoint> _bots;

        private void Awake()
        {
            Instance = this;
            _attackers = GetComponentsInChildren<SpawnPoint>().Where(spawn => spawn.CompareTag(Attacker)).ToList();
            _defenders = GetComponentsInChildren<SpawnPoint>().Where(spawn => spawn.CompareTag(Defender)).ToList();
            _bots = GetComponentsInChildren<SpawnPoint>().Where(spawn => spawn.CompareTag(Bot)).ToList();
        }

        public Transform GetRandomSpawnPoint(string type)
        {
            List<SpawnPoint> spawnPoints = type switch
            {
                Attacker => _attackers,
                Defender => _defenders,
                Bot => _bots,
                _ => throw new ArgumentException("No such spawn point type: " + type)
            };

            if (spawnPoints.Count == 0) throw new IndexOutOfRangeException("No spawn points left");

            return spawnPoints[Random.Range(0, spawnPoints.Count)].transform;
        }

        public List<Transform> GetSpawnPointList(string role)
        {
            List<SpawnPoint> spawnPoints = role switch
            {
                Attacker => _attackers,
                Defender => _defenders,
                Bot => _bots,
                _ => throw new ArgumentException("No such role.")
            };

            return spawnPoints.Select(spawnPoint => spawnPoint.transform).ToList();
        }
    }
}
