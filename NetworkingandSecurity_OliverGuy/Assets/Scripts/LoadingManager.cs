using Photon.Pun;
using UnityEngine;

public class LoadingManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.LoadLevel("GameplayScene_Multiplayer");
    }
}
