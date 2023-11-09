using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEffects : MonoBehaviour
{
    [SerializeField] private float disableTimer;

    private CollisionController _collisionController;

    private void Awake()
    {
        _collisionController = this.GetComponent<CollisionController>();
    }

    public void DisableEnemy()
    {
        if(_collisionController.CollisionObj != null)
        {
            _collisionController.CollisionObj.DisableEnemy(disableTimer);
        }
    }
}
