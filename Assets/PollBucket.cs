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
    public Transform leftPanel, rightPanel;
    public TMP_Text answerLabel;

    Dictionary<string, GameObject> spawnedBalls = new Dictionary<string, GameObject>();

    float width = 3;
    public float Width
    {
        get {  return width; }
        set
        {
            width = value;

            //scale the bottomPanel
            bottomPanel.transform.localScale = new Vector3(width, 1, 1);

            //position the leftPanel
            var tmp1 = leftPanel.transform.localPosition;
            tmp1.x = -width * 0.5f + leftPanel.transform.localScale.x * 0.5f;
            leftPanel.transform.localPosition = tmp1;

            //now do the same thing for the rightPanel
            var tmp2 = rightPanel.transform.localPosition;
            tmp2.x = width * 0.5f - rightPanel.transform.localScale.x * 0.5f;
            rightPanel.transform.localPosition = tmp2;

            //also change the width of the answer label
            var tmp = answerLabel.rectTransform.sizeDelta;
            tmp.x = width / (1f / answerLabel.transform.localScale.x) * 20f *3f * 0.5f;
            answerLabel.rectTransform.sizeDelta = tmp;
        }
    }

    public void Init(PollVisuals poll, string answer)
    {
        this.poll = poll;
        this.answer = answer;

        answerLabel.text = answer;

        Width = poll.BUCKET_WIDTH;
    }

    public void AddVote(DiscordMember member)
    {
        var go = GameObject.Instantiate(ballPrefab, spawnPoint.position + new Vector3(Random.Range(-0.2f, 0.2f), 0), Quaternion.Euler(0, 0, Random.value * 360f), transform);
        go.transform.localScale = Vector3.one * poll.VoteBallScale;

        var nameScript = go.GetComponent<PollBallNameHover>();
        nameScript.Name = member.name;
        
        ColorUtility.TryParseHtmlString(member.color, out Color memberColor);
        StartCoroutine(LoadImage(member.avatar, go.GetComponentInChildren<RawImage>(), memberColor));
        spawnedBalls.Add(member.id, go);
    }
    public void RemoveVote(DiscordMember member)
    {
        Destroy(spawnedBalls[member.id]);
        spawnedBalls.Remove(member.id);
    }
    public void ResetVotes()
    {
        foreach (var ball in spawnedBalls.Values)
        {
            Destroy(ball);
        }
        spawnedBalls.Clear();
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
                if (poll.TotalVotes == 0) countText = "0%";
                else countText = (count / (float)poll.TotalVotes * 100f).ToString("0") + "%";
                break;
            case PollVisuals.CountType.CountOutOf:
                countText = count + "/" + poll.TotalVotes;
                break;
            case PollVisuals.CountType.CountOutOfHardCodedNumber:
                countText = count + "/" + poll.hardCodedTotalCount;
                break;
            case PollVisuals.CountType.PercentageOutOfHardCodedNumber:
                if (poll.hardCodedTotalCount == 0) countText = "0%";
                else countText = (count / (float)poll.hardCodedTotalCount * 100f).ToString("0") + "%";
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
