using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 7;
    [SerializeField] private float rotationSpeed = 7; //The speed in which this object will rotate towards the mouse cursor

    [SerializeField] private LayerMask environmentLayer; //The layer to decide the end position of the mouse cursor ray

    [SerializeField] private Transform cursorTransform; //An object for purely visual feedback to inform the player of current aiming direction

    [Header("Movement Constraints")]

    //The bounds to keep the player object within the map
    [SerializeField] private float minX;
    [SerializeField] private float minZ;
    [SerializeField] private float maxX;
    [SerializeField] private float maxZ;

    //Player Components
    private Animator _playerAnimator;
    private Rigidbody _playerRb;

    private enum FacingDirection { forward, right, backward, left };
    private FacingDirection _facingDirection; //The current world direction the player is facing towards

    private bool _stopMovement; //If true, the player will not be able to move

    public bool StopMovement
    {
        set { _stopMovement = value; }
        get { return _stopMovement; }
    }

    private void Awake()
    {
        //Assign player components
        _playerRb = this.GetComponent<Rigidbody>();
        _playerAnimator = this.transform.GetChild(0).GetComponent<Animator>();
    }

    #region Movement & Rotation

    public void UpdateMovement()
    {
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0 || _stopMovement)
        {
            MovementAnimations(0, 0);
            return;
        }

        var horizontalInput = Input.GetAxis("Horizontal"); //Right/Left (World)
        var verticalInput = Input.GetAxis("Vertical"); //Forward/Backward (World)

        MovementAnimations(horizontalInput, verticalInput); //Update movement animation based on current facing direction and movement input

        //Apply movement to the player based on user's input
        Vector3 movementDir = new Vector3(horizontalInput, 0, verticalInput) * Time.deltaTime * movementSpeed;
        Vector3 position = _playerRb.position + movementDir;

        //Ensure the player stays within the map bounds
        float clampX = Mathf.Clamp(position.x, minX, maxX);
        float clampZ = Mathf.Clamp(position.z, minZ, maxZ);

        position = new Vector3(clampX, position.y, clampZ);

        //Apply the position after we have ensured the player is still within the map bounds. (This order prevents inconsistent movement at the edge of the world)
        _playerRb.MovePosition(position);
    }

    private void MovementAnimations(float xMove, float zMove) //This function ensures that movement animations match with the player's current local movement. Example: The player is facing right, so 'right is now forward' in local space.
    {
        switch(_facingDirection) //Based on the player's current world facing direction and current movement input, assign the vertical and horizontal values to the player's animator.
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

        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, environmentLayer)) //Calculate the current facing direction by using the mouse cursor as the player's target.
        {
            float mouseDistanceToPlayer = Vector3.Distance(this.transform.position, hit.point); 

            if(mouseDistanceToPlayer > 1) //Ensure that the mouse cursor is not placed on top of the player object. (Avoids unwanted rotation).
            {
                //Calculate the facing direction and update the object's rotation overtime.
                Quaternion lookRotation = Quaternion.LookRotation(hit.point - this.transform.position);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
                this.transform.rotation = Quaternion.Euler(0, this.transform.eulerAngles.y, 0);

                cursorTransform.transform.position = hit.point; //Visual representation of the position the player is aiming at

                //Update the facing direction each time we change the object's rotation.
                float diffZ = this.transform.position.z - hit.point.z;
                float diffX = this.transform.position.x - hit.point.x;

                if(diffZ > diffX) //If the player has moved the cursor more along the z axis, we need to update the lower half of the total rotation.
                {
                    if(diffZ > 0) //We are rotating towards positive Z
                    {
                        _facingDirection = FacingDirection.backward;
                    }
                    else
                    {
                        _facingDirection = FacingDirection.right;
                    }
                }
                else //If the player has moved the cursor more along the z axis, we need to update the upper half of the total rotation.
                {
                    if(diffX > 0) //We are rotating towards positive X
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

    #endregion
}
