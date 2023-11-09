using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Bullet World Refs")]

    [SerializeField] private Transform bulletPrefab;
    [SerializeField] private Transform bulletSpawn;

    [Header("Firing Action Properties")]

    [SerializeField] private float fireCooldownDuration;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private ParticleSystem bulletShotEfx;

    [Header("Disabled")]

    [SerializeField] private ParticleSystem disabledEffect;

    private Animator _enemyAnimator;

    private Transform _playerTransform;

    private float _fireCooldown;

    private float _enemyDisableTimer;

    private void Awake()
    {
        _enemyAnimator = this.transform.GetChild(0).GetComponent<Animator>();
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if(_enemyDisableTimer > 0) //The enemy is disabled
        {
            _enemyDisableTimer -= Time.deltaTime * 1;
            
            if(_enemyDisableTimer <= 0)
            {
                Debug.Log("Enemy Enabled");
                disabledEffect.Stop();
            }
        }
        else
        {
            float distance = Vector3.Distance(this.transform.position, _playerTransform.position);

            if (distance < 12)
            {
                this.transform.LookAt(_playerTransform);
                this.transform.eulerAngles = new Vector3(0, this.transform.eulerAngles.y, 0);

                if (_fireCooldown <= 0)
                {
                    _enemyAnimator.SetTrigger("Primary");
                    _fireCooldown = fireCooldownDuration;
                }
            }
        }
    }

    public void FireBullet()
    {
        if(_enemyDisableTimer <= 0)
        {
            Rigidbody bulletRb = Instantiate(bulletPrefab.GetComponent<Rigidbody>(), bulletSpawn.transform.position, this.transform.rotation);

            bulletRb.transform.forward = this.transform.forward;
            bulletRb.velocity = bulletRb.transform.forward * bulletSpeed;

            bulletShotEfx.Play();

            StartCoroutine(FireCooldown());
        }
    }

    public void DisableEnemy(float disableTime)
    {
        _enemyDisableTimer = disableTime;
        disabledEffect.Play();
        Debug.Log("Enemy Disabled");
    }

    private IEnumerator FireCooldown()
    {
        yield return new WaitForSeconds(0.1f);
        _fireCooldown -= 0.1f;

        if (_fireCooldown > 0)
        {
            StartCoroutine(FireCooldown());
        }
    }
}
