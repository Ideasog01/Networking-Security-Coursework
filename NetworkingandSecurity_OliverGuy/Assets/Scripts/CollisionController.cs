using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionController : MonoBehaviour
{
    [SerializeField] private GameObject collisionEfxPrefab;
    [SerializeField] private int collisionDamage;
    [SerializeField] private float objectDuration;
    [SerializeField] private UnityEvent collisionEvent;

    private List<Collider> _colliderList = new List<Collider>();

    private Collider _ownerCollider;

    private EnemyController _collisionObj;

    public Collider OwnerCollider
    {
        set { _ownerCollider = value; }
    }

    public EnemyController CollisionObj
    {
        get { return _collisionObj; }
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

        if (collision.collider != _ownerCollider && !_colliderList.Contains(collision.collider))
        {
            if(collision.collider.CompareTag("Enemy"))
            {
                _collisionObj = collision.collider.GetComponent<EnemyController>();
            }

            if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Enemy"))
            {
                collision.collider.GetComponent<HealthController>().TakeDamage(collisionDamage);
            }

            collisionEvent.Invoke();

            SpawnBulletEfx();

            _colliderList.Add(collision.collider);

            Destroy(this.gameObject);
        }  
    }

    private void SpawnBulletEfx()
    {
        VisualEffectManager.SpawnVisualEffect(collisionEfxPrefab, this.transform.position, this.transform.rotation, 5);
    }
}
