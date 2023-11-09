using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    [SerializeField] private float movementSpeed;

    private List<Vector3> _pathList = new List<Vector3>();

    public List<Vector3> PathList
    {
        set { _pathList = value; }
    }

    private void Update()
    {
        if(_pathList.Count > 0)
        {
            PathMovement();
        }
    }

    private void PathMovement()
    {
        float distanceToPoint = Vector3.Distance(this.transform.position, _pathList[0]);

        if(distanceToPoint > 0.25f)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, _pathList[0], Time.deltaTime * movementSpeed);
        }
        else
        {
            _pathList.RemoveAt(0);
        }

        if(_pathList.Count == 0)
        {
            Destroy(this.gameObject);
        }
    }
}
