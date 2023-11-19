using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Singleplayer
{
    public class CollisionController : MonoBehaviour //Responsible for detecting collisions
    {
        [SerializeField] private GameObject collisionEfxPrefab; //The prefab instantiated for visual effect when collision occurs
        [SerializeField] private int collisionDamage; //The damage to apply to the object if another player is hit
        [SerializeField] private float objectDuration; //The object will be destroyed automatically after this time has elapsed (in seconds)
        [SerializeField] private UnityEvent collisionEvent; //The event to call when the collision occurs. (Only works if we collide with another player)

        private Collider _ownerCollider; //The owner of this projectile. (Could be the player or an enemy).

        private EnemyController _collisionObj; //The enemy the player hit with this projectile. (For 'time blast' ability)

        #region Properties

        public Collider OwnerCollider
        {
            set { _ownerCollider = value; }
        }

        public EnemyController CollisionObj
        {
            get { return _collisionObj; }
        }

        #endregion

        private void Awake()
        {
            Destroy(this.gameObject, objectDuration);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Projectile")) //Ignore other projectiles
            {
                Physics.IgnoreCollision(this.GetComponent<Collider>(), collision.collider);
            }

            //Did the projectile hit another player?
            if (collision.collider != _ownerCollider)
            {
                if (collision.collider.CompareTag("Enemy"))
                {
                    _collisionObj = collision.collider.GetComponent<EnemyController>();
                }

                if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Enemy"))
                {
                    collision.collider.GetComponent<HealthController>().TakeDamage(collisionDamage);
                }

                collisionEvent.Invoke(); //Potential collision events could include applying disabling effects, or enabling visual effects.

                VisualEffectManager.SpawnVisualEffect(collisionEfxPrefab, this.transform.position, this.transform.rotation, 5);

                Destroy(this.gameObject);
            }
        }
    }
}