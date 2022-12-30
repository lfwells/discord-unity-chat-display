using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleResponder : MonoBehaviour
{
    public void OnChannelName(DiscordUnityChatDisplay.ChannelUpdateEvent evt)
    {
        Debug.Log("got channel "+evt.name);
    }
    public void OnInteractionCreate(DiscordUnityChatDisplay.InteractionCreateEvent evt)
    {
        Debug.Log(evt.customId);
        //Debug.Log(evt.member.name);
    }
}
