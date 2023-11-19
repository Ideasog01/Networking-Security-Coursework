using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    [SerializeField] private float movementSpeed; //The movement speed of the object

    private List<Vector3> _pathList = new List<Vector3>(); //The path to follow

    public List<Vector3> PathList
    {
        set { _pathList = value; }
    }

    private void Update()
    {
        if(_pathList.Count > 0) //If a path has been assigned, move the projectile along the path.
        {
            PathMovement();
        }
    }

    private void PathMovement()
    {
        float distanceToPoint = Vector3.Distance(this.transform.position, _pathList[0]); //Follow the path in chronological order.

        if(distanceToPoint > 0.25f) //If we have not yet reached the current target position
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, _pathList[0], Time.deltaTime * movementSpeed);
        }
        else //If the projectile has reached the current target position, remove it from the list.
        {
            _pathList.RemoveAt(0);
        }

        if(_pathList.Count == 0) //If this projectile has reached the end of the path, immediately destroy it.
        {
            Destroy(this.gameObject);
        }
    }
}
