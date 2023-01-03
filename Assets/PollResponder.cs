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
        public string[] answers;
        public Dictionary<string, DiscordMember[]> votes;
    }

    public Poll currentPoll;

    public void OnInteractionCreate(DiscordUnityChatDisplay.InteractionCreateEvent evt)
    {
        //if incoming command
        if ("poll" == evt.commandName)
        {
            currentPoll = new Poll {
                interactionId = evt.id
            };
        }
    }
}
