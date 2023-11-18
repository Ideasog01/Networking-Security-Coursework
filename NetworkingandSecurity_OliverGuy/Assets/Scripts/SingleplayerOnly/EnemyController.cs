using System.Collections;
using UnityEngine;

namespace Singleplayer
{
    public class EnemyController : MonoBehaviour
    {
        [Header("Bullet World Refs")]

        [SerializeField] private Transform projectilePrefab;
        [SerializeField] private Transform projectileSpawn; //The spawn position of the projectile

        [Header("Firing Action Properties")]

        [SerializeField] private float fireCooldownDuration; //The duration of the cooldown. This will limit the enemy's ability to fire a projectile every few seconds.
        [SerializeField] private float projectileSpeed; //The movement speed to apply to the projectile for calculating the velocity.
        [SerializeField] private ParticleSystem projectileShotEfx; //The effect to play when this enemy fires a projectile

        [Header("Disabled")]

        [SerializeField] private ParticleSystem disabledEffect; //The disable effect will play when the player hits this enemy with a 'time blast' projectile

        private Animator _enemyAnimator;

        private Transform _playerTransform;

        private float _fireCooldown; //The current cooldown of the enemy's attack

        private float _enemyDisableTimer; //When this value is larger than zero, the enemy's attack will be disabled.

        private void Awake()
        {
            _enemyAnimator = this.transform.GetChild(0).GetComponent<Animator>();
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            if (_enemyDisableTimer > 0) //The enemy is disabled so cannot attack
            {
                _enemyDisableTimer -= Time.deltaTime * 1;

                if (_enemyDisableTimer <= 0) //When the timer reaches zero, disable the effect
                {
                    Debug.Log("Enemy Enabled");
                    disabledEffect.Stop();
                }
            }
            else
            {
                float distance = Vector3.Distance(this.transform.position, _playerTransform.position);

                if (distance < 12) //If the player is within a range of 12, the enemy can 'see' them
                {
                    //Rotate to face the player's current position
                    this.transform.LookAt(_playerTransform);
                    this.transform.eulerAngles = new Vector3(0, this.transform.eulerAngles.y, 0); //Only rotate on the y axis

                    if (_fireCooldown <= 0) //When the attack is not on cooldown, play the attack animation to fire a projectile that moves in a local forward direction
                    {
                        _enemyAnimator.SetTrigger("Primary");
                        _fireCooldown = fireCooldownDuration;
                        //We do not decrease the cooldown overtime until the projectile is fired. (Need to wait for the animation event)
                    }
                }
            }
        }

        public void FireBullet() //Via Animation Event
        {
            if (_enemyDisableTimer <= 0) //If this enemy is not disabled, fire the projectile (in case the enemy was disabled since the attack animation was triggered)
            {
                //Fire a projectile and apply a forward velocity
                Rigidbody projectileRb = Instantiate(projectilePrefab.GetComponent<Rigidbody>(), projectileSpawn.transform.position, this.transform.rotation);

                projectileRb.transform.forward = this.transform.forward;
                projectileRb.velocity = projectileRb.transform.forward * projectileSpeed;

                projectileShotEfx.Play();

                //Start decreasing the attack cooldown
                StartCoroutine(FireCooldown());
            }
        }

        public void DisableEnemy(float disableTime) //Disables the enemy's attack and display a visual effect to represent the state
        {
            _enemyDisableTimer = disableTime;
            disabledEffect.Play();
            Debug.Log("Enemy Disabled");
        }

        private IEnumerator FireCooldown() //This recursive function decreases the fire cooldown every .1 of a second until the cooldown reaches zero. The enemy can only attack if this cooldown is <= to zero.
        {
            yield return new WaitForSeconds(0.1f);
            _fireCooldown -= 0.1f;

            if (_fireCooldown > 0)
            {
                StartCoroutine(FireCooldown());
            }
        }
    }
}