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
        Queue<string> channelUpdateQueue = new Queue<string>(3);
        public UnityEvent<MessageCreateEvent> onMessageCreate;
        Queue<string> messageCreateQueue = new Queue<string>(3);
        public UnityEvent<MessageEditEvent> onMessageEdit;
        Queue<string> messageEditQueue = new Queue<string>(3);
        public UnityEvent<MessageDeleteEvent> onMessageDelete;
        Queue<string> messageDeleteQueue = new Queue<string>(3);
        public UnityEvent<ReactionAddEvent> onReactionAdd;
        Queue<string> reactionAddQueue = new Queue<string>(3);
        public UnityEvent<ReactionRemoveEvent> onReactionRemove;
        Queue<string> reactionRemoveQueue = new Queue<string>(3);
        public UnityEvent<InteractionCreateEvent> onInteractionCreate;
        Queue<string> interactionCreateQueue = new Queue<string>(3);

        WebSocket ws;
        private void Start()
        {
            Application.runInBackground = true;

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

            while (channelUpdateQueue.Count > 0) onChannelUpdate.Invoke(JsonUtility.FromJson<ChannelUpdateEvent>(channelUpdateQueue.Dequeue()));
            while (messageCreateQueue.Count > 0) onMessageCreate.Invoke(JsonUtility.FromJson<MessageCreateEvent>(messageCreateQueue.Dequeue()));
            while (messageEditQueue.Count > 0) onMessageEdit.Invoke(JsonUtility.FromJson<MessageEditEvent>(messageEditQueue.Dequeue()));
            while (messageDeleteQueue.Count > 0) onMessageDelete.Invoke(JsonUtility.FromJson<MessageDeleteEvent>(messageDeleteQueue.Dequeue()));
            while (reactionAddQueue.Count > 0) onReactionAdd.Invoke(JsonUtility.FromJson<ReactionAddEvent>(reactionAddQueue.Dequeue()));
            while (reactionRemoveQueue.Count > 0) onReactionRemove.Invoke(JsonUtility.FromJson<ReactionRemoveEvent>(reactionRemoveQueue.Dequeue()));
            while (interactionCreateQueue.Count > 0) onInteractionCreate.Invoke(JsonUtility.FromJson<InteractionCreateEvent>(interactionCreateQueue.Dequeue()));
        }
        private void OnMessage(object sender, MessageEventArgs e)
        {
            var json = JSON.Parse(e.Data);

            //const { origin, data, content } = JSON.parse(event.data);
            string origin = json["origin"];
            string data = json["data"].ToString();
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
                    Debug.Log("[DISCORD] "+content+" "+data+" json:"+json);
                    switch(content) {
                        default:
                            Debug.LogWarning("unknown discord mode "+content+" "+data);
                            break;
                        case "channel.update":
                            channelUpdateQueue.Enqueue(data);
                            break;
                        case "message.create":
                            messageCreateQueue.Enqueue(data);
                            break;
                        case "message.edit":
                            messageEditQueue.Enqueue(data);
                            break;
                        case "message.delete":
                            messageDeleteQueue.Enqueue(data);
                            break;
                        case "reaction.add":
                            reactionAddQueue.Enqueue(data);
                            break;
                        case "reaction.remove":
                            reactionRemoveQueue.Enqueue(data);
                            break;
                        case "interaction.create":
                            interactionCreateQueue.Enqueue(data);
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
    [System.Serializable] public class InteractionCreateEvent {
        public string id;
        public string commandName;
        public string customId;
        public DiscordMember member; 
    }

    [System.Serializable]
    public class DiscordMember
    {
        public string id;
        public string name;
        public string color;
        public string avatar;
    }
}