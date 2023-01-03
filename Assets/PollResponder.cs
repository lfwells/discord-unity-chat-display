using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiscordUnityChatDisplay;
using System.Linq;

//current limitations: only the most recent poll will appear, all votes for previous polls will be ignored
//doesn't really handle all the different variations of poll modes

public class PollResponder : MonoBehaviour
{
    const string POLL_COMMAND = "poll";
    const string POLL_COMMAND_QUESTION = "question";
    const string POLL_COMMAND_OPTION_PREFIX = "option_";
    const string POLL_OPTION_PREFIX = "poll_option_";

    [System.Serializable]
    public class Poll
    {
        public string interactionId;
        public string question;
        public List<string> answers = new List<string>();
        public List<List<DiscordMember>> votes = new List<List<DiscordMember>>();
    }

    public Poll currentPoll;

    public void OnInteractionCreate(DiscordUnityChatDisplay.InteractionCreateEvent evt)
    {
        //if incoming command
        if (POLL_COMMAND == evt.commandName)
        {
            currentPoll = new Poll {
                interactionId = evt.id,
                question = evt.GetOption(POLL_COMMAND_QUESTION).value
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

            //TODO: do something in unity for this (via subclass I guess, wait no, lets try composition! so use events i guess)
        }
        else //must have been a button press, or another command
        {
            //check there is an original interaction id (means it was a button/similar)
            if (evt.originalInteractionId == null) return;

            //check that the id of the original interaction matches our poll
            if (evt.originalInteractionId != currentPoll?.interactionId) return;

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
            }
            else
            {
                currentPoll.votes[answerIndex].Add(evt.member);
            }
            Debug.Log("vote count for this one: "+currentPoll.votes[answerIndex].Count);
            
            //TODO: do something in unity for this (via subclass I guess, wait no, lets try composition! so use events i guess)
        }
    }
}
