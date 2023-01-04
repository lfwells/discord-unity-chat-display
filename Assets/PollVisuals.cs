using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiscordUnityChatDisplay;

public class PollVisuals : MonoBehaviour
{
    public GameObject pollBucketPrefab;

    List<PollBucket> buckets;

    public void OnPollCreated(PollResponder.Poll poll)
    {

    }
    public void OnPollDeleted(PollResponder.Poll poll)
    {

    }
    public void OnVoteAdded(int answerIndex, DiscordMember member)
    {

    }
    public void OnVoteRemoved(int answerIndex, DiscordMember member)
    {

    }
}
