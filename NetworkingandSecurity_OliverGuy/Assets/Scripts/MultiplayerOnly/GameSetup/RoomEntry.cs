using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Multiplayer
{
    public class RoomEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roomNameText; //Displays the name of the room and the room's current player count
        [SerializeField] private string roomName;

        public TextMeshProUGUI RoomNameText
        {
            get { return roomNameText; }
        }

        public string RoomName
        {
            set { roomName = value; }
        }

        public void JoinRoom() //Via Inspector (Button)
        {
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.JoinRoom(roomName); //Joins the room the player clicked on from the room list display
        }
    }
}