using System.Collections;
using Mekaiju;
using Mekaiju.AI;
using MyBox;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

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

    public VisualEffect shieldVFX;
    public VisualEffect shieldBreakVFX;
    
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
    private Coroutine _shieldStaminaDrainCoroutine;
    private LayerMask _groundLayerMask;

    private void Awake()
    {
        _playerActions = new MechaPlayerActions();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        _speed = _baseSpeed;

        _groundLayerMask = LayerMask.GetMask("Walkable");

        _instance = GetComponent<MechaInstance>();

        _cameraPivot = transform.Find("CameraPivot");
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
        Debug.Log("SwordAttack");
        StartCoroutine(_instance.ExecuteAbility(MechaPart.LeftArm, GameObject.Find("Kaiju").GetComponent<TestAI>(), null));
        _animator.SetTrigger("swordAttack");
    }
    
    private void OnGunAttack(InputAction.CallbackContext p_context)
    {
        Debug.Log("GunAttack");
        StartCoroutine(_instance.ExecuteAbility(MechaPart.RightArm, GameObject.Find("Kaiju").GetComponent<TestAI>(), null));
        _animator.SetTrigger("laserAttack");
    }
    
    private void OnShield(InputAction.CallbackContext p_context)
    {
        shieldVFX.enabled = true;
        shieldBreakVFX.enabled = false;

        float t_shieldSpeedModifier = 0.5f;

        _isProtected = true;
        _animator.SetBool("IsShielding", _isProtected);
        _speed = _baseSpeed * t_shieldSpeedModifier;

        // D marre la consommation de stamina
        if (_shieldStaminaDrainCoroutine != null) StopCoroutine(_shieldStaminaDrainCoroutine);
        _shieldStaminaDrainCoroutine = StartCoroutine(ShieldStaminaDrain());
    }
    
    private void OnUnshield(InputAction.CallbackContext p_context)
    {
        shieldVFX.enabled = false;
        shieldBreakVFX.enabled = true;
        StartCoroutine(ShieldBreakCoroutine());

        _isProtected = false;
        _animator.SetBool("IsShielding", _isProtected);
        _speed = _baseSpeed;

        // Arr te la consommation de stamina
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

            // Si la stamina est  puis e, d sactive le bouclier
            if (!_instance.CanExecuteAbility(_shieldCost))
            {
                OnUnshield(new InputAction.CallbackContext());
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }
    
    private IEnumerator ShieldBreakCoroutine()
    {
        yield return new WaitForSeconds(2);
        shieldBreakVFX.enabled = false;
    }
    
    private void OnJump(InputAction.CallbackContext p_context)
    {
        if (_isGrounded && _instance.CanExecuteAbility(10f))
        {
            _instance.ConsumeStamina(_jumpCost);
            _instance.Context.LastAbilityTime = Time.time;
            _isGrounded = false;
            _animator.SetTrigger("Jump");
            _rigidbody.AddForce(Vector3.up  * _jumpForce, ForceMode.Impulse);
        }
    }

    private void OnDash(InputAction.CallbackContext p_context)
    {
        if (!_isDashing && !_isProtected && _instance.CanExecuteAbility(_dashCost))
        {
            // determine direction du dash bas  sur la direction de deplacement
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

    private IEnumerator DashCoroutine()
    {
        _isDashing = true;
        yield return new WaitForSeconds(_dashDuration);
        _isDashing = false;
    }
    
    private IEnumerator Dash(Vector2 p_moveDir,Vector3 p_vel)
    {
        Debug.Log("Dash");
        _isDashing = true;

        _rigidbody.linearVelocity = new Vector3(_dashForce * p_moveDir.x,p_vel.y,_dashForce*p_moveDir.y);

        yield return new WaitForSeconds(_dashDuration);
        _isDashing = false;
    }

    float ClampAngle(float angle, float from, float to)
    {
        // accepts e.g. -80, 80
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360+from);
        return Mathf.Min(angle, to);
    }

    private void Update()
    {
        Vector2 t_lookDir = _lookAction.ReadValue<Vector2>() * Time.deltaTime * _mouseSensitivity;

        // Tourner le joueur avec la cam ra horizontalement
        transform.Rotate(Vector3.up * t_lookDir.x);

        // G rer la rotation verticale de la cam ra
        var t_clamp = ClampAngle(_cameraPivot.eulerAngles.x - t_lookDir.y, _minVerticalAngle, _maxVerticalAngle);
        var t_delta = t_clamp - _cameraPivot.eulerAngles.x;
        _cameraPivot.Rotate(Vector3.right * t_delta);
    }
    
    private void FixedUpdate()
    {
        Collider[] t_checkGround = Physics.OverlapSphere(groundCheck.position, 0.3f, _groundLayerMask);
        _isGrounded = t_checkGround.Length > 0;

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

            // t_vel = _speed * t_moveDir.y * Time.fixedDeltaTime * transform.forward; for intertia
            t_vel  = _speed * t_moveDir.y * Time.fixedDeltaTime * transform.forward;
            t_vel += _speed * t_moveDir.x * Time.fixedDeltaTime * transform.right;

            _rigidbody.angularVelocity = Vector3.zero;

            // t_vel.x = _speed * t_moveDir.x;
            // t_vel.z = _speed * t_moveDir.y;
            _rigidbody.linearVelocity = new(t_vel.x, _rigidbody.linearVelocity.y, t_vel.z);
            _animator.SetFloat("WalkingSpeed",Mathf.Abs(t_vel.x)+Mathf.Abs(t_vel.z));
        }
        //Debug.Log($"look: {t_lookDir}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, 0.3f);
    }
}
