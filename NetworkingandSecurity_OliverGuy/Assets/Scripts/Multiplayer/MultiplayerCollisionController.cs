using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.UI.GridLayoutGroup;

public class MultiplayerCollisionController : MonoBehaviour
{
    [SerializeField] private Transform collisionEfxPrefab;
    [SerializeField] private int collisionDamage;

    private Photon.Realtime.Player _owner;

    public int CollisionDamage
    {
        get { return collisionDamage; }
    }

    public Photon.Realtime.Player Owner
    {
        get { return _owner; }
        set { _owner = value; }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Player") || collision.collider.CompareTag("Enemy"))
        {
            collision.collider.GetComponent<MultiplayerHealthController>().TakeDamage(this);
        }

        SpawnBulletEfx();

        Destroy(this.gameObject);       
    }

    private void SpawnBulletEfx()
    {
        Instantiate(collisionEfxPrefab, this.transform.position, this.transform.rotation);
    }
}
