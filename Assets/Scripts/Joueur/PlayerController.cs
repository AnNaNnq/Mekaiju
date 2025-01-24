using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public Transform groundCheck;
    public Transform camera;

    private MechaPlayerActions _playerActions;

    private InputAction _moveAction;
    private InputAction _lookAction;

    private Rigidbody _rigidbody;

    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _cameraSpeed = 5f;
    private bool _isGrounded;


    private LayerMask _groundLayerMask;

    private void Awake()
    {
        _playerActions = new MechaPlayerActions();
        _rigidbody = GetComponent<Rigidbody>();

        _groundLayerMask = LayerMask.GetMask("Ground");
    }

    private void OnEnable()
    {
        _moveAction = _playerActions.Player.Move;
        _moveAction.Enable();
        _lookAction = _playerActions.Player.Look;
        _lookAction.Enable();

        _playerActions.Player.SwordAttack.performed += OnSwordAttack;
        _playerActions.Player.SwordAttack.Enable();

        _playerActions.Player.GunAttack.performed += OnGunAttack;
        _playerActions.Player.GunAttack.Enable();

        _playerActions.Player.Shield.performed += OnShield;
        _playerActions.Player.Shield.Enable();

        _playerActions.Player.Jump.started += OnJump;
        //_playerActions.Player.Jump.performed += OnHover;
        //_playerActions.Player.Jump.canceled += OnStopHover;
        _playerActions.Player.Jump.Enable();

        _playerActions.Player.Dash.performed += OnDash;
        _playerActions.Player.Dash.Enable();

    }
    private void OnDisable()
    {
        _moveAction.Disable();
        _lookAction.Disable();
        _playerActions.Player.SwordAttack.Disable();
        _playerActions.Player.GunAttack.Disable();
        _playerActions.Player.Shield.Disable();
        _playerActions.Player.Jump.Disable();
        _playerActions.Player.Dash.Disable();
    }

    private void OnSwordAttack(InputAction.CallbackContext p_context)
    {
        Debug.Log("SwordAttack");
    }
    private void OnGunAttack(InputAction.CallbackContext p_context)
    {
        Debug.Log("GunAttack");
    }
    private void OnShield(InputAction.CallbackContext p_context)
    {
        Debug.Log("Shield");
    }
    private void OnJump(InputAction.CallbackContext p_context)
    {
        if (_isGrounded)
        {
            _isGrounded = false;
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Acceleration);
        }
    }
    //private void OnHover(InputAction.CallbackContext p_context)
    //{
    //    Debug.Log("Hover");
    //}
    //private void OnStopHover(InputAction.CallbackContext p_context)
    //{
    //    Debug.Log("StopHover");
    //}
    private void OnDash(InputAction.CallbackContext p_context)
    {
        Debug.Log("Dash");
    }

    private void FixedUpdate()
    {
        if (!_isGrounded)
        {
            _isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, 0.05f, _groundLayerMask);
        }

        Vector2 t_moveDir = _moveAction.ReadValue<Vector2>();
        Vector3 t_vel = _rigidbody.linearVelocity;
        t_vel.x = _speed * t_moveDir.x;
        t_vel.z = _speed * t_moveDir.y;
        _rigidbody.linearVelocity = t_vel;

        Vector2 t_lookDir = _lookAction.ReadValue<Vector2>();
        //Debug.Log($"look: {t_lookDir}");
    }
}
