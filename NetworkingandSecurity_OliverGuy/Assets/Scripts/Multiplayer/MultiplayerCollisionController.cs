using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultiplayerCollisionController : MonoBehaviour
{
    [SerializeField] private Transform collisionEfxPrefab;
    [SerializeField] private int collisionDamage;
    [SerializeField] private float objectDuration;
    [SerializeField] private UnityEvent collisionEvent;

    private Photon.Realtime.Player _owner;
    private Collider _ownerCollider;

    private Multiplayer _collisionObj;

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

    public Multiplayer CollisionObj
    {
        get { return _collisionObj; }
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

        if (collision.collider != _ownerCollider)
        {
            if (collision.collider.CompareTag("Player"))
            {
                _collisionObj = collision.collider.GetComponent<Multiplayer>();
                collisionEvent.Invoke();
                collision.collider.GetComponent<MultiplayerHealthController>().TakeDamage(this);
            }

            SpawnBulletEfx();

            Destroy(this.gameObject);
        }
    }

    private void SpawnBulletEfx()
    {
        Instantiate(collisionEfxPrefab, this.transform.position, this.transform.rotation);
    }
}
