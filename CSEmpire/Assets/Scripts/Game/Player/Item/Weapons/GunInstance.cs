using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Game.Player.Item.Weapons
{
    public class GunInstance : ItemInstance
    {
        [HideInInspector] public int ammoTotal;
        [HideInInspector] public int ammoLeftInCurrentClip;

        private Gun gun;

        protected override void Init()
        {
            ammoLeftInCurrentClip = gun.ammoPerClip;
            ammoTotal = 5 * gun.ammoPerClip;
        }
        
        public override void Use(PhotonView owner, bool automaticFire)
        {
            if (_locked || (automaticFire && item.itemType is ItemType.Pistol))
                return;

            if (ammoLeftInCurrentClip == 0)
            {
                StartCoroutine(ReloadWeapon());
                return;
            }

            Shoot(owner);
            StartCoroutine(DelayBetweenUsesCoroutine());
        }

        protected override void RefreshItemReference()
        {
            gun = (Gun)item;
        }

        private IEnumerator ReloadWeapon()
        {
            _locked = true;

            if (ammoTotal != 0)
            {
                yield return new WaitForSeconds(2f * gun.reloadTimeInSeconds / 3f);
                int quantity = gun.ammoPerClip - ammoLeftInCurrentClip;
                if (quantity > ammoTotal) quantity = ammoTotal;

                ammoTotal -= quantity;
                ammoLeftInCurrentClip = gun.ammoPerClip;
                yield return new WaitForSeconds(gun.reloadTimeInSeconds / 3f);
            }
            
            _locked = false;
        }

        private void Shoot(PhotonView owner)
        {
            PlayAudioClip(gun.shootSound);
            ammoLeftInCurrentClip--;

            if (!owner.IsMine)
                return;
            
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            ray.origin = Camera.main.transform.position;

            if (!Physics.Raycast(ray, out RaycastHit hit)) return;

            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(owner.ViewID, gun.damage);
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 1000, Color.red, 1000);
        }
    }
}