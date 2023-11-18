using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace Multiplayer
{
    public class MultiplayerLobby : MonoBehaviourPunCallbacks //The script is responsible for setting up rooms for players and allowing them to join new rooms.
    {
        [Header("Panels")]

        //The following panels refer to the different menus for joining/setting up rooms
        [SerializeField] private Transform loginPanel;
        [SerializeField] private Transform selectionPanel;
        [SerializeField] private Transform createRoomPanel;
        [SerializeField] private Transform insideRoomPanel;
        [SerializeField] private Transform listRoomsPanel;
        [SerializeField] private Transform failedRoomsPanel;

        [Header("Prefabs")]

        [SerializeField] private Transform roomEntryPrefab; //For the list of rooms
        [SerializeField] private GameObject playerNamePrefab; //For the list of players inside a given room

        [Header("Input Fields")]

        [SerializeField] private TMP_InputField roomNameInput;
        [SerializeField] private TMP_InputField playerNameInput;

        [Header("Parents")]

        [SerializeField] private Transform listRoomPanelContent; //The parent to use to list the rooms
        [SerializeField] private Transform insideRoomPlayerList;  //The parent to use to list the players inside the room

        [Header("Misc")]

        [SerializeField] private GameObject invalidRoomNamePrompt; //If a room name is invalid, or has not been defined, a warning message will appear informing the player.

        [Header("In-Room Display")]

        [SerializeField] private TextMeshProUGUI roomNameText; //The title of the room, displayed within the room menu itself.
        [SerializeField] private Button startGameButton; //Only available for the host and only interactable when more than one players are inside the room. 
        [SerializeField] private GameObject waitingForPlayersTextObj; //The text that will display when more players are required to start the game. (Only viewable by the master client)

        private Dictionary<string, RoomInfo> _cachedRoomList; //The rooms currently in existance
        private string _playerName; //The player's nickname/username

        private void Start()
        {
            playerNameInput.text = _playerName = string.Format("Player {0}", Random.Range(1, 1000000)); //Set the name to be a random name, that is also editable by the player.
            _cachedRoomList = new Dictionary<string, RoomInfo>();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        #region ButtonFunctions

        public void LoginButtonClicked() //Via Inspector (Button)
        {
            PhotonNetwork.LocalPlayer.NickName = _playerName = playerNameInput.text;
            PhotonNetwork.ConnectUsingSettings();
        }

        public void DisconnectButtonClicked() //Via Inspector (Button)
        {
            PhotonNetwork.Disconnect();
        }

        public void ListRoomsClicked() //Via Inspector (Button)
        {
            PhotonNetwork.JoinLobby();
        }

        public void LeaveLobbyClicked() //Via Inspector (Button)
        {
            PhotonNetwork.LeaveLobby();
        }

        public void OnJoinRandomRoomClicked() //Via Inspector (Button)
        {
            PhotonNetwork.JoinRandomRoom();
        }

        public void StartGameClicked() //Via Inspector (Button)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel("GameplayScene_Multiplayer"); //Loads the scene on all clients within the room
        }

        public void CreateRoomClicked() //Via Inspector (Button)
        {
            if (roomNameInput.text == "" || roomNameInput.text.Length > 30)
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

        public void LeaveRoomClicked() //Via Inspector (Button)
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region Menu Display

        public void ActivatePanel(string panelName)
        {
            //Only enable the menu that is currently being opened. (Close all the others in the case they are active).
            loginPanel.gameObject.SetActive(panelName == loginPanel.gameObject.name);
            selectionPanel.gameObject.SetActive(panelName == selectionPanel.gameObject.name);
            createRoomPanel.gameObject.SetActive(panelName == createRoomPanel.gameObject.name);
            insideRoomPanel.gameObject.SetActive(panelName == insideRoomPanel.gameObject.name);
            listRoomsPanel.gameObject.SetActive(panelName == listRoomsPanel.gameObject.name);
            failedRoomsPanel.gameObject.SetActive(panelName == failedRoomsPanel.gameObject.name);
        }

        public void DestroyChildren(Transform parent) //Destroys all child objects of the given parent.
        {
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }

        #endregion

        #region UpdateRooms

        public void UpdateCachedRoomList(List<RoomInfo> roomList) //Store the rooms that are currently available and are listed in the lobby
        {
            foreach (var room in roomList)
            {
                if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
                {
                    _cachedRoomList.Remove(room.Name); //If the room is no longer available, remove it from the cached list as we no longer want to display it.
                }
                else
                {
                    _cachedRoomList[room.Name] = room;
                }
            }
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("Room Update: " + roomList.Count);

            //When the room list changes, update the display to show all available rooms.

            DestroyChildren(listRoomPanelContent); //Reset the room list display.
            UpdateCachedRoomList(roomList);

            //Creates a new button on the list for each available room.
            foreach (var room in _cachedRoomList)
            {
                var newRoomEntry = Instantiate(roomEntryPrefab, listRoomPanelContent);
                var newRoomEntryScript = newRoomEntry.GetComponent<RoomEntry>();
                newRoomEntryScript.RoomName = room.Key;
                newRoomEntryScript.RoomNameText.text = string.Format("[{0} - ({1}/{2})]", room.Key, room.Value.PlayerCount, room.Value.MaxPlayers); //Display the room name and player count on each button.
            }
        }

        #endregion

        #region DebugOnly

        public override void OnCreatedRoom()
        {
            Debug.Log("Room has been created!");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("Failed to create room!");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("Failed to join room. " + message);
        }

        #endregion

        #region Joining & Leaving

        public override void OnJoinedRoom()
        {
            Debug.Log("Room has been joined!");

            ActivatePanel("InsideRoom");
            startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient); //Only enable the game start button if this is the master client for the room

            //Display the names of the players inside the room.
            foreach (var player in PhotonNetwork.PlayerList)
            {
                var playerListEntry = Instantiate(playerNamePrefab, insideRoomPlayerList);
                playerListEntry.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.NickName;
            }

            //Update room title
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

        public override void OnLeftLobby()
        {
            Debug.Log("Left Lobby");
            DestroyChildren(listRoomPanelContent);
            _cachedRoomList.Clear();
            ActivatePanel("Selection");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer) //When another player enters the room, update the room's player list display.
        {
            Debug.Log("A Player Joined a Room.");

            //Adds a new player to the display
            GameObject playerListEntry = Instantiate(playerNamePrefab, insideRoomPlayerList);
            playerListEntry.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newPlayer.NickName;
            playerListEntry.name = newPlayer.NickName;

            Debug.Log("Player List Length: " + PhotonNetwork.PlayerList.Length);

            //As long as the room has at least two players, the game can start!
            startGameButton.interactable = true;
            waitingForPlayersTextObj.SetActive(false);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log("A Player Left the Room");

            foreach (Transform child in insideRoomPlayerList) //The player list display needs to be reset, as if we only delete the player display object that belongs to the one that left, this could be a problem if the host leaves.
            {
                Destroy(child.gameObject);
            }

            //Populate the player display list with all the players inside the room.
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                GameObject playerListEntry = Instantiate(playerNamePrefab, insideRoomPlayerList);
                playerListEntry.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.NickName;
                playerListEntry.name = player.NickName;
            }

            startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient); //Only display the start game button for master client.
            startGameButton.interactable = PhotonNetwork.PlayerList.Length > 1; //Only enable the start game button if there is more than one players. (Need opponents)
            waitingForPlayersTextObj.SetActive(PhotonNetwork.PlayerList.Length < 2); //If there is not enough players, display the waiting for players object. (Child of start game button, so will only be active for the master client)
        }


        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("Failed to join random room. " + message);
            ActivatePanel("FailedToJoinRandomRoom"); //Gives a description of why joining a room might have failed.
        }

        #endregion

        #region Connecting & Disconnecting

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

        #endregion
    }
}


