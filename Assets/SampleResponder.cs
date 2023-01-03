using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using DiscordUnityChatDisplay;

public class SampleResponder : MonoBehaviour
{
    public GameObject testBall;

    public void OnChannelName(ChannelUpdateEvent evt)
    {
        Debug.Log("got channel "+evt.name);
    }
    public void OnInteractionCreate(InteractionCreateEvent evt)
    {
        var go = Instantiate(testBall, transform.position, transform.rotation, transform);
        StartCoroutine(LoadImage(evt.member.avatar, go.GetComponentInChildren<RawImage>()));
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
