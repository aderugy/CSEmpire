using UnityEngine;

namespace Game.SpawnManagement
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField] private GameObject graphics;

        private void Awake()
        {
            graphics.SetActive(false);
        }
    }
}
