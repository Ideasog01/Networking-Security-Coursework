using System.Collections.Generic;
using UnityEngine;

public class MultiplayerCollisionController : MonoBehaviour
{
    [SerializeField] private Transform collisionEfxPrefab;
    [SerializeField] private int collisionDamage;
    [SerializeField] private float objectDuration;
    [SerializeField] private bool destroyOnCollision;

    private Photon.Realtime.Player _owner;

    private List<Collider> _colliderList = new List<Collider>();

    private Collider _ownerCollider;

    public Collider OwnerCollider
    {
        set { _ownerCollider = value; }
    }

    public int CollisionDamage
    {
        get { return collisionDamage; }
    }

    public Photon.Realtime.Player Owner
    {
        get { return _owner; }
        set { _owner = value; }
    }

    private void Awake()
    {
        Destroy(this.gameObject, objectDuration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Projectile"))
        {
            Physics.IgnoreCollision(this.GetComponent<Collider>(), collision.collider);
        }

        if (collision.collider != _ownerCollider && !_colliderList.Contains(collision.collider))
        {
            if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Enemy"))
            {
                collision.collider.GetComponent<MultiplayerHealthController>().TakeDamage(this);
            }

            SpawnBulletEfx();

            _colliderList.Add(collision.collider);

            if (destroyOnCollision)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Physics.IgnoreCollision(this.GetComponent<Collider>(), collision.collider);
            }
        }
    }

    private void SpawnBulletEfx()
    {
        Instantiate(collisionEfxPrefab, this.transform.position, this.transform.rotation);
    }
}
