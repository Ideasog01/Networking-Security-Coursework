using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float cameraSmoothSpeed = 2f;

    [SerializeField] private Vector3 cameraOffset;

    [Header("Camera Constraints")]

    [SerializeField] private float minX;
    [SerializeField] private float minZ;
    [SerializeField] private float maxX;
    [SerializeField] private float maxZ;

    public float MinX
    {
        get { return minX; }
    }

    public float MinZ
    {
        get { return minZ; }
    }

    public float MaxX
    {
        get { return maxX; }
    }

    public float MaxZ
    {
        get { return maxZ; }
    }

    public Transform PlayerTransform
    {
        set { playerTransform = value; }
    }

    private void FixedUpdate()
    {
        if(playerTransform != null)
        {
            Vector3 targetPosition = playerTransform.position + cameraOffset;

            float clampX = Mathf.Clamp(targetPosition.x, minX, maxX);
            float clampZ = Mathf.Clamp(targetPosition.z, minZ, maxZ);

            targetPosition = new Vector3(clampX, targetPosition.y, clampZ);

            this.transform.position = Vector3.LerpUnclamped(this.transform.position, targetPosition, Time.deltaTime * cameraSmoothSpeed);
        }
    }
}
