using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Singleplayer
{
    public class HealthController : MonoBehaviour
    {
        [Header("Components")]

        [SerializeField] private Slider healthSlider; //The health bar of this entity (For enemies and the player)

        [Header("Statistics")]

        [SerializeField] private int maxHealth;
        [SerializeField] private bool isEnemy; //Does this health controller belong to an enemy (non-player-character)

        [Header("Events")]

        [SerializeField] private UnityEvent onControllerDeathEvent; //The event to invoke when this entity is eliminated.

        private int _currentHealth;
        private bool _isInvulnerable; //If true, the entity will not take damage.

        //Only used for enemies and are assigned via the 'GameManager'.
        public delegate void EnemyKilled();
        public static event EnemyKilled OnEnemyKilled;

        public bool IsInvulnerable
        {
            set { _isInvulnerable = value; }
        }

        //Setup initial display of health
        private void Awake()
        {
            _currentHealth = maxHealth;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = _currentHealth;
        }

        public void TakeDamage(int amount) //Decrease health. If health reaches zero, then eliminate this entity.
        {
            if (!_isInvulnerable)
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

            //For updating the count of enemies (player objective)
            if (isEnemy && OnEnemyKilled != null)
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

}