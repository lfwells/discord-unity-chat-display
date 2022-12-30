using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using SimpleJSON;


public class DiscordChat : MonoBehaviour
{
    public string channelID;
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
                        break;
                    case "message.create":
                        break;
                    case "message.edit":
                        break;
                    case "message.delete":
                        break;
                    case "reaction.add":
                    case "reaction.remove":
                        break;
                    case "interaction.create":
                        
                        break;
                }
                break;

            default:
                Debug.LogWarning("[UNKNOWN] " + origin+" "+data+" "+content);
                break;

        }
    }
}
