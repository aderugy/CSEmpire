using System;
using System.Collections;
using Game.Player.Item;
using Game.Player.Item.Weapons;
using Photon.Pun;
using UnityEngine;

namespace Game.Player
{
	public sealed class PlayerBody : MonoBehaviourPunCallbacks, IDamageable
    {
        public const int MaxHealth = 100;
        [SerializeField] private int health = MaxHealth;

        public bool isUntouchable;

        [SerializeField] private Transform itemHolder;
        
        /// <summary>
        /// Script that handles the GUI.
        /// </summary>
        [HideInInspector] public PlayerGUI playerGUI;

        [SerializeField] private GameObject[] itemInstancePrefabs;
        
        private ItemInstance itemInstance;

        private PlayerManager _playerManager;

        [SerializeField] private GameObject[] bodyParts;
        private bool _isDead;

        private void Start()
        {
            if (photonView.IsMine) return;
            Destroy(GetComponentInChildren<Camera>().gameObject);
        }
        
        private void Awake()
        {
            _playerManager = PhotonView.Find((int) photonView.InstantiationData[0]).GetComponent<PlayerManager>();
        }

        public void TakeDamage(int attackerViewID, int damage)
        {
            if (isUntouchable) return;
            
            photonView.RPC(nameof(RPC_TakeDamage), RpcTarget.All, attackerViewID, damage);
        }

        public void EquipItem(Item.Item item)
        {
            photonView.RPC(nameof(RPC_EquipItem), RpcTarget.All, item.ItemID);
            
            Utils.PlayAudioClip(itemInstance.audioSource, itemInstance.item.drawingSound);
            playerGUI.ReloadAmmoInfo(itemInstance);

            StartCoroutine(DrawingDelayBeforeUseCoroutine(itemInstance.item.drawDelayBeforeUse));
        }

        private static IEnumerator DrawingDelayBeforeUseCoroutine(float duration)
        {
            yield return new WaitForSeconds(duration);
        }

        [PunRPC]
        public void RPC_EquipItem(int itemID)
        {
            if (itemInstance is not null)
            {
                itemInstance.Destroy();
                Destroy(itemInstance);
            }

            Item.Item item = Items.Instance.items[itemID];
            int itemInstancePrefabIndex = (int)item.itemType;
            
            if (itemInstancePrefabs[itemInstancePrefabIndex] is null)
                throw new NotImplementedException();

            itemInstance = Instantiate(itemInstancePrefabs[itemInstancePrefabIndex], itemHolder)
                .GetComponent<ItemInstance>();

            itemInstance.Init(item);
            itemInstance.Spawn(itemHolder);
        }

        [PunRPC]
        private void RPC_TakeDamage(int attackerViewID, int damage)
        {
            if (!photonView.IsMine) return;

            if (health >= damage)
                health -= damage;
            else
            {
                health = 0;
            }

            playerGUI.UpdateHealth(health);

            if (health > 0) return;

            Die(attackerViewID);
        }
        
        private void Die(int attackerViewID = -1)
        {
            if (_isDead) return;
            
            _isDead = true;
            _playerManager.Die(attackerViewID);
            SetBodyActive(false);
        }

        public void Spawn() {
            health = MaxHealth;
            _isDead = false;
            
            if (!photonView.IsMine) return;
            playerGUI.UpdateHealth(health);
            SetBodyActive(true);
        }

        private void SetBodyActive(bool active)
        {
            photonView.RPC(nameof(RPC_SetBodyActive), RpcTarget.All, active);
        }

        public void UseItemHeld(PhotonView owner, bool automaticFire)
        {
            photonView.RPC(nameof(RPC_UseItemHeld), RpcTarget.All, owner.ViewID, automaticFire);
            StartCoroutine(WaitUntilReloadedCoroutine());
        }

        private IEnumerator WaitUntilReloadedCoroutine()
        {
            playerGUI.ReloadAmmoInfo(itemInstance);
            yield return new WaitWhile(() =>
                itemInstance is GunInstance { ammoLeftInCurrentClip: 0 });
            playerGUI.ReloadAmmoInfo(itemInstance);
        }

        [PunRPC]
        public void RPC_UseItemHeld(int ownerPhotonViewID, bool automaticFire)
        {
            itemInstance.Use(PhotonView.Find(ownerPhotonViewID), automaticFire);
        }

        [PunRPC]
        public void RPC_SetBodyActive(bool active)
        {
            foreach (GameObject o in bodyParts)
                o.SetActive(active);
        }
    }
}
