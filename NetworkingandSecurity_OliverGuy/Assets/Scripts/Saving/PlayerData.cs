using UnityEngine;
using System;

public class PlayerData
{
    public string uid;
    public string encryptedRequest;
    public string username;
    public int bestScore;
    public string bestScoreDate;
    public int totalPlayersInGame;
    public string roomName;

    public PlayerData()
    {
        uid = Guid.NewGuid().ToString();
        Debug.Log("UID: " + uid + "\nLength: " + uid.Length);
    }
}
