using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiscordUnityChatDisplay;

public class PollVisuals : MonoBehaviour
{
    public GameObject pollBucketPrefab;

    List<PollBucket> buckets = new List<PollBucket>();

    public void OnPollCreated(PollResponder.Poll poll)
    {
        for (var i = 0; i < poll.answers.Count; i++)
        {
            var answer = poll.answers[i];
            var offset = new Vector3(i * 3, 0, 0);
            var go = Instantiate(pollBucketPrefab, offset, Quaternion.identity, transform);
            buckets.Add(go.GetComponent<PollBucket>());
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
