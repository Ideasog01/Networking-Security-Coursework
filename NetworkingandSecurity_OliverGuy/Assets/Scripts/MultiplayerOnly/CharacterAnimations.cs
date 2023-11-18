using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace Multiplayer
{
    public class CharacterAnimations : MonoBehaviour
    {
        //The events to be called with each ability animation. Allows for abilities to be synced to the character's movements.
        [SerializeField] private UnityEvent primaryEvent;
        [SerializeField] private UnityEvent ability1Event;
        [SerializeField] private UnityEvent ability2Event;
        [SerializeField] private UnityEvent ability3Event;

        //Allows the animation to have control over when movement is enabled for a smoother gameplay experience.
        [SerializeField] private UnityEvent enableMovement;

        private PhotonView _photonView;

        private void Awake()
        {
            _photonView = this.transform.parent.GetComponent<PhotonView>();
        }

        #region AbilityEvents

        //These functions are called by the animation as an event. The invoked events below will have assigned the corresponding ability functions in the player controller script.

        public void Primary()
        {
            if (_photonView.IsMine)
            {
                primaryEvent.Invoke();
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

        #endregion

        public void EnableMovement()
        {
            if (_photonView.IsMine)
            {
                enableMovement.Invoke();
            }
        }
    }
}