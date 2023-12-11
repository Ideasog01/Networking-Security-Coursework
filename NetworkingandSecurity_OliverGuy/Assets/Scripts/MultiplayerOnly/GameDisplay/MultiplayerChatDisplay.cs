using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Chat;

namespace Multiplayer
{
    public class MultiplayerChatDisplay : MonoBehaviourPunCallbacks, IChatClientListener
    {
        [SerializeField] private TextMeshProUGUI chatLogTextPrefab;
        [SerializeField] private Transform chatLogParent; //The parent used for the chat log text objects
        [SerializeField] private TMP_InputField messageInputField;

        private ChatClient _chatClient;

        private List<TextMeshProUGUI> _chatLogList = new List<TextMeshProUGUI>(); //The list of messages

        private PhotonView _photonView;

        private void Awake()
        {
            _photonView = this.GetComponent<PhotonView>();
        }

        private void Start()
        {
            InitialiseChat();
        }

        private void Update()
        {
            _chatClient.Service();
        }

        private void InitialiseChat()
        {
            _chatClient = new ChatClient(this);

            _chatClient.SetOnlineStatus(ChatUserStatus.Online);
            _chatClient.ChatRegion = "EU";
            _chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(PhotonNetwork.LocalPlayer.NickName));
            
            _chatClient.Subscribe(PhotonNetwork.CurrentRoom.Name);
        }

        public void SendMessage() //Via Inspector (Button)
        {
            bool result = _chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, messageInputField.text);
            messageInputField.text = "";
            Debug.Log("Message sent in chat! Result: " + result);
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

        #region IChatClientListener Implementation

        public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
        {
            
        }

        public void OnDisconnected()
        {
            
        }

        public override void OnConnected()
        {
           
        }

        public void OnChatStateChange(ChatState state)
        {
            
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            for(int i = 0; i < senders.Length; i++)
            {
                DisplayNewMessage(senders[i] + ": " + messages[i]);
                Debug.Log("New messages recieved!");
            }
        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            
        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
            Debug.Log("Client subscribed to channels!");
        }

        public void OnUnsubscribed(string[] channels)
        {
            
        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
            
        }

        public void OnUserSubscribed(string channel, string user)
        {
           
        }

        public void OnUserUnsubscribed(string channel, string user)
        {
            
        }

        #endregion
    }
}
