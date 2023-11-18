using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

namespace Multiplayer
{
    public class MultiplayerChatDisplay : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI chatLogTextPrefab;
        [SerializeField] private Transform chatLogParent; //The parent used for the chat log text objects

        private List<TextMeshProUGUI> _chatLogList = new List<TextMeshProUGUI>(); //The list of messages

        private PhotonView _photonView;

        private void Awake()
        {
            _photonView = this.GetComponent<PhotonView>();
        }

        public void CallNewMessage(string message, bool isLocal)
        {
            if (isLocal) //In the case the message is only for the local player
            {
                DisplayNewMessage(message);
            }
            else //If the message is not local, display it on all clients
            {
                _photonView.RPC("DisplayNewMessage", RpcTarget.AllViaServer, message);
            }
        }

        [PunRPC]
        private void DisplayNewMessage(string message)
        {
            if (_chatLogList.Count > 50) //To avoid too many objects overloading the game, only create and display 50 messages at one time
            {
                List<string> messageList = new List<string>();

                for (int i = 0; i < _chatLogList.Count; i++) //Decrease each message index by one, to remove the earliest/first message that was sent.
                {
                    if (i != 0) //Not the first/earliest message?
                    {
                        TextMeshProUGUI messageText = _chatLogList[i - 1];
                        messageList.Add(messageText.text);
                    }
                }

                for (int j = 0; j < messageList.Count; j++) //Populate the display with all the new messages
                {
                    _chatLogList[j].text = messageList[j];
                }

                _chatLogList[_chatLogList.Count - 1].text = message; //Assign the new message to the last
            }
            else //Instantiate a new text display for the message. The position will automatically update using a vertical layout component.
            {
                TextMeshProUGUI newText = Instantiate(chatLogTextPrefab, chatLogParent);
                newText.text = message;
                _chatLogList.Add(newText);
            }
        }
    }
}
