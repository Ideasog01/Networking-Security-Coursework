using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace Multiplayer
{
    public class LoadingManager : MonoBehaviourPunCallbacks
    {
        private void Start()
        {
            if (PhotonNetwork.MasterClient == PhotonNetwork.LocalPlayer) //We only want to load the gameplay scene when this is the MasterClient (The scene will be loaded across the network)
            {
                StartCoroutine(DelayReload());
            }
        }

        private IEnumerator DelayReload()
        {
            yield return new WaitForSeconds(2); //A delay is required as we need to ensure all players have entered the loading scene. 'PhotonNetwork.LoadLevel' is not instantaneous due to latency.
            PhotonNetwork.LoadLevel("GameplayScene_Multiplayer"); //Loads the given scene on all clients.
        }
    }
}


