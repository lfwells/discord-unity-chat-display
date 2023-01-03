using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiscordUnityChatDisplay;

//current limitations: only the most recent poll will appear, all votes for previous polls will be ignored

public class PollResponder : MonoBehaviour
{
    [System.Serializable]
    public class Poll
    {
        public string interactionId;
        public string question;
        public List<string> answers = new List<string>();
        public Dictionary<string, DiscordMember[]> votes = new Dictionary<string, DiscordMember[]>();
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
                }
            }
        }
    }
}
