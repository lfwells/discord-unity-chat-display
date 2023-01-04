using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using DiscordUnityChatDisplay;

public class PollBucket : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform spawnPoint;
    public GameObject bottomPanel;
    Dictionary<string, GameObject> spawnedBalls = new Dictionary<string, GameObject>();

    public void AddVote(DiscordMember member)
    {
        var go = GameObject.Instantiate(ballPrefab, spawnPoint.position, Quaternion.Euler(0,0,Random.value*360f), transform);
        StartCoroutine(LoadImage(member.avatar, go.GetComponentInChildren<RawImage>()));
        spawnedBalls.Add(member.id, go);
    }
    public void RemoveVote(DiscordMember member)
    {
        Destroy(spawnedBalls[member.id]);
        spawnedBalls.Remove(member.id);
    }
    IEnumerator LoadImage(string url, RawImage ontoImage)
    {
        url = url.Replace(".webp", ".png");
        var www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            ontoImage.texture = myTexture;
        }

    }
}