using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform _cameraTransform; //The main camera transform. (Target to rotate towards)

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void Update() //Rotates the object to face the main camera every frame. (Mostly used for in-world UI objects)
    {
        this.transform.LookAt(_cameraTransform.position);
        this.transform.eulerAngles = new Vector3(0, this.transform.eulerAngles.y, 0);
    }
}
