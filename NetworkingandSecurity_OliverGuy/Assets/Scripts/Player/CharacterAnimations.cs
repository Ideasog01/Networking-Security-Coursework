using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterAnimations : MonoBehaviour
{
    [SerializeField] private UnityEvent primaryEvent;

    public void Primary()
    {
        primaryEvent.Invoke();
    }
}
