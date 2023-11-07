using UnityEngine;
using UnityEngine.Events;

public class CharacterAnimations : MonoBehaviour
{
    [SerializeField] private UnityEvent primaryEvent;
    [SerializeField] private UnityEvent ability1Event;
    [SerializeField] private UnityEvent ability2Event;

    [SerializeField] private UnityEvent enableMovement;

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
}
