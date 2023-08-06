using Game.Player.Item.Objects;
using Game.Player.Item.Weapons;
using UnityEngine;

namespace Game.Player.Item
{
    public class Items : MonoBehaviour
    {
        [SerializeField] public Item[] items;
        
        [SerializeField] public Gun ak47;
        [SerializeField] public Gun m4a1s;
        
        [SerializeField] public Gun usp;
        [SerializeField] public Gun glock;
        [SerializeField] public Gun desertEagle;
        
        [SerializeField] public Knife defaultKnifeTerrorist;

        [SerializeField] public Grenade defaultGrenade;


        public static Items Instance;

        public void Awake()
        {
            items = new Item[] { ak47, m4a1s, usp, glock, desertEagle, defaultKnifeTerrorist, defaultGrenade };
            
            for (int i = 0; i < items.Length; i++)
                items[i].ItemID = i;
            
            Instance = this;
        }
    }
}