using System.Linq;
using Mekaiju;
using Mekaiju.AI;
using Mekaiju.AI.Body;
using Mekaiju.LockOnTargetSystem;
using MyBox;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Mekaiju.Entity;
using UnityEngine.Animations.Rigging;

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
    private GameObject _aimConstraint;

    [Foldout("Movement Boolean")]
    [SerializeField] bool _isGrounded;

    private MechaInstance _instance;
    private KaijuInstance _target;

    private LayerMask _groundLayerMask;

    //Public variables
    [HideInInspector] public bool isLockedOn = false;
    [HideInInspector] public float scroll;

    private float _timeSinceLastScroll = 0f;

    private void Awake()
    {
        _playerActions = new MechaPlayerActions();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();

        _groundLayerMask = LayerMask.GetMask("Walkable");

        _aimConstraint = transform.FindNested("Aimconstraint").gameObject;

        _instance = GetComponent<MechaInstance>();

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
    }

    private void OnEnable()
    {
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
        BodyPartObject t_target = _lockOnTargetSystem.GetTargetBodyPartObject();
        StartCoroutine(_instance[MechaPart.LeftArm].TriggerAbility(t_target, null));
    }
    
    private void OnRightArm(InputAction.CallbackContext p_context)
    {
        BodyPartObject t_target = _lockOnTargetSystem.GetTargetBodyPartObject();
        StartCoroutine(_instance[MechaPart.RightArm].TriggerAbility(t_target, null));
    }

    private void OnHead(InputAction.CallbackContext p_context)
    {
        StartCoroutine(_instance[MechaPart.Head].TriggerAbility(null, null));
    }
    
    private void OnShield(InputAction.CallbackContext p_context)
    {
        StartCoroutine(_instance.desc.standalones[StandaloneAbility.Shield].behaviour.Trigger(_instance, null, null));
    }
    
    private void OnUnshield(InputAction.CallbackContext p_context)
    {
        _instance.desc.standalones[StandaloneAbility.Shield].behaviour.Release();
    }
    
    private void OnJump(InputAction.CallbackContext p_context)
    {
        StartCoroutine(_instance[MechaPart.Legs].TriggerAbility(null, LegsSelector.Jump));
    }

    private void OnLock(InputAction.CallbackContext p_context)
    {
        isLockedOn = !isLockedOn;
        _lockOnTargetSystem.ToggleLockOn(isLockedOn);
        if (isLockedOn && _lockOnTargetSystem.GetTargetBodyPartObject() != null)
        {
            _lookAction.Disable();
            SetConstraintTarget(_lockOnTargetSystem.GetTargetBodyPartObject().transform);
        }
        else
        {
            _lookAction.Enable();
            SetConstraintTarget(_cameraPivot.GetChild(0));
            isLockedOn = false;
        }
    }

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

    private void SetConstraintTarget(Transform p_newConstraintPos)
    {
        //Head
        _aimConstraint.transform.GetChild(0).GetComponent<MultiAimConstraint>().data.sourceObjects.SetTransform(0, p_newConstraintPos);
        //Chest
        _aimConstraint.transform.GetChild(1).GetComponent<MultiAimConstraint>().data.sourceObjects.SetTransform(0, p_newConstraintPos);
        //RightArm
        _aimConstraint.transform.GetChild(2).GetComponent<MultiAimConstraint>().data.sourceObjects.SetTransform(0, p_newConstraintPos);
        //Elbow
        _aimConstraint.transform.GetChild(2).GetChild(0).GetComponent<MultiAimConstraint>().data.sourceObjects.SetTransform(0, p_newConstraintPos);

    }
    public void SetArmTargeting(bool p_targeting)
    {
        if (p_targeting)
        {
            //RightArm
            _aimConstraint.transform.GetChild(2).GetComponent<MultiAimConstraint>().weight = 0.5f;
            _aimConstraint.transform.GetChild(2).GetChild(0).GetComponent<MultiAimConstraint>().weight = 1;
        }
        else
        {
            //RightArm
            _aimConstraint.transform.GetChild(2).GetComponent<MultiAimConstraint>().weight = 0;
            _aimConstraint.transform.GetChild(2).GetChild(0).GetComponent<MultiAimConstraint>().weight = 0;
        }
    }

    private void Update()
    {
        _instance.states[State.Grounded] = _isGrounded;

        Vector2 t_lookDir = _lookAction.ReadValue<Vector2>() * Time.deltaTime * _mouseSensitivity;

        // Tourner le container du cameraPivot sur l'axe X
        _cameraPivot.parent.Rotate(Vector3.up * t_lookDir.x);

        // tourner le camera pivot sur l'axe Y
        var t_clamp = ClampAngle(_cameraPivot.eulerAngles.x - t_lookDir.y, _minVerticalAngle, _maxVerticalAngle);
        var t_delta = t_clamp - _cameraPivot.eulerAngles.x;
        _cameraPivot.Rotate(Vector3.right * t_delta);

        //tourner le gameobject lorsque l'angle de la camera depasse un certain angle
        float cameraYaw = _cameraPivot.eulerAngles.y;
        float playerYaw = transform.eulerAngles.y;
        float deltaYaw = Mathf.DeltaAngle(playerYaw, cameraYaw);

        if (Mathf.Abs(deltaYaw) > 45f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, cameraYaw, 0), Time.deltaTime * 1f);
        }

        int t_scrollValue = (int)_scrollAction.ReadValue<float>();

        if (t_scrollValue != 0 && isLockedOn && Time.time - _timeSinceLastScroll >= 0.2f) 
        {
            _timeSinceLastScroll = Time.time;
            _lockOnTargetSystem.ChangeTarget(t_scrollValue);
            SetConstraintTarget(_lockOnTargetSystem.GetTargetBodyPartObject().transform);
        }
    }
    
    private void FixedUpdate()
    {
        Collider[] t_checkGround = Physics.OverlapSphere(groundCheck.position, _groundCheckRadius, _groundLayerMask);
        _isGrounded = t_checkGround.Length > 0;

        if (!_instance.states[State.MovementOverrided] && !_instance.states[State.MovementLocked])
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
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, _groundCheckRadius);
    }
}
