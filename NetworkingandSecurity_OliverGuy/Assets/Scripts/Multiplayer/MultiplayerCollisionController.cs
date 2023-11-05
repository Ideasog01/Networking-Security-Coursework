using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MultiplayerCollisionController : MonoBehaviour
{
    [SerializeField] private Transform collisionEfxPrefab;
    [SerializeField] private int collisionDamage;

    private bool _isPlayerBullet;

    public bool IsPlayerBullet
    {
        set { _isPlayerBullet = value; }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Player") || collision.collider.CompareTag("Enemy"))
        {
            collision.collider.GetComponent<MultiplayerHealthController>().TakeDamage(collisionDamage);
        }

        SpawnBulletEfx();

        Destroy(this.gameObject);       
    }

    private void SpawnBulletEfx()
    {
        Instantiate(collisionEfxPrefab, this.transform.position, this.transform.rotation);
    }
}
