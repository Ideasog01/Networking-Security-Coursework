using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class MultiplayerChatDisplay : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI chatLogTextPrefab;
    [SerializeField] private Transform chatLogParent;

    private List<TextMeshProUGUI> _chatLogList = new List<TextMeshProUGUI>();

    private PhotonView _photonView;

    private void Awake()
    {
        _photonView = this.GetComponent<PhotonView>();
    }

    public void CallNewMessage(string message, Color color, bool isLocal)
    {
        if(isLocal)
        {
            DisplayNewMessage(message);
        }
        else
        {
            _photonView.RPC("DisplayNewMessage", RpcTarget.AllViaServer, message);
        }
    }

    [PunRPC]
    private void DisplayNewMessage(string message)
    {
        if(_chatLogList.Count > 50)
        {
            List<string> messageList = new List<string>();
            List<Color> colorList = new List<Color>();

            for(int i = 0; i < _chatLogList.Count; i++)
            {
                if(i != 0)
                {
                    TextMeshProUGUI messageText = _chatLogList[i - 1];
                    messageList.Add(messageText.text);
                    colorList.Add(messageText.color);
                }
            }

            for(int j = 0; j < messageList.Count; j++)
            {
                _chatLogList[j].text = messageList[j];
                _chatLogList[j].color = colorList[j];
            }

            _chatLogList[_chatLogList.Count - 1].text = message;
            //_chatLogList[_chatLogList.Count - 1].color = color;
        }
        else
        {
            TextMeshProUGUI newText = Instantiate(chatLogTextPrefab, chatLogParent);
            newText.text = message;
            //newText.color = color;
            _chatLogList.Add(newText);
        }
    }
}
