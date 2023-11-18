using UnityEngine;

namespace Multiplayer
{
    public class MultiplayerCollisionEffects : MonoBehaviour //Collision effects refers to status effects, such as disabling the player
    {
        private MultiplayerCollisionController _collisionController; //The collision controller that handles all collisions (Another component on this object)

        private void Awake()
        {
            _collisionController = this.GetComponent<MultiplayerCollisionController>();
        }

        public void DisableEnemy() //Called by Animation Event
        {
            if (_collisionController.CollisionPlayer != null) //Get the player that the projectile collided with and disable them
            {
                _collisionController.CollisionPlayer.ActivateNetworkFunction("DisablePlayer"); //Disables the selected player on all clients
            }
        }
    }
}

