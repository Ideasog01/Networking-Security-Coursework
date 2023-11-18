using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class MultiplayerLobby : MonoBehaviourPunCallbacks
{
    [Header("Panels")]

    public Transform loginPanel;
    public Transform selectionPanel;
    public Transform createRoomPanel;
    public Transform insideRoomPanel;
    public Transform listRoomsPanel;
    public Transform failedRoomsPanel;

    [Header("Prefabs")]

    public Transform roomEntryPrefab;
    public GameObject playerNamePrefab;

    [Header("Input Fields")]

    public TMP_InputField roomNameInput;
    public TMP_InputField playerNameInput;

    [Header("Parents")]

    public Transform listRoomPanelContent;
    public Transform insideRoomPlayerList;

    [Header("Misc")]

    public string playerName;
    [SerializeField] private GameObject invalidRoomNamePrompt;

    [Header("In-Room Display")]

    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private Button startGameButton;
    [SerializeField] private GameObject waitingForPlayersTextObj;

    private Dictionary<string, RoomInfo> _cachedRoomList;

    private void Start()
    {
        playerNameInput.text = playerName = string.Format("Player {0}", Random.Range(1, 1000000));
        _cachedRoomList = new Dictionary<string, RoomInfo>();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach(var room in roomList)
        {
            if(!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                _cachedRoomList.Remove(room.Name);
            }
            else
            {
                _cachedRoomList[room.Name] = room;
            }
        }
    }

    public void LoginButtonClicked()
    {
        PhotonNetwork.LocalPlayer.NickName = playerName = playerNameInput.text;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void DisconnectButtonClicked()
    {
        PhotonNetwork.Disconnect();
    }

    public void ListRoomsClicked()
    {
        PhotonNetwork.JoinLobby();
    }

    public void LeaveLobbyClicked()
    {
        PhotonNetwork.LeaveLobby();
    }

    public void OnJoinRandomRoomClicked()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void StartGameClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("GameplayScene_Multiplayer");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("We have connected to the master server!");
        ActivatePanel("Selection");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from the master server!");
        ActivatePanel("Login");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room has been created!");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room!");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room has been joined!");
        ActivatePanel("InsideRoom");
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        foreach(var player in PhotonNetwork.PlayerList)
        {
            var playerListEntry = Instantiate(playerNamePrefab, insideRoomPlayerList);
            playerListEntry.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.NickName;
        }

        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Room has been left!");
        ActivatePanel("CreateRoom");
        DestroyChildren(insideRoomPlayerList);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        ActivatePanel("ListRooms");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room Update: " + roomList.Count);

        DestroyChildren(listRoomPanelContent);

        UpdateCachedRoomList(roomList);

        foreach(var room in _cachedRoomList)
        {
            var newRoomEntry = Instantiate(roomEntryPrefab, listRoomPanelContent);
            var newRoomEntryScript = newRoomEntry.GetComponent<RoomEntry>();
            newRoomEntryScript.roomName = room.Key;
            newRoomEntryScript.roomText.text = string.Format("[{0} - ({1}/{2})]", room.Key, room.Value.PlayerCount, room.Value.MaxPlayers);
        }
    }

    public override void OnLeftLobby()
    {
        Debug.Log("Left Lobby");
        DestroyChildren(listRoomPanelContent);
        DestroyChildren(insideRoomPlayerList);
        _cachedRoomList.Clear();
        ActivatePanel("Selection");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("A Player Joined a Room.");
        GameObject playerListEntry = Instantiate(playerNamePrefab, insideRoomPlayerList);
        playerListEntry.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newPlayer.NickName;
        playerListEntry.name = newPlayer.NickName;

        Debug.Log("Player List Length: " + PhotonNetwork.PlayerList.Length);
        startGameButton.interactable = PhotonNetwork.PlayerList.Length > 1;
        waitingForPlayersTextObj.SetActive(PhotonNetwork.PlayerList.Length < 2);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("A Player Left the Room");

        foreach(Transform child in insideRoomPlayerList)
        {
            Destroy(child.gameObject);
        }

        foreach(Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerListEntry = Instantiate(playerNamePrefab, insideRoomPlayerList);
            playerListEntry.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.NickName;
            playerListEntry.name = player.NickName;
        }

        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        startGameButton.interactable = PhotonNetwork.PlayerList.Length > 1;
        waitingForPlayersTextObj.SetActive(PhotonNetwork.PlayerList.Length < 2);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join room. " + message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join random room. " + message);
        ActivatePanel("FailedToJoinRandomRoom");
    }

    public void ActivatePanel(string panelName)
    {
        loginPanel.gameObject.SetActive(false);
        selectionPanel.gameObject.SetActive(false);
        createRoomPanel.gameObject.SetActive(false);
        insideRoomPanel.gameObject.SetActive(false);
        listRoomsPanel.gameObject.SetActive(false);
        failedRoomsPanel.gameObject.SetActive(false);

        if(panelName == loginPanel.gameObject.name)
        {
            loginPanel.gameObject.SetActive(true);
        }
        else if (panelName == selectionPanel.gameObject.name)
        {
            selectionPanel.gameObject.SetActive(true);
        }
        else if (panelName == createRoomPanel.gameObject.name)
        {
            createRoomPanel.gameObject.SetActive(true);
        }
        else if (panelName == insideRoomPanel.gameObject.name)
        {
            insideRoomPanel.gameObject.SetActive(true);
        }
        else if(panelName == listRoomsPanel.gameObject.name)
        {
            listRoomsPanel.gameObject.SetActive(true);
        }
        else if(panelName == failedRoomsPanel.gameObject.name)
        {
            failedRoomsPanel.gameObject.SetActive(true);
        }
    }

    public void CreateARoom()
    {
        if(roomNameInput.text == "" || roomNameInput.text.Length > 30)
        {
            Debug.Log("Invalid Room Name");
            invalidRoomNamePrompt.SetActive(true);
        }
        else
        {
            invalidRoomNamePrompt.SetActive(false);

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
            roomOptions.IsVisible = true;

            PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions);
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void DestroyChildren(Transform parent)
    {
        foreach(Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}
