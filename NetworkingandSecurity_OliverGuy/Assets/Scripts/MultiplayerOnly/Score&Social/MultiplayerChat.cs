using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;
using System.Collections;

namespace Multiplayer
{
    public class MultiplayerChat : MonoBehaviour, IChatClientListener
    {
        public static bool IsChatActive;

        [SerializeField] private string username;
        [SerializeField] private ChatClient chatClient;

        [SerializeField] private TextMeshProUGUI chatText;
        [SerializeField] private TMP_InputField userMessageInput;

        private GameObject _chatDisplayParent;

        private float _hideChatDelay;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            if(_chatDisplayParent == null)
            {
                _chatDisplayParent = this.transform.GetChild(0).gameObject;
                _chatDisplayParent.SetActive(false);
            }
        }

        private void Update()
        {
            if(chatClient != null)
            {
                chatClient.Service();

                ChatInput();

                HideChatCooldown();
            }
        }

        private void HideChatCooldown()
        {
            if(_hideChatDelay > 0)
            {
                _hideChatDelay -= Time.deltaTime * 1;
            }
            else
            {
                DisplayChat(false);
            }
        }

        public void ConnectToChat()
        {
            chatClient = new ChatClient(this);

            var authentication = new AuthenticationValues(PhotonNetwork.LocalPlayer.NickName);
            username = PhotonNetwork.LocalPlayer.NickName;
            chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", authentication);

            DisplayChat(true);
        }

        public void DisconnectFromChat()
        {
            chatClient.Disconnect();
            DisplayChat(false);
        }

        public void BeginHideChatTimer()
        {
            _hideChatDelay = 8;
        }

        public void DisplayChat(bool active)
        {
            if(chatClient != null)
            {
                _chatDisplayParent.SetActive(active);
                IsChatActive = active;

                if(!active) //Hiding the chat
                {
                    userMessageInput.text = "";
                    StopAllCoroutines();
                }
                else
                {
                    userMessageInput.Select();
                    BeginHideChatTimer();
                }
            }
        }

        public void SetMessage() //Sends the message to the chat over the server
        {
            if(userMessageInput.text != "")
            {
                if(chatClient != null)
                {
                    chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, userMessageInput.text);
                    userMessageInput.text = "";
                }
                else
                {
                    Debug.Log("Chat client is null!");
                }
            }
        }

        private void ChatInput()
        {
            if(IsChatActive) //Then the chat is being displayed
            {
                if(Input.GetKeyDown(KeyCode.Return) && userMessageInput.text != "")
                {
                    SetMessage(); //Sends a message
                }
            }
            else if(Input.GetKeyDown(KeyCode.Return))
            {
                DisplayChat(true);
            }
        }

        public void DebugReturn(DebugLevel level, string message)
        {
            Debug.Log("Chat - " + level + " - " + message);
        }

        public void OnChatStateChange(ChatState state)
        {
            Debug.Log("Chat - OnChatStateChange - " + state);
        }

        public void OnConnected()
        {
            Debug.Log("Chat - User: " + username + " has connected");
            chatClient.Subscribe(PhotonNetwork.CurrentRoom.Name, creationOptions: new ChannelCreationOptions() { PublishSubscribers = true });
        }

        public void OnDisconnected()
        {
            Debug.Log("Chat - User: " + username + " has disconnected");
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            ChatChannel currentChat;

            if(chatClient.TryGetChannel(PhotonNetwork.CurrentRoom.Name, out currentChat))
            {
                chatText.text = currentChat.ToStringMessages();
                DisplayChat(true);
            }
        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            
        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
            
        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
            for(int i = 0; i < channels.Length; i++)
            {
                if (results[i])
                {
                    Debug.Log("Chat - Subscribed to " + channels[i] + " channel");
                    chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, " has joined the chat!");
                }
            }
        }

        public void OnUnsubscribed(string[] channels)
        {
            
        }

        public void OnUserSubscribed(string channel, string user)
        {
            
        }

        public void OnUserUnsubscribed(string channel, string user)
        {
           
        }
    }
}
