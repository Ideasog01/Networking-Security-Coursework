using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Camera playerCamera;

    private CharacterAnimations _characterAnimations;

    private Rigidbody _playerRb;

    private void Awake()
    {
        _playerRb = this.GetComponent<Rigidbody>();
        _characterAnimations = this.GetComponent<CharacterAnimations>();
    }

    private void Update()
    {
        if (InputManager.EnableRotation)
        {
            UpdateRotation();
        }
    }

    private void FixedUpdate()
    {
        UpdateMovement(InputManager.MovementInput.x, InputManager.MovementInput.y);
    }

    private void UpdateMovement(float horizontalInput, float verticalInput)
    {
        Vector3 movementDir = Vector3.zero;

        if(horizontalInput == 0 && verticalInput != 0)
        {
            movementDir += this.transform.forward * verticalInput;
        }
        else if(verticalInput == 0 && horizontalInput != 0)
        {
            movementDir += this.transform.right * horizontalInput;
        }
        else if(horizontalInput != 0 && verticalInput != 0)
        {
            movementDir += this.transform.forward * verticalInput;
            movementDir += this.transform.right * horizontalInput;
        }

        movementDir *= Time.deltaTime * movementSpeed;

        _playerRb.velocity = movementDir * 100;
    }

    private void UpdateRotation()
    {
        RaycastHit hit;

        float turnAxis = 0;

        if(Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit))
        {
            float distanceToMouse = Vector3.Distance(this.transform.position, hit.point);

            if(distanceToMouse > 2)
            {
                float previousY = this.transform.rotation.y;
                Quaternion lookRotation = Quaternion.LookRotation(hit.point - this.transform.position);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
                this.transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.eulerAngles.y, 0));
                turnAxis = previousY - this.transform.rotation.y;
            }
        }

        _characterAnimations.RotateCharacter(turnAxis);


    }
}
