using Mekaiju;
using Mekaiju.AI;
using MyBox;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public Transform groundCheck;
    public Transform camera;

    public Transform _cameraPivot;

    private Animator _animator;

    private MechaPlayerActions _playerActions; // NewInputSystem reference

    private InputAction _moveAction;
    private InputAction _lookAction;

    private Rigidbody _rigidbody;
    
    [Foldout("Movement Attributes")]
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _dashForce = 20f;
    [SerializeField] private float _dashDuration = 0.25f;
    [SerializeField] private float _baseSpeed = 5f;
    private Vector3 _dashDirection;
    private float _speed;
    //[SerializeField] private float _cameraSpeed = 5f;

    [Foldout("Movement Boolean")]
    [SerializeField] bool _isGrounded;
    [SerializeField] private bool _isDashing;
    [SerializeField] private bool _isProtected;

    [SerializeField] private float _mouseSensitivity = 75f; 
    [SerializeField] private float _minVerticalAngle = -30f; 
    [SerializeField] private float _maxVerticalAngle = 80f; 

    [Foldout("Stamina Costs")]
    [SerializeField] private float _shieldCost = 2f;
    [SerializeField] private float _jumpCost = 10f;
    [SerializeField] private float _dashCost = 33f;

    private MechaInstance _instance;
    private BasicAI       _target;

    private LayerMask _groundLayerMask;

    private void Awake()
    {
        _playerActions = new MechaPlayerActions();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        _speed = _baseSpeed;

        _groundLayerMask = LayerMask.GetMask("Walkable");

        _instance = GetComponent<MechaInstance>();
        _target   = GameObject.Find("Kaiju").GetComponent<BasicAI>();

        _cameraPivot = transform.Find("CameraPivot");
    }

    private void Start()
    {
        _moveAction = _playerActions.Player.Move;
        _lookAction = _playerActions.Player.Look;

        _playerActions.Player.SwordAttack.performed += OnSwordAttack;
        _playerActions.Player.GunAttack.performed += OnGunAttack;
        _playerActions.Player.Shield.performed += OnShield;
        _playerActions.Player.Shield.canceled  += OnUnshield;
        _playerActions.Player.Jump.started += OnJump;
        _playerActions.Player.Dash.performed += OnDash;

        _instance.Context.MoveAction = _moveAction;
    }

    private void OnEnable()
    {
        _playerActions.Player.Move.Enable();
        _playerActions.Player.Look.Enable();
        _playerActions.Player.SwordAttack.Enable();
        _playerActions.Player.GunAttack.Enable();
        _playerActions.Player.Shield.Enable();
        _playerActions.Player.Jump.Enable();
        _playerActions.Player.Dash.Enable();

    }

    private void OnDisable()
    {
        _playerActions.Player.Move.Disable();
        _playerActions.Player.Look.Disable();
        _playerActions.Player.SwordAttack.Disable();
        _playerActions.Player.GunAttack.Disable();
        _playerActions.Player.Shield.Disable();
        _playerActions.Player.Jump.Disable();
        _playerActions.Player.Dash.Disable();
    }

    private BodyPartObject PickRandomTargetPart()
    {
        if (_target)
        {
            BodyPart t_part;
            do
            {
                t_part = _target.bodyParts[Random.Range(0, _target.bodyParts.Length)];
            }
            while (t_part.isDestroyed == true);
            return t_part.part[Random.Range(0, t_part.part.Length)].GetComponent<BodyPartObject>();
        }
        else
        {
            return null;
        }
    }

    private void OnSwordAttack(InputAction.CallbackContext p_context)
    {
        BodyPartObject t_target = PickRandomTargetPart();
        if (t_target)
        {
            StartCoroutine(_instance[MechaPart.LeftArm].TriggerDefaultAbility(PickRandomTargetPart(), null));
        }
    }
    
    private void OnGunAttack(InputAction.CallbackContext p_context)
    {
        BodyPartObject t_target = PickRandomTargetPart();
        if (t_target)
        {
            StartCoroutine(_instance[MechaPart.RightArm].TriggerDefaultAbility(PickRandomTargetPart(), null));
        }
    }
    
    private void OnShield(InputAction.CallbackContext p_context)
    {
        StartCoroutine(_instance[MechaPart.Chest].TriggerDefaultAbility(null, null));
    }
    
    private void OnUnshield(InputAction.CallbackContext p_context)
    {
        _instance[MechaPart.Chest].ReleaseDefaultAbility();
    }
    
    private void OnJump(InputAction.CallbackContext p_context)
    {
        StartCoroutine(_instance[MechaPart.Legs].TriggerDefaultAbility(null, LegsSelector.Jump));
    }

    private void OnDash(InputAction.CallbackContext p_context)
    {
        StartCoroutine(_instance[MechaPart.Legs].TriggerDefaultAbility(null, LegsSelector.Dash));
    }

    float ClampAngle(float p_angle, float p_from, float p_to)
    {
        // accepts e.g. -80, 80
        if (p_angle < 0f) p_angle = 360 + p_angle;
        if (p_angle > 180f) return Mathf.Max(p_angle, 360 + p_from);
        return Mathf.Min(p_angle, p_to);
    }

    private void Update()
    {
        _instance.Context.IsGrounded = _isGrounded;

        Vector2 t_lookDir = _lookAction.ReadValue<Vector2>() * Time.deltaTime * _mouseSensitivity;

        // Tourner le joueur avec la cam�ra horizontalement
        transform.Rotate(Vector3.up * t_lookDir.x);

        // G�rer la rotation verticale de la cam�ra
        var t_clamp = ClampAngle(_cameraPivot.eulerAngles.x - t_lookDir.y, _minVerticalAngle, _maxVerticalAngle);
        var t_delta = t_clamp - _cameraPivot.eulerAngles.x;
        _cameraPivot.Rotate(Vector3.right * t_delta);
    }
    
    private void FixedUpdate()
    {
        Collider[] t_checkGround = Physics.OverlapSphere(groundCheck.position, 0.3f, _groundLayerMask);
        _isGrounded = t_checkGround.Length > 0;

        if (!_instance.Context.IsMovementOverrided)
        {
            _speed = _baseSpeed * _instance.Context.SpeedModifier;

            Vector2 t_moveDir = _moveAction.ReadValue<Vector2>();
            Vector3 t_vel;

            t_vel  = _speed * t_moveDir.y * Time.fixedDeltaTime * transform.forward;
            t_vel += _speed * t_moveDir.x * Time.fixedDeltaTime * transform.right;

            _rigidbody.angularVelocity = Vector3.zero;

            _rigidbody.linearVelocity = t_vel;
            _animator.SetFloat("WalkingSpeed",Mathf.Abs(t_vel.x)+Mathf.Abs(t_vel.z));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, 0.3f);
    }
}
