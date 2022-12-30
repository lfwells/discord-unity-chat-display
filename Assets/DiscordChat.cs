using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using SimpleJSON;
using UnityEngine.Events;

namespace DiscordUnityChatDisplay
{

    public class DiscordChat : MonoBehaviour
    {
        public string channelID;

        public UnityEvent<ChannelUpdateEvent> onChannelUpdate;
        public UnityEvent<MessageCreateEvent> onMessageCreate;
        public UnityEvent<MessageEditEvent> onMessageEdit;
        public UnityEvent<MessageDeleteEvent> onMessageDelete;
        public UnityEvent<ReactionAddEvent> onReactionAdd;
        public UnityEvent<ReactionRemoveEvent> onReactionRemove;
        public UnityEvent<InteractionCreateEvent> onInteractionCreate;

        WebSocket ws;
        private void Start()
        {
            ws = new WebSocket("ws://localhost:3000/chat/"+channelID);
            ws.OnOpen += (sender,e) =>
            {
                Debug.Log("Connected to "+((WebSocket)sender).Url);
            };
            ws.OnError += (sender,e) =>
            {
                Debug.LogError("Error " + e.Message);
            };
            ws.OnMessage += OnMessage;
            ws.Connect();
        }
        private void Update()
        {
            if(ws == null)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ws.Send("Hello");
            }  
        }
        private void OnMessage(object sender, MessageEventArgs e)
        {
            var json = JSON.Parse(e.Data);

            //const { origin, data, content } = JSON.parse(event.data);
            string origin = json["origin"];
            string data = json["data"];
            string content = json["content"];
            switch(origin) {
                case "system":
                    Debug.Log("[SERVER] "+content);
                    break;

                case "ping":
                    int time = 0;
                    int.TryParse(data, out time);
                    int latency = (System.DateTime.Now.Millisecond - time);
                    //if (LOG_PING_PONG) console.log(Date.now(), 'PONG 🏓', latency + "ms");
                    ws.Send("{origin: 'client', data: null, content: '🏓 PONG!'})");
                    break;

                case "discord":
                    Debug.Log("[DISCORD] "+content+" "+data);
                    switch(content) {
                        default:
                            Debug.LogWarning("unkown discord mode "+content+" "+data);
                            break;
                        case "channel.update":
                            onChannelUpdate.Invoke(new ChannelUpdateEvent());
                            break;
                        case "message.create":
                            onMessageCreate.Invoke(new MessageCreateEvent());
                            break;
                        case "message.edit":
                            onMessageEdit.Invoke(new MessageEditEvent());
                            break;
                        case "message.delete":
                            onMessageDelete.Invoke(new MessageDeleteEvent());
                            break;
                        case "reaction.add":
                            onReactionAdd.Invoke(new ReactionAddEvent());
                            break;
                        case "reaction.remove":
                            onReactionRemove.Invoke(new ReactionRemoveEvent());
                            break;
                        case "interaction.create":
                            onInteractionCreate.Invoke(new InteractionCreateEvent());
                            break;
                    }
                    break;

                default:
                    Debug.LogWarning("[UNKNOWN] " + origin+" "+data+" "+content);
                    break;

            }
        }
    }

    [System.Serializable] public class ChannelUpdateEvent {}
    [System.Serializable] public class MessageCreateEvent {}
    [System.Serializable] public class MessageEditEvent {}
    [System.Serializable] public class MessageDeleteEvent {}
    [System.Serializable] public class ReactionAddEvent {}
    [System.Serializable] public class ReactionRemoveEvent {}
    [System.Serializable] public class InteractionCreateEvent {}
}