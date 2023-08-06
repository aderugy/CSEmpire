using Photon.Pun;
using UnityEngine;

namespace Game.Player.Item.Objects
{
    public class ThrownGrenade : MonoBehaviourPunCallbacks
    {
        [HideInInspector] public Grenade Grenade;
        public AudioSource audioSource;
        private bool _isDestroyed;

        public GameObject[] body;

        [PunRPC]
        public void RPC_Explode(int attackerViewID)
        {
            _isDestroyed = true;
            
            ApplyGrenadeEffect(attackerViewID);
            Instantiate(Grenade.explosionEffect, gameObject.transform.position, Quaternion.identity);
            
            audioSource.clip = Grenade.explosionSound;
            audioSource.Play();
            
            StartCoroutine(Utils.DestroyAfterDurationCoroutine(gameObject, audioSource.clip.length));
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (_isDestroyed || !(collision.relativeVelocity.magnitude > 2)) return;
            audioSource.clip = Grenade.collisionSound;
            audioSource.Play();
        }

        private void ApplyGrenadeEffect(int attackerViewID)
        {
            foreach (GameObject o in body)
                o.SetActive(false);
            
            if (!photonView.IsMine)
                return;
            
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players)
            {
                float distance = Vector3.Distance(player.transform.position, gameObject.transform.position);
                
                if (!(distance < 4f)) continue;
                int damage = (int)(Grenade.damageAtCenter / (distance + 1));
                player.GetComponent<PlayerBody>().TakeDamage(attackerViewID, damage);
            }
        }
    }
}