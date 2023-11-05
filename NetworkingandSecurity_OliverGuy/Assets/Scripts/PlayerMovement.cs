using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;

    private Rigidbody _playerRb;

    private void Awake()
    {
        _playerRb = this.GetComponent<Rigidbody>();
    }

    public void UpdateMovement()
    {
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            return;
        }

        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");

        var rotation = Quaternion.LookRotation(new Vector3(horizontalInput, 0, verticalInput));
        this.transform.rotation = rotation;

        Vector3 movementDir = this.transform.forward * Time.deltaTime * movementSpeed;
        _playerRb.MovePosition(_playerRb.position + movementDir);
    }
}
