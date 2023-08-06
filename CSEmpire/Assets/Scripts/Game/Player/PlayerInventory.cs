using System;
using System.Collections.Generic;
using System.Linq;
using Game.Player.Item;
using UnityEngine;

namespace Game.Player
{
    public sealed class PlayerInventory : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly List<Item.Item>[] inventory = new List<Item.Item>[5];

        private ItemType itemHeldType;
        private int itemIndex;

        public void SetDefaultInventory(bool isTerrorist)
        {
            inventory[0] = new List<Item.Item> { isTerrorist ? Items.Instance.ak47 : Items.Instance.m4a1s };
            inventory[1] = new List<Item.Item> { isTerrorist ? Items.Instance.glock : Items.Instance.usp };
            inventory[2] = new List<Item.Item> { Items.Instance.defaultKnifeTerrorist };
            inventory[3] = new List<Item.Item> { Items.Instance.defaultGrenade };
            inventory[4] = new List<Item.Item>();

            itemHeldType = ItemType.Rifle;
            itemIndex = 0;
        }

        public void Add(Item.Item item)
        {
            int index = (int)item.itemType;

            if (item.itemType != ItemType.Grenade && inventory[index].Count > 0)
                Drop(index);
            if (item.itemType == ItemType.Grenade && inventory[index].Count >= 3)
                return;
            
            inventory[index].Add(item);
            EquipItemByIndex(index);
        }

        private void Drop(int index)
        {
            inventory[index].Clear();
        }

        public Item.Item GetCurrentItem()
        {
            return inventory[(int)itemHeldType][itemIndex];
        }

        public bool EquipItemByIndex(int index)
        {
            if (index < 0 || index >= inventory.Length || inventory[index].Count == 0 || index == (int) itemHeldType)
                return false;

            itemIndex = 0;
            itemHeldType = inventory[index][itemIndex].itemType;
            return true;
        }

        public void Next()
        {
            while (true)
            {
                int itemHeldIndex = (int)itemHeldType;

                if (itemHeldType != ItemType.Grenade)
                    itemHeldIndex++;
                else
                {
                    itemIndex++;

                    if (itemIndex >= inventory[itemHeldIndex].Count)
                    {
                        itemIndex = 0;
                        itemHeldIndex++;
                    }
                }

                if (itemHeldIndex >= inventory.Length) itemHeldIndex = 0;

                if (inventory[itemHeldIndex].Count == 0)
                    continue;
                itemHeldType = inventory[itemHeldIndex][itemIndex].itemType;
                break;
            }
        }

        public void Previous()
        {
            while (true)
            {
                int itemHeldIndex = (int)itemHeldType;

                if (itemHeldType != ItemType.Grenade)
                    itemHeldIndex--;
                else
                {
                    itemIndex--;

                    if (itemIndex < 0)
                    {
                        itemIndex = 0;
                        itemHeldIndex--;
                    }
                }

                if (itemHeldIndex < 0) itemHeldIndex = inventory.Length - 1;

                if (inventory[itemHeldIndex].Count == 0)
                    continue;
                itemHeldType = inventory[itemHeldIndex][itemIndex].itemType;
                break;
            }
        }
    }
}