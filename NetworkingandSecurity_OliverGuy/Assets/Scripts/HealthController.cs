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

    private int _currentHealth;

    public delegate void EnemyKilled();
    public static event EnemyKilled OnEnemyKilled;

    private void Awake()
    {
        _currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = _currentHealth;
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
        this.gameObject.SetActive(false);

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
