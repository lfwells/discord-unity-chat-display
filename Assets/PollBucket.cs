using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiscordUnityChatDisplay;

public class PollBucket : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform spawnPoint;
    public GameObject bottomPanel;
    Dictionary<DiscordMember, GameObject> spawnedBalls = new Dictionary<DiscordMember, GameObject>();

    public void AddVote(DiscordMember member)
    {
        var go = GameObject.Instantiate(ballPrefab, spawnPoint.position, Quaternion.Euler(0,0,Random.value*360f), transform);
        spawnedBalls.Add(member, go);
    }
    public void RemoveVote(DiscordMember member)
    {
        Destroy(spawnedBalls[member]);
    }
}
