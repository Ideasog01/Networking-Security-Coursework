using UnityEngine;
using UnityEngine.Events;

namespace Multiplayer
{
    public class CollisionController : MonoBehaviour //Responsible for detecting collisions
    {
        [SerializeField] private Transform collisionEfxPrefab; //The prefab instantiated for visual effect when collision occurs
        [SerializeField] private int collisionDamage; //The damage to apply to the object if another player is hit
        [SerializeField] private float objectDuration; //The object will be destroyed automatically after this time has elapsed (in seconds)
        [SerializeField] private UnityEvent collisionEvent; //The event to call when the collision occurs. (Only works if we collide with another player)

        private Photon.Realtime.Player _owner; //The player that launched this projectile
        private Collider _ownerCollider;
        private PlayerController _collisionPlayer;

        #region Properties

        public Collider OwnerCollider
        {
            set { _ownerCollider = value; }
            get { return _ownerCollider; }
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

        public PlayerController CollisionPlayer
        {
            get { return _collisionPlayer; }
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
                return;
            }

            //Did the projectile hit another player?
            if (collision.collider != _ownerCollider)
            {
                if (collision.collider.CompareTag("Player"))
                {
                    collisionEvent.Invoke(); //Potential collision events could include applying disabling effects, or enabling visual effects
                    collision.collider.GetComponent<HealthController>().TakeDamage(this);
                }

                //Display collision effect
                Instantiate(collisionEfxPrefab, this.transform.position, this.transform.rotation);

                Destroy(this.gameObject);
            }
        }
    }
}