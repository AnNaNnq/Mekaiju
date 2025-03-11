<<<<<<< HEAD
using System.Collections;
using Mekaiju;
using Mekaiju.AI;
using MyBox;
using Unity.VisualScripting;
=======
using System.Linq;
using Mekaiju;
using Mekaiju.AI;
using Mekaiju.AI.Body;
using Mekaiju.LockOnTargetSystem;
using MyBox;
using Unity.Cinemachine;
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
using UnityEngine;
using UnityEngine.InputSystem;
using Mekaiju.Entity;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public Transform groundCheck;
<<<<<<< HEAD
    public Transform camera;
=======

    public Transform _cameraPivot;
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54

    private Animator _animator;

    private MechaPlayerActions _playerActions; // NewInputSystem reference
<<<<<<< HEAD

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

    [Foldout("Stamina Costs")]
    [SerializeField] private float _shieldCost = 2f;
    [SerializeField] private float _jumpCost = 10f;
    [SerializeField] private float _dashCost = 33f;

    private MechaInstance _instance;
    private Coroutine _shieldStaminaDrainCoroutine;
    private LayerMask _groundLayerMask;

=======
    [SerializeField] private LockOnTargetSystem _lockOnTargetSystem;

    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _scrollAction;

    private Rigidbody _rigidbody;

    
    [Foldout("Movement Attributes")]
    [SerializeField] 
    private float _groundCheckRadius = 0.5f;
    [SerializeField] 
    private float _speedFactor = 5f;
    [SerializeField, ReadOnly] 
    private float _speed;


    [Foldout("Camera Attributes")]
    [SerializeField] [Range(1f,100f)] private float _mouseSensitivity; 
    [SerializeField] private float _minVerticalAngle = -30f; 
    [SerializeField] private float _maxVerticalAngle = 80f; 

    [Foldout("Movement Boolean")]
    [SerializeField] bool _isGrounded;


    private MechaInstance _instance;
    private KaijuInstance _target;

    private LayerMask _groundLayerMask;

    //Public variables
    [HideInInspector] public bool isLockedOn = false;
    [HideInInspector] public float scroll;

    private float _timeSinceLastScroll = 0f;

>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
    private void Awake()
    {
        _playerActions = new MechaPlayerActions();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

<<<<<<< HEAD
        _speed = _baseSpeed;

        _groundLayerMask = LayerMask.GetMask("Walkable");

        _instance = GetComponent<MechaInstance>();
=======
        _groundLayerMask = LayerMask.GetMask("Walkable");

        _instance = GetComponent<MechaInstance>();

        _cameraPivot = transform.Find("CameraPivot");
    }

    private void Start()
    {
        _moveAction = _playerActions.Player.Move;
        _lookAction = _playerActions.Player.Look;
        _scrollAction = _playerActions.Player.LockSwitch;

        _playerActions.Player.LeftArm.performed += OnLeftArm;
        _playerActions.Player.RightArm.performed += OnRightArm;
        _playerActions.Player.Head.performed += OnHead;
        _playerActions.Player.Shield.performed += OnShield;
        _playerActions.Player.Shield.canceled  += OnUnshield;
        _playerActions.Player.Jump.performed += OnJump;
        _playerActions.Player.Dash.performed += OnDash;
        _playerActions.Player.Heal.performed += OnHeal;
        _playerActions.Player.Torse.performed += OnTorse;
        _playerActions.Player.Pause.performed += OnPause;

        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor at the center of the screen
        Cursor.visible = false; // Make the cursor invisible during gameplay

        GameObject t_go;
        t_go = GameObject.Find("CombatManager");
        if (t_go)
        {
            if (t_go.TryGetComponent<CombatManager>(out var t_cm))
            {
                _target = t_cm.kaijuInstance;
            }
            else
            {
                Debug.Log("Combat manager must have CombatManager script!");
            }
        }
        else
        {
            Debug.Log("CombatManager must be in the scene!");
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
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
    }

    private void OnEnable()
    {
<<<<<<< HEAD
        _moveAction = _playerActions.Player.Move;
        _moveAction.Enable();
        _lookAction = _playerActions.Player.Look;
        _lookAction.Enable();

        _playerActions.Player.SwordAttack.performed += OnSwordAttack;
        _playerActions.Player.SwordAttack.Enable();

        _playerActions.Player.GunAttack.performed += OnGunAttack;
        _playerActions.Player.GunAttack.Enable();

        _playerActions.Player.Shield.performed += OnShield;
        _playerActions.Player.Shield.canceled += OnUnshield;
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
        StartCoroutine(_instance[MechaPart.LeftArm].TriggerDefaultAbility(GameObject.Find("Kaiju").GetComponent<TestAI>(), null));
    }
    private void OnGunAttack(InputAction.CallbackContext p_context)
    {
        StartCoroutine(_instance[MechaPart.RightArm].TriggerDefaultAbility(GameObject.Find("Kaiju").GetComponent<TestAI>(), null));
    }
    private void OnShield(InputAction.CallbackContext p_context)
    {
        float t_shieldSpeedModifier = 0.5f;

        _isProtected = true;
        _animator.SetBool("IsShielding", _isProtected);
        _speed = _baseSpeed * t_shieldSpeedModifier;

        // Démarre la consommation de stamina
        if (_shieldStaminaDrainCoroutine != null) StopCoroutine(_shieldStaminaDrainCoroutine);
        _shieldStaminaDrainCoroutine = StartCoroutine(ShieldStaminaDrain());
    }
    private void OnUnshield(InputAction.CallbackContext p_context)
    {
        _isProtected = false;
        _animator.SetBool("IsShielding", _isProtected);
        _speed = _baseSpeed;

        // Arrête la consommation de stamina
        if (_shieldStaminaDrainCoroutine != null)
        {
            StopCoroutine(_shieldStaminaDrainCoroutine);
            _shieldStaminaDrainCoroutine = null;
        }
    }

    private IEnumerator ShieldStaminaDrain()
    {
        while (_isProtected && _instance.CanExecuteAbility(_shieldCost))
        {
            // Consomme 2 points de stamina par seconde
            _instance.ConsumeStamina(2f);
            _instance.Context.LastAbilityTime = Time.time;

            // Si la stamina est épuisée, désactive le bouclier
            if (!_instance.CanExecuteAbility(_shieldCost))
            {
                OnUnshield(new InputAction.CallbackContext());
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void OnJump(InputAction.CallbackContext p_context)
    {
        StartCoroutine(_instance[MechaPart.Legs].TriggerDefaultAbility(null, null));
    }

    private void OnDash(InputAction.CallbackContext p_context)
    {
        if (!_isDashing && !_isProtected && _instance.CanExecuteAbility(_dashCost))
        {
            // determine direction du dash basé sur la direction de deplacement
            Vector2 moveInput = _moveAction.ReadValue<Vector2>();
            //_dashDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            _dashDirection = moveInput.y *  transform.forward;
            _dashDirection += moveInput.x * transform.right;
            _dashDirection = _dashDirection.normalized;

            // Si aucune input, pas de dash
            if (_dashDirection.sqrMagnitude == 0f)
            {
                //CHANGER ICI POUR CHANGER LE COMPORTEMENT QUAND LE JOUEUR DASH SANS DIRECTION
                return;
            }

            _instance.ConsumeStamina(_dashCost);
            _instance.Context.LastAbilityTime = Time.time;
            StartCoroutine(DashCoroutine());
        }
    }

<<<<<<< HEAD
=======
=======
        _playerActions.Player.Move.Enable();
        _playerActions.Player.Look.Enable();
        _playerActions.Player.LockSwitch.Enable();
        _playerActions.Player.LeftArm.Enable();
        _playerActions.Player.Head.Enable();
        _playerActions.Player.RightArm.Enable();
        _playerActions.Player.Shield.Enable();
        _playerActions.Player.Jump.Enable();
        _playerActions.Player.Dash.Enable();
        _playerActions.Player.Heal.Enable();
        _playerActions.Player.Torse.Enable();
        _playerActions.Player.Pause.Enable();

        _playerActions.Player.Lock.performed += OnLock;
        _playerActions.Player.Lock.Enable();

    }

    private void OnDisable()
    {
        _playerActions.Player.Move.Disable();
        _playerActions.Player.Look.Disable();
        _playerActions.Player.LockSwitch.Disable();
        _playerActions.Player.LeftArm.Disable();
        _playerActions.Player.RightArm.Disable();
        _playerActions.Player.Head.Disable();
        _playerActions.Player.Shield.Disable();
        _playerActions.Player.Jump.Disable();
        _playerActions.Player.Dash.Disable();
        _playerActions.Player.Lock.Disable();
        _playerActions.Player.Heal.Disable();
        _playerActions.Player.Torse.Disable();
        _playerActions.Player.Pause.Disable();
    }

    private BodyPartObject PickRandomTargetPart()
    {
        if (_target)
        {
            BodyPart[] t_parts = _target.bodyParts.Where(t_part => !t_part.isDestroyed).ToArray();
            if (t_parts.Length > 0)
            {
                BodyPart   t_part  = t_parts[Random.Range(0, t_parts.Length)];
                if (t_part != null)
                    return t_part.part[Random.Range(0, t_part.part.Length)].GetComponent<BodyPartObject>();
            }
        }
        return null;
    }

    private void OnLeftArm(InputAction.CallbackContext p_context)
    {
        BodyPartObject t_target = PickRandomTargetPart();
        if (t_target)
        {
            StartCoroutine(_instance[MechaPart.LeftArm].TriggerAbility(PickRandomTargetPart(), null));
        }
    }
    
    private void OnRightArm(InputAction.CallbackContext p_context)
    {
        BodyPartObject t_target = PickRandomTargetPart();
        if (t_target)
        {
            StartCoroutine(_instance[MechaPart.RightArm].TriggerAbility(PickRandomTargetPart(), null));
        }
    }

    private void OnHead(InputAction.CallbackContext p_context)
    {
        StartCoroutine(_instance[MechaPart.Head].TriggerAbility(null, null));
    }
    
    private void OnShield(InputAction.CallbackContext p_context)
    {
        StartCoroutine(_instance.shieldAbility.behaviour.Trigger(_instance, null, null));
    }
    
    private void OnUnshield(InputAction.CallbackContext p_context)
    {
        _instance.shieldAbility.behaviour.Release();
    }
    
    private void OnJump(InputAction.CallbackContext p_context)
    {
        StartCoroutine(_instance[MechaPart.Legs].TriggerAbility(null, LegsSelector.Jump));
    }

>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
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

<<<<<<< HEAD
>>>>>>> b1b3882 (blocked mouse input when locked)
    private IEnumerator DashCoroutine()
    {
        _isDashing = true;
        yield return new WaitForSeconds(_dashDuration);
        _isDashing = false;
    }

    private void FixedUpdate()
    {
        Collider[] t_checkGround = Physics.OverlapSphere(groundCheck.position, 0.3f, _groundLayerMask);
        _isGrounded = t_checkGround.Length > 0;

        _instance.Context.IsGrounded = _isGrounded;

        if (_isDashing)
        {
            // Maintain dash velocity while preserving gravity
            Vector3 newVelocity = _dashDirection * _dashForce;
            newVelocity.y = _rigidbody.linearVelocity.y;
            _rigidbody.linearVelocity = newVelocity;
        }
        else
        {
            // Regular movement
            Vector2 t_moveDir = _moveAction.ReadValue<Vector2>();
            Vector3 t_vel = _rigidbody.linearVelocity;
            t_vel.x = _speed * t_moveDir.x;
            t_vel.z = _speed * t_moveDir.y;
            _rigidbody.linearVelocity = t_vel;
            _animator.SetFloat("WalkingSpeed",Mathf.Abs(t_vel.x)+Mathf.Abs(t_vel.z));
        }
<<<<<<< HEAD

        Vector2 t_lookDir = _lookAction.ReadValue<Vector2>();
        //Debug.Log($"look: {t_lookDir}");
=======
>>>>>>> b1b3882 (blocked mouse input when locked)
=======
    private void OnDash(InputAction.CallbackContext p_context)
    {
        StartCoroutine(_instance[MechaPart.Legs].TriggerAbility(null, LegsSelector.Dash));
    }

    private void OnHeal(InputAction.CallbackContext p_context)
    {
        //a faire
    }
    private void OnTorse(InputAction.CallbackContext p_context)
    {
        // a faire
    }
    private void OnPause(InputAction.CallbackContext p_context)
    {
        // a faire
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
        _instance.states[State.Grounded] = _isGrounded;

        Vector2 t_lookDir = _lookAction.ReadValue<Vector2>() * Time.deltaTime * _mouseSensitivity;

        // Tourner le joueur avec la camï¿½ra horizontalement
        transform.Rotate(Vector3.up * t_lookDir.x);

        // Gï¿½rer la rotation verticale de la camï¿½ra
        var t_clamp = ClampAngle(_cameraPivot.eulerAngles.x - t_lookDir.y, _minVerticalAngle, _maxVerticalAngle);
        var t_delta = t_clamp - _cameraPivot.eulerAngles.x;
        _cameraPivot.Rotate(Vector3.right * t_delta);


        int t_scrollValue = (int)_scrollAction.ReadValue<float>();

        if (t_scrollValue != 0 && isLockedOn && Time.time - _timeSinceLastScroll >= 0.2f) 
        {
            _timeSinceLastScroll = Time.time;
            _lockOnTargetSystem.ChangeTarget(t_scrollValue);
        }
    }
    
    private void FixedUpdate()
    {
        Collider[] t_checkGround = Physics.OverlapSphere(groundCheck.position, _groundCheckRadius, _groundLayerMask);
        _isGrounded = t_checkGround.Length > 0;

        if (!_instance.states[State.MovementOverrided] && !_instance.states[State.Stun])
        {
            _speed = _instance.ComputedStatistics(Statistics.Speed) * _speedFactor;

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
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
<<<<<<< HEAD
        Gizmos.DrawWireSphere(groundCheck.position, 0.3f);
=======
        Gizmos.DrawWireSphere(groundCheck.position, _groundCheckRadius);
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
    }
}
