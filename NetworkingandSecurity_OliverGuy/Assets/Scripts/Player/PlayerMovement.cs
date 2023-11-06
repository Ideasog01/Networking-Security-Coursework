using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 7;
    [SerializeField] private float rotationSpeed = 7;

    private Animator _playerAnimator;
    private Rigidbody _playerRb;

    private void Awake()
    {
        _playerRb = this.GetComponent<Rigidbody>();
        _playerAnimator = this.transform.GetChild(0).GetComponent<Animator>();
    }

    public void UpdateMovement()
    {
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            _playerAnimator.SetFloat("Speed", 0);
            return;
        }

        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDir = new Vector3(horizontalInput, 0, verticalInput) * Time.deltaTime * movementSpeed;
        _playerRb.MovePosition(_playerRb.position + movementDir);

        _playerAnimator.SetFloat("Speed", 1);
        Debug.Log(movementDir.magnitude);
    }

    public void UpdateRotation()
    {
        RaycastHit hit;

        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            float mouseDistanceToPlayer = Vector3.Distance(this.transform.position, hit.point);

            if(mouseDistanceToPlayer > 1)
            {
                if(hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Player"))
                {
                    Quaternion lookRotation = Quaternion.LookRotation(hit.collider.gameObject.transform.position - this.transform.position);
                    this.transform.rotation = Quaternion.Lerp(this.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
                    this.transform.rotation = Quaternion.Euler(0, this.transform.eulerAngles.y, 0);
                }
                else
                {
                    Quaternion lookRotation = Quaternion.LookRotation(hit.point - this.transform.position);
                    this.transform.rotation = Quaternion.Lerp(this.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
                    this.transform.rotation = Quaternion.Euler(0, this.transform.eulerAngles.y, 0);
                }
            }
        }
    }
}
