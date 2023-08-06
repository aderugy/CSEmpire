using System.Threading.Tasks;
using MenuHandling;
using Photon.Pun;
using UnityEngine;

/// <author>Arthur de Rugy</author>
/// <summary>
/// Sets up and handles the connection to Photon's server.
/// </summary>
public class ServerConnect : MonoBehaviourPunCallbacks
{
    private string gameMode = "MULTI";

    /// <summary>
    /// Opens loading screen and sets the server up when the script is first called (ie: when the application is started)
    /// </summary>
    public void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void StartGame()
    {
        LobbyMenuManager.Instance.OpenLoadingMenu();
        PhotonNetwork.LoadLevel(1);
    }

    public void StartSoloGame()
    {
        gameMode = "SOLO";
        LobbyMenuManager.Instance.OpenMenu(Menus.LoadingMenu);
        PhotonNetwork.CreateRoom("Solo" + Random.Range(0, 100000));
    }

    public async void MatchMakingGame()
    {
        string username = Credentials.Instance.GetUsername();
        Room room = await HttpRequestsHandler.GetHttpRequest<Room>($"room?username={username}", null);
        if (room.RoomId.Equals(username))
            PhotonNetwork.CreateRoom(room.RoomId);
        else
            PhotonNetwork.JoinRoom(room.RoomId);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
        LobbyMenuManager.Instance.OpenMainMenu();
    }

    public override void OnJoinedRoom()
    {
        if (gameMode == "SOLO")
            PhotonNetwork.LoadLevel(2);
        else
            LobbyMenuManager.Instance.OpenRoomMenu();
        Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        LobbyMenuManager.Instance.JoinRoom("dev");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        LobbyMenuManager.Instance.OpenMainMenu();
        Debug.LogError($"Joining room failed with error code {returnCode.ToString()}.");
        Debug.LogError(message);
    }
}