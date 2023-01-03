using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiscordUnityChatDisplay;

//current limitations: only the most recent poll will appear, all votes for previous polls will be ignored
//doesn't really handle all the different variations of poll modes

public class PollResponder : MonoBehaviour
{
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
        if ("poll" == evt.commandName)
        {
            currentPoll = new Poll {
                interactionId = evt.id,
                question = evt.GetOption("question").value
            };
            //go through each of the answers
            for (var i = 1; i <= 16; i++)
            {
                var answer = evt.GetOption("option_"+i);
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

            //THEN we can update the poll results
            Debug.Log("got a vote "+evt.member.id+" on "+evt.customId);
        }
    }
}
