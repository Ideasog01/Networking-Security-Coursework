using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 7;
    [SerializeField] private float rotationSpeed = 7;

    [SerializeField] private LayerMask environmentLayer;

    [SerializeField] private Transform cursorTransform;

    private Animator _playerAnimator;
    private Rigidbody _playerRb;
    private bool _stopMovement;

    public bool StopMovement
    {
        set { _stopMovement = value; }
        get { return _stopMovement; }
    }

    private void Awake()
    {
        _playerRb = this.GetComponent<Rigidbody>();
        _playerAnimator = this.transform.GetChild(0).GetComponent<Animator>();
    }

    public void UpdateMovement()
    {
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0 || _stopMovement)
        {
            MovementAnimations(0, 0);
            return;
        }

        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");

        MovementAnimations(horizontalInput, verticalInput);

        Vector3 movementDir = new Vector3(horizontalInput, 0, verticalInput) * Time.deltaTime * movementSpeed;
        _playerRb.MovePosition(_playerRb.position + movementDir);
    }

    enum FacingDirection { forward, right, backward, left };
    private FacingDirection _facingDirection;

    private void MovementAnimations(float xMove, float zMove)
    {
        switch(_facingDirection)
        {
            case FacingDirection.forward:
                _playerAnimator.SetFloat("Horizontal", xMove);
                _playerAnimator.SetFloat("Vertical", zMove);
                break;
            case FacingDirection.right:
                _playerAnimator.SetFloat("Horizontal", zMove);
                _playerAnimator.SetFloat("Vertical", xMove);
                break;
            case FacingDirection.backward:
                _playerAnimator.SetFloat("Horizontal", -xMove);
                _playerAnimator.SetFloat("Vertical", -zMove);
                break;
            case FacingDirection.left:
                _playerAnimator.SetFloat("Horizontal", -zMove);
                _playerAnimator.SetFloat("Vertical", -xMove);
                break;
        }
    }

    public void UpdateRotation()
    {
        RaycastHit hit;

        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, environmentLayer))
        {
            float mouseDistanceToPlayer = Vector3.Distance(this.transform.position, hit.point);

            if(mouseDistanceToPlayer > 1)
            {
                Quaternion lookRotation = Quaternion.LookRotation(hit.point - this.transform.position);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
                this.transform.rotation = Quaternion.Euler(0, this.transform.eulerAngles.y, 0);

                if(cursorTransform != null)
                {
                    cursorTransform.transform.position = hit.point;
                }

                float diffZ = this.transform.position.z - hit.point.z;
                float diffX = this.transform.position.x - hit.point.x;

                if(diffZ > diffX)
                {
                    if(diffZ > 0)
                    {
                        _facingDirection = FacingDirection.backward;
                    }
                    else
                    {
                        _facingDirection = FacingDirection.right;
                    }
                }
                else
                {
                    if(diffX > 0)
                    {
                        _facingDirection = FacingDirection.left;
                    }
                    else
                    {
                        _facingDirection = FacingDirection.forward;
                    }
                }
            }
        }
    }
}
