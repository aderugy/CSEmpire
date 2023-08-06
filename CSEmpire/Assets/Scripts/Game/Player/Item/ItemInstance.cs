using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Game.Player.Item
{
    public abstract class ItemInstance : MonoBehaviourPunCallbacks
    {
        public AudioSource audioSource;
        
        [HideInInspector] public Item item;
        [HideInInspector] public GameObject itemGameObject;
        
        protected bool _locked;

        protected void PlayAudioClip(AudioClip audioClip)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        /// <summary>
        /// HAS TO BE CALLED!!!!
        /// </summary>
        /// <param name="source"></param>
        public void Init(Item source)
        {
            item = source;
            RefreshItemReference();
            Init();
        }

        public void Spawn(Transform parent)
        {
            itemGameObject = Instantiate(item.itemPrefab, parent);
        }

        protected abstract void Init();
        
        public abstract void Use(PhotonView photonView, bool automaticFire);

        protected abstract void RefreshItemReference();

        protected IEnumerator DelayBetweenUsesCoroutine()
        {
            _locked = true;
            yield return new WaitForSeconds(item.secondsBetweenUses);
            _locked = false;
        }

        public void Destroy()
        {
            Destroy(itemGameObject);
            Destroy(audioSource);
            Destroy(gameObject);
        }
    }
}