using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitFromEscape : MonoBehaviour
{
    public string ScenetoLoad;
    // Update is called once per frame
    
    public void exit()
    {
        PhotonNetwork.Disconnect();
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(ScenetoLoad);
    }
}
