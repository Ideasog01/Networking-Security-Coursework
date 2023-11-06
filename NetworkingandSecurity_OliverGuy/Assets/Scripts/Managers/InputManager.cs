using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static Vector2 MovementInput;
    public static bool EnableRotation;

    [SerializeField] private PlayerActions playerActions;

    private PlayerInputActions _playerInputActions;

    private void Awake()
    {
        InitialiseInput();
    }

    private void Update()
    {
        MovementInput = _playerInputActions.Gameplay.Movement.ReadValue<Vector2>();
    }

    private void InitialiseInput()
    {
        _playerInputActions = new PlayerInputActions();

        if (playerActions != null)
        {
            _playerInputActions.Gameplay.Primary.performed += ctx => playerActions.PrimaryActionTrigger();
            _playerInputActions.Gameplay.Ab1.performed += ctx => playerActions.Ability1ActionTrigger();
            _playerInputActions.Gameplay.Ab2.performed += ctx => playerActions.Ability2ActionTrigger();
            _playerInputActions.Gameplay.Secondary.performed += ctx => EnableRotation = !EnableRotation;
        }
    }

    private void OnEnable()
    {
        _playerInputActions.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
    }
}
