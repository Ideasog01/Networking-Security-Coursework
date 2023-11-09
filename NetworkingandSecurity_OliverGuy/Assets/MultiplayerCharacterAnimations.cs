using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultiplayerCharacterAnimations : MonoBehaviour
{
    [SerializeField] private UnityEvent primaryEvent;
    [SerializeField] private UnityEvent ability1Event;
    [SerializeField] private UnityEvent ability2Event;
    [SerializeField] private UnityEvent ability3Event;

    [SerializeField] private UnityEvent enableMovement;

    [SerializeField] private PhotonView _photonView;

    private void Awake()
    {
        _photonView = this.transform.parent.GetComponent<PhotonView>();
    }

    public void Primary()
    {
        if(_photonView.IsMine)
        {
            primaryEvent.Invoke();
        }
    }

    public void EnableMovement()
    {
        if (_photonView.IsMine)
        {
            enableMovement.Invoke();
        }
    }

    public void Ability1()
    {
        if (_photonView.IsMine)
        {
            ability1Event.Invoke();
        }
    }

    public void Ability2()
    {
        if (_photonView.IsMine)
        {
            ability2Event.Invoke();
        }
    }

    public void Ability3()
    {
        if (_photonView.IsMine)
        {
            ability3Event.Invoke();
        }
    }
}
