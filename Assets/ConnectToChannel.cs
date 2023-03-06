using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectToChannel : MonoBehaviour
{
    public int sceneBuildIndex = 1;

    //pre-fill the channel id from playerpref in the text box 
    private void Start()
    {
        var input = GetComponentInChildren<TMPro.TMP_InputField>();
        input.text = PlayerPrefs.GetString("channelID");
    }

    public void Connect(string channelID)
    {
        StartCoroutine(LoadScene(channelID));
    }
    IEnumerator LoadScene(string channelID)
    {
        //save the channel id to playerprefs
        PlayerPrefs.SetString("channelID", channelID);

        var op = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);
        yield return op;

        var chat = FindObjectOfType<DiscordUnityChatDisplay.DiscordChat>();
        chat.channelID = channelID;

        Debug.Log("set channel id "+channelID);
        Destroy(transform.root.gameObject);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
