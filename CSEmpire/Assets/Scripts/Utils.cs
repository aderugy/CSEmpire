using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Game.EventManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static T GetValueFromEventData<T>(EventData data, DataCodes key)
    {
        return (T)((Dictionary<byte, object>)data.CustomData)[(byte)key];
    }

    public static void RaiseEvent(EventCodes code, Dictionary<byte, object> data)
    {
        RaiseEventOptions options = new() { Receivers = ReceiverGroup.All };

        PhotonNetwork.RaiseEvent((byte) code, data, options, SendOptions.SendReliable);
        Debug.Log($"Raising event with event code: {code.ToString()} ({(byte) code})");
    }

    public static void PlayAudioClip(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }

    public static IEnumerator DestroyAfterDurationCoroutine(Object go, float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(go);
    }
}