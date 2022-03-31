using FishNet.Object;
using UnityEngine;

public class Moving : NetworkBehaviour
{
    public float RotateSpeed = 150f;
    public float MoveSpeed = 5f;
    private CharacterController _characterController;
    private Animating _animating;
    private PlayerInputActions _inputActions;
    private Vector2 _movement = Vector2.zero;
    private bool _jumpPressed = false;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animating = GetComponent<Animating>();
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        _inputActions.Player.Movement.performed += Movement_performed;
        _inputActions.Player.Movement.canceled += Movement_canceled;
        _inputActions.Player.Jump.performed += Jump_performed;
    }

    private void Movement_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _movement = obj.ReadValue<Vector2>();
    }

    private void Movement_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _movement = Vector2.zero;
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _jumpPressed = obj.ReadValueAsButton();
    }

    private void Update()
    {
        if (!base.IsOwner)
            return;

        transform.Rotate(new Vector3(0f, _movement.x * RotateSpeed * Time.deltaTime));
        Vector3 offset = new Vector3(0f, Physics.gravity.y, _movement.y) * (MoveSpeed * Time.deltaTime);
        offset = transform.TransformDirection(offset);

        _characterController.Move(offset);

        bool moving = (_movement.x != 0f || _movement.y != 0f);
        _animating.SetMoving(moving);
        if (_jumpPressed)
        {
            _animating.Jump();
            _jumpPressed = false;
        }
    }
}
