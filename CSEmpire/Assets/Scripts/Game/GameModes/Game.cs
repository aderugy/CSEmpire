using Photon.Pun;
using UnityEngine.SceneManagement;

namespace Game.GameModes {

    /// <summary>
    /// Abstract class which gives the skeleton of every game mode in the game.
    /// Those will have to handle the different events called in the PlayerManager class, in order to
    /// implement the expected behaviour.
    /// </summary>
    public abstract class Game : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// Only instance of this class.
        /// </summary>
        public static Game Instance;
        
        public bool isFrozen;
        public bool canUseItems = true;
        
        /// <summary>
        /// Called to start the game.
        /// </summary>
        public abstract void StartGame();

        /// <summary>
        /// Called to end the game.
        /// </summary>
        public abstract void EndGame();

        protected void QuitGame()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("MenuScene");
        }

        public abstract void OnPlayerKilled(PhotonView killer, PhotonView killed);

        /// <summary>
        /// Instantiation of the singleton.
        /// We also retrieve the PhotonView component.
        /// </summary>
        private void Awake()
        {
            Instance = this;
        }

        protected void FreezePosition(bool freeze)
        {
            photonView.RPC(nameof(RPC_FreezePosition), RpcTarget.All, freeze);
        }

        [PunRPC]
        protected void RPC_FreezePosition(bool freeze)
        {
            isFrozen = freeze;
        }

        protected void AllowUsingItems(bool value)
        {
            photonView.RPC(nameof(RPC_AllowUsingItems), RpcTarget.All, value);
        }

        [PunRPC]
        protected void RPC_AllowUsingItems(bool value)
        {
            canUseItems = value;
        }
    }
}