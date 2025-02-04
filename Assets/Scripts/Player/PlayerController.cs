using System.Collections;
using Mekaiju;
using Mekaiju.AI;
using MyBox;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public Transform groundCheck;
    public Transform camera;

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
            // determine direction du dash basé sur la direction de deplacement
            Vector2 moveInput = _moveAction.ReadValue<Vector2>();
            _dashDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

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
            t_vel.x = _speed * t_moveDir.x;
            t_vel.z = _speed * t_moveDir.y;
            _rigidbody.linearVelocity = t_vel;
            _animator.SetFloat("WalkingSpeed",Mathf.Abs(t_vel.x)+Mathf.Abs(t_vel.z));
        }

        Vector2 t_lookDir = _lookAction.ReadValue<Vector2>();
        //Debug.Log($"look: {t_lookDir}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, 0.3f);
    }
}
