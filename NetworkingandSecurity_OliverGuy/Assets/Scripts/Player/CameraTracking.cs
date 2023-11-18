using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    [SerializeField] private Transform targetTransform; //The transform that this camera will follow
    [SerializeField] private float cameraSmoothSpeed = 2f; //The movement speed of the camera.

    [SerializeField] private Vector3 cameraOffset; //The position to add to the location of the player for the object to be in view.

    [Header("Camera Constraints")] //The following constraints will ensure the camera never leaves the map

    [SerializeField] private float minX;
    [SerializeField] private float minZ;
    [SerializeField] private float maxX;
    [SerializeField] private float maxZ;

    public Transform TargetTransform
    {
        set { targetTransform = value; }
    }

    private void FixedUpdate()
    {
        if(targetTransform != null)
        {
            Vector3 targetPosition = targetTransform.position + cameraOffset;

            //Ensure the camera stays within the map bounds
            float clampX = Mathf.Clamp(targetPosition.x, minX, maxX);
            float clampZ = Mathf.Clamp(targetPosition.z, minZ, maxZ);

            targetPosition = new Vector3(clampX, targetPosition.y, clampZ);

            this.transform.position = Vector3.LerpUnclamped(this.transform.position, targetPosition, Time.deltaTime * cameraSmoothSpeed); //Move the camera to the target position overtime for a smooth transition
        }
    }
}
