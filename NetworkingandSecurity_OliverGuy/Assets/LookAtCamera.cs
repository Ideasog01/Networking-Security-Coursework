using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        this.transform.LookAt(_cameraTransform.position);
        this.transform.eulerAngles = new Vector3(0, this.transform.eulerAngles.y, 0);
    }
}
