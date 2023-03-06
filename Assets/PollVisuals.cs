using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiscordUnityChatDisplay;
using TMPro;
using System.Linq;
using static PollResponder;

public class PollVisuals : MonoBehaviour
{
    public enum CountType
    {
        None,
        RawCount,
        CountOutOf,
        Percentage,
        CountOutOfHardCodedNumber,
        PercentageOutOfHardCodedNumber
    }
    public CountType countType;
    public int hardCodedTotalCount;

    public TMP_Text questionText;

    public GameObject pollBucketPrefab;
    public float BUCKET_WIDTH = 3f;
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

    bool showTitle = true;
    public bool ShowTitle
    {
        get { return showTitle; }  
        set
        {
            showTitle = value;
            questionText.gameObject.SetActive(showTitle);
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
        else if (Input.GetKeyDown(KeyCode.Space)) 
        {
            //set countType to the next value in the CountType enum, wrapping around
            countType = (CountType)((((int)countType)+1) % System.Enum.GetValues(typeof(CountType)).Length);

            UpdateAllAnswerTexts();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            OnPollDeleted(null);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            ShowTitle = !ShowTitle;
        }
        else if (Input.mouseScrollDelta.y != 0)
        {
            BUCKET_WIDTH += Input.mouseScrollDelta.y;
            buckets.ForEach(b => b.Width = BUCKET_WIDTH);
            RepositionBuckets();
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
        RepositionBuckets();
        UpdateAllAnswerTexts();
    }
    public void OnPollDeleted(PollResponder.Poll poll)
    {
        foreach (var bucket in buckets)
            Destroy(bucket.gameObject);
        buckets.Clear();
        questionText.text = "";
        TotalVotes = 0;
    }
    public void OnVoteAdded(int answerIndex, DiscordMember member)
    {
        TotalVotes++;
        buckets[answerIndex].AddVote(member);
        UpdateAllAnswerTexts();
    }
    public void OnVoteRemoved(int answerIndex, DiscordMember member)
    {
        TotalVotes--; 
        buckets[answerIndex].RemoveVote(member);
        UpdateAllAnswerTexts();
    }
    public void OnPollReset(PollResponder.Poll poll)
    {
        TotalVotes = 0;
        buckets.ForEach(b => b.ResetVotes());
        UpdateAllAnswerTexts();
    }

    void RepositionBuckets()
    {
        var hcentering = (buckets.Count - 1) * (BUCKET_WIDTH + BUCKET_SPACING * 2) / 2f;
        for (var i = 0; i < buckets.Count; i++)
        {
            var bucket = buckets[i];
            var offset = new Vector3(i * BUCKET_WIDTH - hcentering + i * BUCKET_SPACING * 2, bucket.transform.position.y, 0);
            bucket.transform.position = offset;
        }
    }
    public void UpdateAllAnswerTexts()
    {
        buckets.ForEach(b => b.UpdateVoteCountText());
    }
}
