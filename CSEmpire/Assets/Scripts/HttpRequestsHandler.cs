using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public static class HttpRequestsHandler
{
    private const string ServerUrl = "http://179.43.142.142:3002/api/v1/";
    public static async Task<R> GetHttpRequest<R>(string url, R defaultValue)
    {
        url = ServerUrl + url;
        Debug.LogWarning(url);
        using var www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Content-Type", "application/json");

        var operation = www.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        if (www.result == UnityWebRequest.Result.Success)
        {
            try
            {
                return JsonConvert.DeserializeObject<R>(www.downloadHandler.text);
            }
            catch
            {
                return defaultValue;
            }
        }

        Debug.Log($"Failed: {www.error}");
        throw new InvalidDataException();
    }

    public static async Task<long> GetHttpRequest(string url)
    {
        url = ServerUrl + url;
        using var www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Content-Type", "application/json");

        var operation = www.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        return www.responseCode;
    }

    public static async Task<long> PostHttpRequest(string url, Dictionary<string, string> body)
    {
        url = ServerUrl + url;
        using var www = UnityWebRequest.Post(url, body);

        var operation = www.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        return www.responseCode;
    }
}