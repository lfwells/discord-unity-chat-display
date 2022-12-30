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
        Queue<ChannelUpdateEvent> channelUpdateQueue = new Queue<ChannelUpdateEvent>(3);
        public UnityEvent<MessageCreateEvent> onMessageCreate;
        Queue<MessageCreateEvent> messageCreateQueue = new Queue<MessageCreateEvent>(3);
        public UnityEvent<MessageEditEvent> onMessageEdit;
        Queue<MessageEditEvent> messageEditQueue = new Queue<MessageEditEvent>(3);
        public UnityEvent<MessageDeleteEvent> onMessageDelete;
        Queue<MessageDeleteEvent> messageDeleteQueue = new Queue<MessageDeleteEvent>(3);
        public UnityEvent<ReactionAddEvent> onReactionAdd;
        Queue<ReactionAddEvent> reactionAddQueue = new Queue<ReactionAddEvent>(3);
        public UnityEvent<ReactionRemoveEvent> onReactionRemove;
        Queue<ReactionRemoveEvent> reactionRemoveQueue = new Queue<ReactionRemoveEvent>(3);
        public UnityEvent<InteractionCreateEvent> onInteractionCreate;
        Queue<InteractionCreateEvent> interactionCreateQueue = new Queue<InteractionCreateEvent>(3);

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
                Debug.LogError("Error " + e.Message+" "+e.Exception);
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

            while (channelUpdateQueue.Count > 0) onChannelUpdate.Invoke(channelUpdateQueue.Dequeue());
            while (messageCreateQueue.Count > 0) onMessageCreate.Invoke(messageCreateQueue.Dequeue());
            while (messageEditQueue.Count > 0) onMessageEdit.Invoke(messageEditQueue.Dequeue());
            while (messageDeleteQueue.Count > 0) onMessageDelete.Invoke(messageDeleteQueue.Dequeue());
            while (reactionAddQueue.Count > 0) onReactionAdd.Invoke(reactionAddQueue.Dequeue());
            while (reactionRemoveQueue.Count > 0) onReactionRemove.Invoke(reactionRemoveQueue.Dequeue());
            while (interactionCreateQueue.Count > 0) onInteractionCreate.Invoke(interactionCreateQueue.Dequeue());
        }
        private void OnMessage(object sender, MessageEventArgs e)
        {
            var json = JSON.Parse(e.Data);

            //const { origin, data, content } = JSON.parse(event.data);
            string origin = json["origin"];
            JSONNode data = json["data"];
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
                            channelUpdateQueue.Enqueue(new ChannelUpdateEvent {
                                name = data["name"]
                            });
                            break;
                        case "message.create":
                            messageCreateQueue.Enqueue(new MessageCreateEvent());
                            break;
                        case "message.edit":
                            messageEditQueue.Enqueue(new MessageEditEvent());
                            break;
                        case "message.delete":
                            messageDeleteQueue.Enqueue(new MessageDeleteEvent());
                            break;
                        case "reaction.add":
                            reactionAddQueue.Enqueue(new ReactionAddEvent());
                            break;
                        case "reaction.remove":
                            reactionRemoveQueue.Enqueue(new ReactionRemoveEvent());
                            break;
                        case "interaction.create":
                            interactionCreateQueue.Enqueue(new InteractionCreateEvent());
                            break;
                    }
                    break;

                default:
                    Debug.LogWarning("[UNKNOWN] " + origin+" "+data+" "+content);
                    break;

            }
        }
    }

    [System.Serializable] public class ChannelUpdateEvent {
        public string name;
    }
    [System.Serializable] public class MessageCreateEvent {}
    [System.Serializable] public class MessageEditEvent {}
    [System.Serializable] public class MessageDeleteEvent {}
    [System.Serializable] public class ReactionAddEvent {}
    [System.Serializable] public class ReactionRemoveEvent {}
    [System.Serializable] public class InteractionCreateEvent {}
}