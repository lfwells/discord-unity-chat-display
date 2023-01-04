using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using DiscordUnityChatDisplay;
using TMPro;

public class PollBucket : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform spawnPoint;
    public GameObject bottomPanel;
    public TMP_Text answerLabel;

    Dictionary<string, GameObject> spawnedBalls = new Dictionary<string, GameObject>();

    public void Init(string answer)
    {
        answerLabel.text = answer;
    }

    public void AddVote(DiscordMember member)
    {
        var go = GameObject.Instantiate(ballPrefab, spawnPoint.position, Quaternion.Euler(0,0,Random.value*360f), transform);
        Color memberColor = Color.white;
        ColorUtility.TryParseHtmlString(member.color, out memberColor);
        StartCoroutine(LoadImage(member.avatar, go.GetComponentInChildren<RawImage>(), memberColor));
        spawnedBalls.Add(member.id, go);
    }
    public void RemoveVote(DiscordMember member)
    {
        Destroy(spawnedBalls[member.id]);
        spawnedBalls.Remove(member.id);
    }
    IEnumerator LoadImage(string url, RawImage ontoImage, Color originalColor)
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
