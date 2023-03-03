using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using DiscordUnityChatDisplay;
using TMPro;

public class PollBucket : MonoBehaviour
{
    PollVisuals poll;
    string answer;

    public GameObject ballPrefab;
    public Transform spawnPoint;
    public GameObject bottomPanel;
    public TMP_Text answerLabel;

    Dictionary<string, GameObject> spawnedBalls = new Dictionary<string, GameObject>();

    public void Init(PollVisuals poll, string answer)
    {
        this.poll = poll;
        this.answer = answer;

        answerLabel.text = answer;
    }

    public void AddVote(DiscordMember member)
    {
        var go = GameObject.Instantiate(ballPrefab, spawnPoint.position + new Vector3(Random.Range(-0.2f, 0.2f), 0), Quaternion.Euler(0, 0, Random.value * 360f), transform);
        go.transform.localScale = Vector3.one * poll.VoteBallScale;

        ColorUtility.TryParseHtmlString(member.color, out Color memberColor);
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

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            ontoImage.texture = myTexture;
        }

    }

    public void UpdateVoteCountText()
    {
        var count = spawnedBalls.Count;
        var countText = "";
        switch (poll.countType)
        {
            case PollVisuals.CountType.RawCount:
                countText = count.ToString();
                break;
            case PollVisuals.CountType.Percentage:
                countText = (count / (float)poll.TotalVotes * 100f).ToString("0") + "%";
                break;
            case PollVisuals.CountType.CountOutOf:
                countText = count + "/" + poll.TotalVotes;
                break;
            case PollVisuals.CountType.CountOutOfHardCodedNumber:
                countText = count + "/" + poll.hardCodedTotalCount;
                break;
            case PollVisuals.CountType.PercentageOutOfHardCodedNumber:
                countText = (count / (float)poll.hardCodedTotalCount * 100f).ToString("0") + "%";
                break;
        }
        if (poll.countType != PollVisuals.CountType.None)
        {
            answerLabel.text = answer + " (" + countText + ")";
        }
    }

    public void SetBallSize(float size)
    {
        foreach (var ball in spawnedBalls.Values)
        {
            ball.transform.localScale = Vector3.one * size;
        }
    }
}
