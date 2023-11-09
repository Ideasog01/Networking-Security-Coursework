using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float cameraSmoothSpeed = 2f;

    [SerializeField] private Vector3 cameraOffset;

    public Transform PlayerTransform
    {
        set { playerTransform = value; }
    }

    private void FixedUpdate()
    {
        if(playerTransform != null)
        {
            Vector3 targetPosition = playerTransform.position + cameraOffset;

            this.transform.position = Vector3.LerpUnclamped(this.transform.position, targetPosition, Time.deltaTime * cameraSmoothSpeed);
        }
    }
}
