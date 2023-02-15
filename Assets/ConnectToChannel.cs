using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectToChannel : MonoBehaviour
{
    public int sceneBuildIndex = 1;
    public void Connect(string channelID)
    {
        StartCoroutine(LoadScene(channelID));
    }
    IEnumerator LoadScene(string channelID)
    {
        var op = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);
        yield return op;
        var chat = FindObjectOfType<DiscordUnityChatDisplay.DiscordChat>();
        chat.channelID = channelID;
        Debug.Log("set channel id "+channelID);
        Destroy(transform.root.gameObject);

    }
}
