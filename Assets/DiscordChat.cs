using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using SimpleJSON;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace DiscordUnityChatDisplay
{

    public class DiscordChat : MonoBehaviour
    {
        public bool initOnStart = true;

        public bool useLocalConnection = false;
        public string connection = "ws://131.217.172.176:3000/chat/{0}";
        string localConnection = "ws://localhost:3000/chat/{0}";
        string Connection
        {
            get
            {
                return useLocalConnection ? localConnection : connection;
            }
        }
        public string channelID;

        public MessageEditEvent latestUpdateEvent;

        public UnityEvent<ChannelUpdateEvent> onChannelUpdate;
        public UnityEvent<MessageCreateEvent> onMessageCreate;
        public UnityEvent<MessageEditEvent> onMessageEdit;
        public UnityEvent<MessageDeleteEvent> onMessageDelete;
        public UnityEvent<ReactionAddEvent> onReactionAdd;
        public UnityEvent<ReactionRemoveEvent> onReactionRemove;
        public UnityEvent<InteractionCreateEvent> onInteractionCreate;

        private WebSocket ws;
        private object l_frames = new object();
        private Queue<string> _frames = new Queue<string>();
        private int _framesLastProcessed = -1;

        private void Start()
        {
            Debug.Log("start "+channelID);
        //}
        //public void Init()
        //{
            Application.runInBackground = true;

            ws = new WebSocket(string.Format(Connection, channelID));
            ws.OnOpen += (sender,e) =>
            {
                Debug.Log("Connected to "+((WebSocket)sender).Url);
            };
            ws.OnError += (sender,e) =>
            {
                Debug.LogError("Error " + e.Message+" "+e.Exception);
            };
            ws.OnClose += (sender, e) =>
            {
                Debug.LogError($"Disconnected from websocket 0.o {e.Code} - {e.Reason} ");
                ws = null;
            };
            ws.OnMessage += OnThreadedMessage;
            ws.Connect();
        }

        
        private void OnThreadedMessage(object sender, MessageEventArgs e)
        {
            lock (l_frames) {
                _frames.Enqueue(e.Data);
            }
        }
        private void DequeueMessages()
        {
            lock (l_frames)
            {
                _framesLastProcessed = _frames.Count;
                while (_frames.Count > 0) ProcessMessage(_frames.Dequeue());
            }
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene(0);
            }

            if(ws == null)
            {
                return;
            }

            DequeueMessages();
        }

        private void ProcessMessage(string message)
        {
            if (message.Contains("ping") == false) Debug.Log(message);
            var json = JSON.Parse(message);

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
                    ws.Send("{origin: 'client', data: null, content: '🏓 PONG!'})");
                    break;

                case "discord":
                    Debug.Log("[DISCORD] "+content+" "+data+" json:"+json);
                    switch(content) {
                        default:
                            Debug.LogWarning("unknown discord mode "+content+" "+data);
                            break;
                        case "channel.update":
                            onChannelUpdate.Invoke(JsonUtility.FromJson<ChannelUpdateEvent>(data));
                            break;
                        case "message.create":
                            onMessageCreate.Invoke(JsonUtility.FromJson<MessageCreateEvent>(data));
                            break;
                        case "message.edit":
                            onMessageEdit.Invoke(JsonUtility.FromJson<MessageEditEvent>(data));
                            break;
                        case "message.delete":
                            onMessageDelete.Invoke(JsonUtility.FromJson<MessageDeleteEvent>(data));
                            break;
                        case "reaction.add":
                            onReactionAdd.Invoke(JsonUtility.FromJson<ReactionAddEvent>(data));
                            break;
                        case "reaction.remove":
                            onReactionRemove.Invoke(JsonUtility.FromJson<ReactionRemoveEvent>(data));
                            break;
                        case "interaction.create":
                            onInteractionCreate.Invoke(JsonUtility.FromJson<InteractionCreateEvent>(data));
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
    [System.Serializable] public class MessageEditEvent
    {
        public string id;
        public DiscordEmbed[] embeds;
    }
    [System.Serializable] public class MessageDeleteEvent {}
    [System.Serializable] public class ReactionAddEvent {}
    [System.Serializable] public class ReactionRemoveEvent {}
    [System.Serializable] public class InteractionCreateEvent 
    {
        public string id;
        public string commandName;
        public string customId;
        public DiscordMember member; 
        public string originalInteractionId;
        public InteractionOption[] options;

        public InteractionOption GetOption(string name)
        {
            if (options == null) return null;
            for (var i = 0; i < options.Length; i++)
            {
                if (options[i].name == name) return options[i];
            }
            return null;
        }
    }

    [System.Serializable]
    public class DiscordMember
    {
        public string id;
        public string name;
        public string color;
        public string avatar;
    }

    [System.Serializable]
    public class InteractionOption
    {
        public string name;
        public string type;
        public string value;
    }
    [System.Serializable]
    public class DiscordEmbed
    {
        public string title;
        public string descripton;
        public DiscordEmbedField[] fields;
    }
    [System.Serializable]
    public class DiscordEmbedField
    {
        public string name;
        public string value;
    }
}