using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Bullet World Refs")]

    [SerializeField] private Transform bulletPrefab;
    [SerializeField] private Transform bulletSpawn;

    [Header("Firing Action Properties")]

    [SerializeField] private float fireCooldown;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private ParticleSystem bulletShotEfx;

    private float _fireCooldownDuration;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            this.transform.LookAt(other.transform);
            FireBullet();
        }
    }

    private void FireBullet()
    {
        Rigidbody bulletRb = Instantiate(bulletPrefab.GetComponent<Rigidbody>(), bulletSpawn.transform.position, this.transform.rotation);

        bulletRb.transform.forward = this.transform.forward;
        bulletRb.velocity = bulletRb.transform.forward * bulletSpeed;

        bulletShotEfx.Play();

        _fireCooldownDuration = fireCooldown;
        StartCoroutine(FireCooldown());
    }

    private IEnumerator FireCooldown()
    {
        yield return new WaitForSeconds(0.1f);
        _fireCooldownDuration -= 0.1f;

        if (_fireCooldownDuration > 0)
        {
            StartCoroutine(FireCooldown());
        }
    }
}
