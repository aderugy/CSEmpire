using System.Collections;
using System.IO;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

namespace Game.Player.Item.Objects
{
    public class GrenadeInstance : ItemInstance
    {
        private Grenade grenade;
        private bool _isTriggered;
        private bool _isThrown;

        [CanBeNull] private GameObject grenadeInstance;

        public void Update()
        {
            if (_isThrown || !_isTriggered || !Input.GetMouseButtonUp(0)) return;
            _isThrown = true;
            Launch();
        }

        protected override void Init()
        {
            
        }

        public override void Use(PhotonView pv, bool automaticFire)
        {
            if (automaticFire || _isTriggered || _isThrown)
                return;

            audioSource.clip = grenade.pullingPinSound;
            audioSource.Play();
            
            StartCoroutine(GrenadeTimerCoroutine(pv.ViewID));
        }

        private IEnumerator GrenadeTimerCoroutine(int viewID)
        {
            _isTriggered = true;
            yield return new WaitForSeconds(grenade.delay);
            Explode(viewID);
        }

        protected override void RefreshItemReference()
        {
            grenade = (Grenade) item;
        }

        private void Launch()
        {
            if (photonView.IsMine)
                Utils.PlayAudioClip(audioSource, grenade.throwingSound);
            
            grenadeInstance = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "grenade"), Camera.main.transform.position, Quaternion.identity);
            grenadeInstance.GetComponent<ThrownGrenade>().Grenade = grenade;
            grenadeInstance.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 1000);
        }

        private void Explode(int viewID)
        {
            grenadeInstance.GetComponent<PhotonView>().RPC(nameof(ThrownGrenade.RPC_Explode), RpcTarget.All, viewID);
            
            grenadeInstance = null;
            _isThrown = false;
            _isTriggered = false;
        }
    }
}