using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiscordUnityChatDisplay;
using TMPro;

public class PollVisuals : MonoBehaviour
{
    public TMP_Text questionText;

    public GameObject pollBucketPrefab;
    const float BUCKET_WIDTH = 3f;
    const float BUCKET_HEIGHT = 10f - 1f;

    List<PollBucket> buckets = new List<PollBucket>();

    void Start()
    {
        questionText.text = string.Empty;
    }

    public void OnPollCreated(PollResponder.Poll poll)
    {
        var hcentering = poll.answers.Count / 2f * BUCKET_WIDTH / 2f;
        var vcentering = BUCKET_HEIGHT / 2f;

        questionText.text = poll.question;
        questionText.transform.position = new Vector3(0,BUCKET_HEIGHT-vcentering, 0);

        for (var i = 0; i < poll.answers.Count; i++)
        {
            var answer = poll.answers[i];
            var offset = new Vector3(i * BUCKET_WIDTH - hcentering, -vcentering, 0);
            var go = Instantiate(pollBucketPrefab, offset, Quaternion.identity, transform);
            var bucket = go.GetComponent<PollBucket>();
            bucket.Init(answer);
            buckets.Add(bucket);
        }
    }
    public void OnPollDeleted(PollResponder.Poll poll)
    {
        foreach (var bucket in buckets)
            Destroy(bucket.gameObject);
        buckets.Clear();
    }
    public void OnVoteAdded(int answerIndex, DiscordMember member)
    {
        buckets[answerIndex].AddVote(member);
    }
    public void OnVoteRemoved(int answerIndex, DiscordMember member)
    {
        buckets[answerIndex].RemoveVote(member);
    }
}
