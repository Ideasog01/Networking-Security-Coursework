using UnityEngine;
using UnityEngine.Events;

namespace Singleplayer
{
    public class CharacterAnimations : MonoBehaviour
    {
        [SerializeField] private UnityEvent primaryEvent;
        [SerializeField] private UnityEvent ability1Event;
        [SerializeField] private UnityEvent ability2Event;
        [SerializeField] private UnityEvent ability3Event;

        [SerializeField] private UnityEvent enableMovement;

        //These functions are called by the animation as an event. The invoked events below will have assigned the corresponding ability functions in the player controller script.

        public void Primary()
        {
            primaryEvent.Invoke();
        }

        public void EnableMovement()
        {
            enableMovement.Invoke();
        }

        public void Ability1()
        {
            ability1Event.Invoke();
        }

        public void Ability2()
        {
            ability2Event.Invoke();
        }

        public void Ability3()
        {
            ability3Event.Invoke();
        }
    }
}
