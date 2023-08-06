using System.Collections.Generic;
using System.Threading.Tasks;
using MenuHandling;
using Photon.Pun;
using UnityEngine;

public class Credentials : MonoBehaviour
{
    private string _username;
    public static Credentials Instance;
    public bool IsConnected;

    private async void Awake()
    {
        if (Instance is null)
            Instance = this;
        else
            return;

        DontDestroyOnLoad(this);
        _username = "dev";
        LobbyMenuManager.Instance.OpenMainMenu();
        return;

        string username = PlayerPrefs.GetString("username", "");
        string hashedPassword = PlayerPrefs.GetString("password", "");

        if (username.Length == 0 || hashedPassword.Length == 0 || !await Login(username, hashedPassword))
            LobbyMenuManager.Instance.OpenLoginMenu();
        else
        {
            if (PhotonNetwork.IsConnected)
                LobbyMenuManager.Instance.OpenMainMenu();
            else
                LobbyMenuManager.Instance.OpenLoadingMenu();
        }
    }

    public async Task<bool> Login(string username, string hashedPassword)
    {
        try
        {
            long code = await HttpRequestsHandler.GetHttpRequest(
                $"login?username={username}&password={hashedPassword}");

            Debug.Log(code);
            if (code != 200) return false;
            
            _username = username;
            Debug.Log("Successfully logged in.");
            PlayerPrefs.SetString("username", username);
            PlayerPrefs.SetString("password", hashedPassword);
            IsConnected = true;
            return true;
        }
        catch
        {
            Debug.Log("Failed logging in.");
            return false;
        }
    }

    public async Task<bool> Register(string username, string password, string email)
    {
        if (username.Length == 0 || password.Length == 0 || email.Length == 0)
            return false;
        
        string hashedPassword = HashBCrypt(password);

        Dictionary<string, string> requestBody = new() {
            { "username", username },
            { "password", hashedPassword },
            { "email", email }
        };

        long code = await HttpRequestsHandler.PostHttpRequest("register", requestBody);

        if (code != 200)
        {
            Debug.LogWarning($"Registering with username: '{username}' and email '{email}' failed.");
            return false;
        }

        _username = username;
        
        PlayerPrefs.SetString("username", username);
        PlayerPrefs.SetString("password", hashedPassword);
        IsConnected = true;
        return true;
    }

    public static string HashBCrypt(string toHash)
    {
        //return BCrypt.Net.BCrypt.HashPassword(toHash);
        return toHash;
    }
    
    public string GetUsername() => _username;
}