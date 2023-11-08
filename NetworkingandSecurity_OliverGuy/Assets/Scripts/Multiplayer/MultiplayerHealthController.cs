using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MultiplayerHealthController : MonoBehaviour, IPunObservable
{
    [Header("Statistics")]

    [SerializeField] private int maxHealth;
    [SerializeField] private bool isEnemy;

    private int _currentHealth;

    [SerializeField] private Slider healthSlider;

    private bool _isInvulnerable;

    public delegate void EnemyKilled();
    public static event EnemyKilled OnEnemyKilled;

    private PhotonView _photonView;

    public bool IsInvulnerable
    {
        set { _isInvulnerable = value; }
    }

    public Slider HealthSlider
    {
        set { healthSlider = value; }
    }


    private void Awake()
    {
        _photonView = this.GetComponent<PhotonView>();

        if (_photonView.IsMine)
        {
            healthSlider = MultiplayerLevelManager.PlayerHealthSlider;
        }

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

            if(healthSlider != null)
            {
                healthSlider.value = _currentHealth;
            }
        }
    }

    public void TakeDamage(MultiplayerCollisionController collision)
    {
        if(!_isInvulnerable)
        {
            _currentHealth -= collision.CollisionDamage;

            if (healthSlider != null)
            {
                healthSlider.value = _currentHealth;
            }

            if (_currentHealth <= 0)
            {
                if(!_photonView.IsMine)
                {
                    collision.Owner.AddScore(1);
                }
                
                ControllerDeath();
            }
        }
    }

    public void ControllerDeath()
    {
        _currentHealth = 100;

        if(healthSlider != null)
        {
            healthSlider.value = _currentHealth;
        }

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
