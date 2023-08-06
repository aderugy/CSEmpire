using UnityEngine;

namespace Game.Player.Item
{
    public abstract class Item : MonoBehaviour
    {
        [HideInInspector] public int ItemID;
        
        [SerializeField] public string itemName;
        [SerializeField] public ItemType itemType;
        
        [SerializeField] public float secondsBetweenUses;
        [SerializeField] private float slowFactor = 1;
        
        [SerializeField] public GameObject itemPrefab;
        [SerializeField] public int cost;

        [SerializeField] public AudioClip drawingSound;
        [SerializeField] public float drawDelayBeforeUse;
    }
}
