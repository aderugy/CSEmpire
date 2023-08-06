using Photon.Pun;
using TMPro;
using UnityEngine;

namespace MenuHandling
{ 
    /// <author>Arthur de Rugy</author>
    /// <summary>
    /// Handles different menus in the lobby.
    /// Singleton class.
    /// </summary>
    public class LobbyMenuManager : MonoBehaviour
    {
        public static LobbyMenuManager Instance;

        /// <summary>
        /// List of all the menus in the lobby.
        /// </summary>
        [SerializeField] private Menu[] menus;
        [SerializeField] private TMP_InputField createRoomNameInputField;
        [SerializeField] private TMP_InputField findRoomNameInputField;
        [SerializeField] private GameObject startButton;
        
        /// <summary>
        /// Instantiate the singleton when the script is called.
        /// </summary>
        public void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Closes the window (works only on built version)
        /// </summary>
        public void Quit()
        {
            Application.Quit();
        }
        
        /// <summary>
        /// Opens the menu with the given name.
        /// Does nothing if not found.
        ///
        /// Careful: it closes all the other menus before opening.
        /// </summary>
        /// <param name="menuName">Name of the menu to open.</param>
        public void OpenMenu(string menuName)
        {
            CloseAll();
            foreach (Menu menu in menus)
            {
                if (menu.menuName == menuName)
                    OpenMenu(menu);
            }
            
            if (menuName.Equals(Menus.RoomMenu))
                startButton.SetActive(PhotonNetwork.IsMasterClient);
        }

        /// <summary>
        /// Creates the room using the name specified in the input field if not null nor empty.
        /// </summary>
        public void CreateRoom()
        {
            if (string.IsNullOrEmpty(createRoomNameInputField.text))
                return;

            Instance.OpenMenu(Menus.LoadingMenu);
            PhotonNetwork.CreateRoom(createRoomNameInputField.text);
        }

        /// <summary>
        /// Joins the room using the name specified in the input field if not null nor empty.
        /// </summary>
        public void JoinRoom()
        {
            if (string.IsNullOrEmpty(findRoomNameInputField.text))
                return;

            Instance.OpenMenu(Menus.LoadingMenu);
            
            if (!PhotonNetwork.JoinRoom(findRoomNameInputField.text))
                Instance.OpenMenu(Menus.MainMenu);
        }


        public void JoinRoom(string s)
        {
            PhotonNetwork.JoinRoom(s);
        }

        private void CloseAll()
        {
            foreach (Menu menu in menus)
            {
                CloseMenu(menu);
            }
        }
        
        private static void OpenMenu(Menu menu)
        {
            menu.Open();
        }

        private static void CloseMenu(Menu menu)
        {
            menu.Close();
        }

        /// <summary>
        /// Opens the loading screen (waiting for Unity to do what it has to do)
        /// Remark: it calls the CloseAll() and OpenMenu() methods.
        /// </summary>
        public void OpenLoadingMenu()
        {
            OpenMenu(Menus.LoadingMenu);
        }

        /// <summary>
        /// Opens the main menu (the one on which the game opens)
        /// Remark: it calls the CloseAll() and OpenMenu() methods.
        /// </summary>
        public void OpenMainMenu()
        {
            OpenMenu(Menus.MainMenu);
        }

        /// <summary>
        /// Opens the create room menu (the one where you choose a name and create the room)
        /// Remark: it calls the CloseAll() and OpenMenu() methods.
        /// </summary>
        public void OpenCreateRoomMenu()
        {
            OpenMenu(Menus.CreateRoomMenu);
        }

        public void OpenLoginMenu()
        {
            OpenMenu(Menus.LoginMenu);
        }

        /// <summary>
        /// Opens the find room menu (the one where you can join a room from name)
        /// Remark: it calls the CloseAll() and OpenMenu() methods.
        /// </summary>
        public void OpenFindRoomMenu()
        {
            OpenMenu(Menus.FindRoomMenu);
        }

        /// <summary>
        /// Opens the room menu (the one where you wait until the game starts)
        /// Remark: it calls the CloseAll() and OpenMenu() methods.
        /// </summary>
        public void OpenRoomMenu()
        {
            OpenMenu(Menus.RoomMenu);
        }
    }
}