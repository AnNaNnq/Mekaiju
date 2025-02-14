using Mekaiju;
using Mekaiju.AI;
using Mekaiju.LockOnTargetSystem;
using MyBox;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public Transform groundCheck;

    public Transform _cameraPivot;

    private Animator _animator;

    private MechaPlayerActions _playerActions; // NewInputSystem reference
    [SerializeField] private LockOnTargetSystem _lockOnTargetSystem;

    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _scrollAction;

    private Rigidbody _rigidbody;

    
    [Foldout("Movement Attributes")]
    [SerializeField] private float _groundCheckRadius = 0.5f;
    [SerializeField] private float _baseSpeed = 5f;
    [SerializeField] private float _speed;


    [Foldout("Camera Attributes")]
    [SerializeField] private float _mouseSensitivity = 75f; 
    [SerializeField] private float _minVerticalAngle = -30f; 
    [SerializeField] private float _maxVerticalAngle = 80f; 

    [Foldout("Movement Boolean")]
    [SerializeField] bool _isGrounded;


    private MechaInstance _instance;
    private BasicAI       _target;

    private LayerMask _groundLayerMask;

    //Public variables
    public bool isLockedOn = false;
    public float scroll;

    private void Awake()
    {
        _playerActions = new MechaPlayerActions();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        _speed = _baseSpeed;

        _groundLayerMask = LayerMask.GetMask("Walkable");

        _instance = GetComponent<MechaInstance>();

        _cameraPivot = transform.Find("CameraPivot");

        GameObject t_go;
        t_go = GameObject.FindWithTag("Kaiju");
        if (t_go)
        {
            if (t_go.TryGetComponent<BasicAI>(out var t_comp))
            {
                _target = t_comp;
            }
            else
            {
                Debug.Log("Kaiju must have BasicAI inherited component!");
            }
        }
        else
        {
            Debug.Log("Kaiju must have Kaiju tag!");
        }

        t_go = GameObject.FindWithTag("MainCamera");
        if (t_go)
        {
            if (t_go.TryGetComponent<CinemachineCamera>(out var t_comp))
            {
                t_comp.Target.TrackingTarget = _cameraPivot;
            }
            else
            {
                Debug.Log("MainCamera must have CinemachineCamera component!");
            }
        }
        else
        {
            Debug.Log("Scene camera must have MainCamera tag!");
        }
    }

    private void Start()
    {
        _moveAction = _playerActions.Player.Move;
        _lookAction = _playerActions.Player.Look;
        _scrollAction = _playerActions.Player.LockSwitch;

        _playerActions.Player.SwordAttack.performed += OnSwordAttack;
        _playerActions.Player.GunAttack.performed += OnGunAttack;
        _playerActions.Player.Head.performed += OnHead;
        _playerActions.Player.Shield.performed += OnShield;
        _playerActions.Player.Shield.canceled  += OnUnshield;
        _playerActions.Player.Jump.started += OnJump;
        _playerActions.Player.Dash.performed += OnDash;

        _instance.Context.MoveAction = _moveAction;

        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor at the center of the screen
        Cursor.visible = false; // Make the cursor invisible during gameplay
    }

    private void OnEnable()
    {
        _playerActions.Player.Move.Enable();
        _playerActions.Player.Look.Enable();
        _playerActions.Player.LockSwitch.Enable();
        _playerActions.Player.SwordAttack.Enable();
        _playerActions.Player.Head.Enable();
        _playerActions.Player.GunAttack.Enable();
        _playerActions.Player.Shield.Enable();
        _playerActions.Player.Jump.Enable();
        _playerActions.Player.Dash.Enable();

        _playerActions.Player.Lock.performed += OnLock;
        _playerActions.Player.Lock.Enable();

    }

    private void OnDisable()
    {
        _playerActions.Player.Move.Disable();
        _playerActions.Player.Look.Disable();
        _playerActions.Player.LockSwitch.Disable();
        _playerActions.Player.SwordAttack.Disable();
        _playerActions.Player.GunAttack.Disable();
        _playerActions.Player.Head.Disable();
        _playerActions.Player.Shield.Disable();
        _playerActions.Player.Jump.Disable();
        _playerActions.Player.Dash.Disable();
        _playerActions.Player.Lock.Disable();
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

    private void OnHead(InputAction.CallbackContext p_context)
    {
        StartCoroutine(_instance[MechaPart.Head].TriggerDefaultAbility(null, null));
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

    private void OnLock(InputAction.CallbackContext p_context)
    {
        isLockedOn = !isLockedOn;
        _lockOnTargetSystem.ToggleLockOn(isLockedOn);
        if (isLockedOn)
        {
            _lookAction.Disable();
        }
        else
        {
            _lookAction.Enable();
        }
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


        int t_scrollValue = (int)_scrollAction.ReadValue<float>();

        if (t_scrollValue != 0 && isLockedOn) 
        {
            _lockOnTargetSystem.ChangeTarget(t_scrollValue);
        }
    }
    
    private void FixedUpdate()
    {
        Collider[] t_checkGround = Physics.OverlapSphere(groundCheck.position, _groundCheckRadius, _groundLayerMask);
        _isGrounded = t_checkGround.Length > 0;

        if (!_instance.Context.IsMovementOverrided)
        {
            _speed = _instance.Context.Modifiers[ModifierTarget.Speed]?.ComputeValue(_baseSpeed) ?? _baseSpeed;
            // _speed = _baseSpeed * _instance.Context.SpeedModifier;

            if (_isGrounded)
            {
                Vector2 t_moveDir = _moveAction.ReadValue<Vector2>();
                Vector3 t_vel;

                t_vel  = _speed * t_moveDir.y * Time.fixedDeltaTime * transform.forward;
                t_vel += _speed * t_moveDir.x * Time.fixedDeltaTime * transform.right;

                _rigidbody.angularVelocity = Vector3.zero;

                _rigidbody.linearVelocity = new(t_vel.x, _rigidbody.linearVelocity.y, t_vel.z);

                _animator.SetFloat("WalkingSpeed", Mathf.Abs(_rigidbody.linearVelocity.x) + Mathf.Abs(_rigidbody.linearVelocity.z));
            }
            else
            {
                _animator.SetFloat("WalkingSpeed", 0);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, _groundCheckRadius);
    }
}
