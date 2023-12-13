using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;

namespace Multiplayer
{
    public class MultiplayerChat : MonoBehaviour, IChatClientListener
    {
        public string username;
        public ChatClient chatClient;

        public TextMeshProUGUI chatText;
        public TMP_InputField userMessageInput;

        private void Start()
        {
            chatClient = new ChatClient(this);
        }

        private void Update()
        {
            chatClient.Service();
        }

        public void SetMessage()
        {
            if(userMessageInput.text != "")
            {
                chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, userMessageInput.text);
                userMessageInput.text = "";
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
