using Photon.Pun;
using UnityEngine;

public class MultiplayerLevelManager : MonoBehaviour
{
    private void Start()
    {
        PhotonNetwork.Instantiate("Player_Multiplayer", Vector3.zero, Quaternion.identity);
    }
}
