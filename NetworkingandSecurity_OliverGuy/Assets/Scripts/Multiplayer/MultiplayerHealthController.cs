using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MultiplayerHealthController : MonoBehaviour, IPunObservable
{
    [Header("Components")]

    [SerializeField] private Slider healthSlider;

    [Header("Statistics")]

    [SerializeField] private int maxHealth;
    [SerializeField] private bool isEnemy;

    private int _currentHealth;

    public delegate void EnemyKilled();
    public static event EnemyKilled OnEnemyKilled;

    private void Awake()
    {
        _currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = _currentHealth;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_currentHealth);
        }
        else
        {
            _currentHealth = (int)stream.ReceiveNext();
            healthSlider.value = _currentHealth;
        }
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;

        healthSlider.value = _currentHealth;

        if(_currentHealth <= 0)
        {
            ControllerDeath();
        }
    }

    public void ControllerDeath()
    {
        _currentHealth = 100;
        healthSlider.value = _currentHealth;

        if(isEnemy && OnEnemyKilled != null)
        {
            OnEnemyKilled.Invoke();
        }
        else
        {
            Debug.Log("Enemy Killed event was null");
        }
    }
}
