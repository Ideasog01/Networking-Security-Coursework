using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    [Header("Components")]

    [SerializeField] private Slider healthSlider;

    [Header("Statistics")]

    [SerializeField] private int maxHealth;
    [SerializeField] private bool isEnemy;

    [Header("Events")]

    [SerializeField] private UnityEvent onControllerDeathEvent;

    private int _currentHealth;
    private bool _isInvulnerable;

    public delegate void EnemyKilled();
    public static event EnemyKilled OnEnemyKilled;

    public bool IsInvulnerable
    {
        set { _isInvulnerable = value; }
    }

    private void Awake()
    {
        _currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = _currentHealth;
    }

    public void TakeDamage(int amount)
    {
        if(!_isInvulnerable)
        {
            _currentHealth -= amount;

            healthSlider.value = _currentHealth;

            if (_currentHealth <= 0)
            {
                ControllerDeath();
            }
        }
    }

    public void ControllerDeath()
    {
        onControllerDeathEvent.Invoke();

        if(isEnemy && OnEnemyKilled != null)
        {
            OnEnemyKilled.Invoke();
        }
        else
        {
            Debug.Log("Enemy Killed event was null");
        }

        this.gameObject.SetActive(false);
    }
}
