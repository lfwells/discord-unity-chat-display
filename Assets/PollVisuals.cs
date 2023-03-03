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
    const float BUCKET_SPACING = 0.1f;

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

                OnVoteAdded(Random.Range(0, 1), new DiscordMember()
                {
                    avatar = "https://mylo.utas.edu.au/content/enforced/570649-AW_KCT_23S1_31948_0_0_1_0_1/Unit%20Overview/File_e4f8cc7f60e642c09012be4bcb9ada2c_image.png?_&d2lSessionVal=1t6D74BnavrUVgxvXADTztVT1&ou=570649",
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
