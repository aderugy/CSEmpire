using UnityEngine;

namespace Game.Player.Item.Objects
{
    public class Grenade : Consumables
    {
        public float delay;
        public float damageAtCenter;
        
        public AudioClip explosionSound;
        public AudioClip pullingPinSound;
        public AudioClip throwingSound;
        public AudioClip collisionSound;
        
        public GameObject explosionEffect;

        
    }
}