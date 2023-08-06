using UnityEngine;

namespace Game.Player.Item.Weapons
{
    public class Gun : Item
    {
        [SerializeField] public int damage;

        [SerializeField] public float reloadTimeInSeconds;

        [SerializeField] public int ammoPerClip;

        [SerializeField] public AudioClip shootSound;
    }
}