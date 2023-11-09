using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerCollisionEffects : MonoBehaviour
{
    [SerializeField] private float disableTimer;

    private MultiplayerCollisionController _collisionController;

    private void Awake()
    {
        _collisionController = this.GetComponent<MultiplayerCollisionController>();
    }

    public void DisableEnemy()
    {
        if (_collisionController.CollisionObj != null)
        {
            _collisionController.CollisionObj.ActivateAbility("DisablePlayer");
        }
    }
}
