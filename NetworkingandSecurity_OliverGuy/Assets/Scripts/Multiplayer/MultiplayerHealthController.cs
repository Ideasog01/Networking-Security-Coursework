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

    private Slider _healthSlider;

    private bool _isInvulnerable;

    public delegate void EnemyKilled();
    public static event EnemyKilled OnEnemyKilled;

    public bool IsInvulnerable
    {
        set { _isInvulnerable = value; }
    }


    private void Awake()
    {
        if(this.TryGetComponent(out PhotonView view))
        {
            if(view.IsMine)
            {
                _healthSlider = MultiplayerLevelManager.PlayerHealthSlider;

                _currentHealth = maxHealth;
                _healthSlider.maxValue = maxHealth;
                _healthSlider.value = _currentHealth;
            }
        }
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

            if(_healthSlider != null)
            {
                _healthSlider.value = _currentHealth;
            }
        }
    }

    public void TakeDamage(MultiplayerCollisionController collision)
    {
        if(!_isInvulnerable)
        {
            _currentHealth -= collision.CollisionDamage;

            if (_healthSlider != null)
            {
                _healthSlider.value = _currentHealth;
            }

            if (_currentHealth <= 0)
            {
                collision.Owner.AddScore(1);
                ControllerDeath();
            }
        }
    }

    public void ControllerDeath()
    {
        _currentHealth = 100;

        if(_healthSlider != null)
        {
            _healthSlider.value = _currentHealth;
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
