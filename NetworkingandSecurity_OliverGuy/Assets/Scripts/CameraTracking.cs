using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    private Vector3 _cameraOffset;

    private void Awake()
    {
        _cameraOffset = this.transform.position - playerTransform.position;
    }

    private void Update()
    {
        this.transform.position = playerTransform.position + _cameraOffset;
    }
}
