using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [Header("Bullet World Refs")]

    [SerializeField] private Transform bulletPrefab;
    [SerializeField] private Transform bulletSpawn;

    [Header("Firing Action Properties")]

    [SerializeField] private float fireCooldown;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private ParticleSystem bulletShotEfx;

    private float _fireCooldownDuration;

    private PlayerMovement _playerMovement;

    private void Awake()
    {
        _playerMovement = this.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        //Input

        if(_fireCooldownDuration <= 0 && Input.GetMouseButtonDown(0))
        {
            FireBullet();
        }
    }

    private void FixedUpdate()
    {
        _playerMovement.UpdateMovement();
    }

    private void FireBullet()
    {
        Rigidbody bulletRb = Instantiate(bulletPrefab.GetComponent<Rigidbody>(), bulletSpawn.transform.position, this.transform.rotation);

        bulletRb.transform.forward = this.transform.forward;
        bulletRb.velocity = bulletRb.transform.forward * bulletSpeed;

        bulletRb.GetComponent<CollisionController>().IsPlayerBullet = true;

        bulletShotEfx.Play();

        _fireCooldownDuration = fireCooldown;
        StartCoroutine(FireCooldown());
    }

    private IEnumerator FireCooldown()
    {
        yield return new WaitForSeconds(0.1f);
        _fireCooldownDuration -= 0.1f;

        if(_fireCooldownDuration > 0)
        {
            StartCoroutine(FireCooldown());
        }
    }
}
