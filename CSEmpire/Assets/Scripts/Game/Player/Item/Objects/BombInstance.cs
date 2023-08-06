using Photon.Pun;
using UnityEngine;

namespace Game.Player.Item.Objects
{
    public class BombInstance : ItemInstance
    {
        [HideInInspector] public Bomb bomb;
        
        protected override void Init()
        {
        }

        public override void Use(PhotonView pv, bool automaticFire)
        {
            
        }

        protected override void RefreshItemReference()
        {
            bomb = (Bomb)item;
        }
    }
}