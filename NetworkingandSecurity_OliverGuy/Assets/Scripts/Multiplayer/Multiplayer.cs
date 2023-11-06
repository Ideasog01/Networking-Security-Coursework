using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Multiplayer : MonoBehaviour
{
    [Header("Bullet World Refs")]

    [SerializeField] private Transform bulletPrefab;
    [SerializeField] private Transform bulletSpawn;

    [Header("Firing Action Properties")]

    [SerializeField] private float fireCooldown;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private ParticleSystem bulletShotEfx;

    private Animator _playerAnimator;

    private PhotonView _photonView;
    private PlayerMovement _playerMovement;

    private float _fireCooldownDuration;

    private void Awake()
    {
        _photonView = this.GetComponent<PhotonView>();
        _playerMovement = this.GetComponent<PlayerMovement>();
        _playerAnimator = this.transform.GetChild(0).GetComponent<Animator>();
    }

    private void Update()
    {
        if(!_photonView.IsMine)
        {
            return;
        }

        _playerMovement.UpdateRotation();

        if(Input.GetMouseButtonDown(0))
        {
            _playerAnimator.SetTrigger("Primary");
        }
    }

    private void FixedUpdate()
    {
        if (!_photonView.IsMine)
        {
            return;
        }

        _playerMovement.UpdateMovement();
    }

    public void PrimaryFire()
    {
        _photonView.RPC("FireBullet", RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void FireBullet()
    {
        if(_fireCooldownDuration <= 0)
        {
            Rigidbody bulletRb = Instantiate(bulletPrefab.GetComponent<Rigidbody>(), bulletSpawn.transform.position, this.transform.rotation);

            bulletRb.transform.forward = this.transform.forward;
            bulletRb.velocity = bulletRb.transform.forward * bulletSpeed;

            bulletRb.GetComponent<MultiplayerCollisionController>().Owner = _photonView.Owner;

            bulletShotEfx.Play();

            _fireCooldownDuration = fireCooldown;
            StartCoroutine(FireCooldown());
        }
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
