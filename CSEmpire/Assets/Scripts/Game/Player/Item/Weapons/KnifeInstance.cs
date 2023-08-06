using Photon.Pun;

namespace Game.Player.Item.Weapons
{
    public class KnifeInstance : ItemInstance
    {
        private Knife knife;

        protected override void Init()
        {
            
        }

        public override void Use(PhotonView photonView, bool _)
        {
            throw new System.NotImplementedException();
        }

        protected override void RefreshItemReference()
        {
            knife = (Knife)item;
        }
    }
}