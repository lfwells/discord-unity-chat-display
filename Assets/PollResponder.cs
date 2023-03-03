using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiscordUnityChatDisplay;
using System.Linq;
using UnityEngine.Events;

//current limitations: only the most recent poll will appear, all votes for previous polls will be ignored
//doesn't really handle all the different variations of poll modes

public class PollResponder : MonoBehaviour
{
    const string POLL_COMMAND = "poll";
    const string POLL_COMMAND_QUESTION = "question";
    const string POLL_COMMAND_OPTION_PREFIX = "option_";
    const string POLL_OPTION_PREFIX = "poll_option_";

    public UnityEvent<Poll> onPollCreated;
    public UnityEvent<Poll> onPollDeleted;
    public UnityEvent<int, DiscordMember> onVoteAdded;
    public UnityEvent<int, DiscordMember> onVoteRemoved;

    [System.Serializable]
    public class Poll
    {
        public string interactionId;
        public string question;
        public List<string> answers = new List<string>();
        public List<List<DiscordMember>> votes = new List<List<DiscordMember>>();

        public bool IsScheduledPoll { get; set; }
    }

    public Poll currentPoll;

    public void OnInteractionCreate(DiscordUnityChatDisplay.InteractionCreateEvent evt)
    {
        //if incoming command
        if (POLL_COMMAND == evt.commandName)
        {
            onPollDeleted.Invoke(currentPoll);

            currentPoll = new Poll {
                interactionId = evt.id,
                question = evt.GetOption(POLL_COMMAND_QUESTION).value,
                IsScheduledPoll = false,
            };
            //go through each of the answers
            for (var i = 1; i <= 16; i++)
            {
                var answer = evt.GetOption(POLL_COMMAND_OPTION_PREFIX+i);
                if (answer != null)
                {
                    currentPoll.answers.Add(answer.value);
                    currentPoll.votes.Add(new List<DiscordMember>());
                }
            }
            if (currentPoll.answers.Count == 0)
            {
                currentPoll.answers.Add("Yes");
                currentPoll.votes.Add(new List<DiscordMember>());
                currentPoll.answers.Add("No");
                currentPoll.votes.Add(new List<DiscordMember>());
            }

            onPollCreated.Invoke(currentPoll);
        }
        else //must have been a button press, or another command
        {
            if (!currentPoll.IsScheduledPoll)
            {
                //check there is an original interaction id (means it was a button/similar)
                if (evt.originalInteractionId == null) return;

                //check that the id of the original interaction matches our poll
                if (evt.originalInteractionId != currentPoll?.interactionId) return;
            }

            //check that it was a poll button option
            if (evt.customId == null || evt.customId.StartsWith(POLL_OPTION_PREFIX) == false) return;

            //get the option index (hopefully valid)
            int answerIndex;
            if (!int.TryParse(evt.customId.Replace(POLL_OPTION_PREFIX, ""), out answerIndex)) return;
            if (answerIndex < 0 || answerIndex >= currentPoll.votes.Count) return;

            //THEN we can update the poll results
            Debug.Log("got a vote action "+evt.member.id+" on "+answerIndex);
            if (currentPoll.votes[answerIndex].RemoveAll(m => m.id == evt.member.id) > 0)
            {
                Debug.Log("already voted, must be unvote");
                onVoteRemoved.Invoke(answerIndex, evt.member);
            }
            else
            {
                currentPoll.votes[answerIndex].Add(evt.member);
                onVoteAdded.Invoke(answerIndex, evt.member);
            }
            Debug.Log("vote count for this one: "+currentPoll.votes[answerIndex].Count);
        }
    }

    //to support scheduled polls, we listen to onmessageeditevent
    public void OnMessageEditEvent(DiscordUnityChatDisplay.MessageEditEvent evt)
    {
        //ignore edit events if the id is the same as our current poll
        if (currentPoll?.interactionId == evt.id) return;

        //we can't determine exactly if an embed is a poll, but we can do our best by checking fields
        if (evt.embeds.Length == 1 && evt.embeds[0].fields.Length > 0)
        {
            onPollDeleted.Invoke(currentPoll);

            var embed = evt.embeds[0];
            currentPoll = new Poll
            {
                interactionId = evt.id,
                question = embed.title,
                IsScheduledPoll = true,
            };

            //go through each of the answers
            for (var i = 0; i < embed.fields.Length; i++)
            {
                var answer = embed.fields[i];
                if (answer != null)
                {
                    currentPoll.answers.Add(answer.name);
                    currentPoll.votes.Add(new List<DiscordMember>());
                }
            }

            onPollCreated.Invoke(currentPoll);
        }
    }
}
