using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiscordUnityChatDisplay;
using TMPro;
using System.Linq;

public class PollVisuals : MonoBehaviour
{
    public enum CountType
    {
        None,
        RawCount,
        CountOutOf,
        Percentage,
    }
    public CountType countType;

    public TMP_Text questionText;

    public GameObject pollBucketPrefab;
    const float BUCKET_WIDTH = 3f;
    const float BUCKET_HEIGHT = 10f - 1f;
    const float BUCKET_SPACING = 0.1f;

    public AnimationCurve voteCountToScaleCurve;
    public int voteCountChunkSize = 5;

    int totalVotes = 0;
    public int TotalVotes
    {
        get { return totalVotes; }
        set { 
            totalVotes = value;
            //apply a staircase effect to totalVotes value, rounding down every 5 votes
            //this is so that the scale of the vote balls doesn't jump around too much
            //for example if there are 10 votes, the totalVotes will be 10, but if there are 11 votes, totalVotes will be 10
            var t = Mathf.FloorToInt((float)totalVotes / voteCountChunkSize) * voteCountChunkSize;

            VoteBallScale = voteCountToScaleCurve.Evaluate(t);
        }
    }

    float voteBallScale = 1f;
    public float VoteBallScale
    {
        get { return voteBallScale; }  
        set 
        { 
            voteBallScale = value; 
            //update the scale of all balls in all buckets
            foreach (var bucket in buckets)
            {
                bucket.SetBallSize(voteBallScale);
            }
        }  
    }

    List<PollBucket> buckets = new List<PollBucket>();

    void Start()
    {
        questionText.text = string.Empty;
    }
    //add a  vote when pressing space
    private void Update()
    {
        //check if shift is held
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKeyDown(KeyCode.P))
            {

                //just doing a fake poll for now
                OnPollCreated(new PollResponder.Poll()
                {
                    question = "test",
                    answers = new List<string>()
                    {
                        "yes", "no"
                    }
                });
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //generate a random hex color
                var color = "#" + ColorUtility.ToHtmlStringRGB(Color.HSVToRGB(Random.Range(0f, 1f), 1f, 1f));

                OnVoteAdded(Random.Range(0, 2), new DiscordMember()
                {
                    avatar = "https://pbs.twimg.com/profile_images/1477596557495078912/QuiPSYnb_400x400.jpg",
                    color = color,
                    name = "test",
                    id = Random.value.ToString()
                });
            }
        }
    }

    public void OnPollCreated(PollResponder.Poll poll)
    {
        var hcentering = (poll.answers.Count - 1 ) * (BUCKET_WIDTH + BUCKET_SPACING*2) / 2f;
        var vcentering = BUCKET_HEIGHT / 2f;

        questionText.text = poll.question;
        questionText.transform.position = new Vector3(0,BUCKET_HEIGHT-vcentering, 0);

        for (var i = 0; i < poll.answers.Count; i++)
        {
            var answer = poll.answers[i];
            var offset = new Vector3(i * BUCKET_WIDTH - hcentering + i * BUCKET_SPACING*2, -vcentering, 0);
            var go = Instantiate(pollBucketPrefab, offset, Quaternion.identity, transform);
            go.transform.SetSiblingIndex(0);
            var bucket = go.GetComponent<PollBucket>();
            bucket.Init(this, answer);
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
        TotalVotes++;
        buckets[answerIndex].AddVote(member);
        buckets.ForEach(b => b.UpdateVoteCountText());
    }
    public void OnVoteRemoved(int answerIndex, DiscordMember member)
    {
        TotalVotes--; 
        buckets[answerIndex].RemoveVote(member);
        buckets.ForEach(b => b.UpdateVoteCountText());
    }
}
