using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        if(PhotonNetwork.MasterClient == PhotonNetwork.LocalPlayer)
        {
            StartCoroutine(DelayReload());   
        }
    }

    private IEnumerator DelayReload()
    {
        yield return new WaitForSeconds(2);
        PhotonNetwork.LoadLevel("GameplayScene_Multiplayer"); //Loads the given scene on all clients
    }
}
