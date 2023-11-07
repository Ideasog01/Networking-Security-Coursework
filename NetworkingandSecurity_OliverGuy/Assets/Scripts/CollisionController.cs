using System.Collections.Generic;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
    [SerializeField] private GameObject collisionEfxPrefab;
    [SerializeField] private int collisionDamage;
    [SerializeField] private float objectDuration;
    [SerializeField] private bool destroyOnCollision;

    private List<Collider> _colliderList = new List<Collider>();

    private Collider _ownerCollider;

    public Collider OwnerCollider
    {
        set { _ownerCollider = value; }
    }

    private void Awake()
    {
        Destroy(this.gameObject, objectDuration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Projectile"))
        {
            Physics.IgnoreCollision(this.GetComponent<Collider>(), collision.collider);
        }

        if(collision.collider != _ownerCollider && !_colliderList.Contains(collision.collider))
        {
            if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Enemy"))
            {
                collision.collider.GetComponent<HealthController>().TakeDamage(collisionDamage);
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
        VisualEffectManager.SpawnVisualEffect(collisionEfxPrefab, this.transform.position, this.transform.rotation, 5);
    }
}
