using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerMovement : PlayerAbility
{
    [Header("References")]
    public Transform CameraTransform;
    public Animator Animator;

    [Header("Movement Settings")]
    private float WalkSpeed => _owner.PlayerStat.WalkSpeed;
    private float SprintSpeed => _owner.PlayerStat.SprintSpeed;
    private float JumpHeight => _owner.PlayerStat.JumpHeight;
    private float Gravity => _owner.PlayerStat.Gravity;
    private bool IsInWater => transform.position.y < -0.4f;

    private CharacterController _controller;
    private InputSystem_Actions _inputActions;

    private Vector2 _moveInput;
    private Vector3 _velocity;
    private bool _isSprinting;
    private bool _isGrounded;
    public bool IsGrounded => _isGrounded;

    protected override void Awake()
    {
        base.Awake();
        _controller = GetComponent<CharacterController>();
        _inputActions = new InputSystem_Actions();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (_photonView.IsMine)
        {
            _inputActions.Player.Enable();
            _moveInput = Vector2.zero;
            _inputActions.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
            _inputActions.Player.Move.canceled += ctx => _moveInput = Vector2.zero;


            _inputActions.Player.Jump.performed += ctx => Jump();
            _inputActions.Player.Sprint.performed += ctx => _isSprinting = true;
            _inputActions.Player.Sprint.canceled += ctx => _isSprinting = false;
        }
        else
        {
            _inputActions.Player.Disable();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _inputActions.Player.Disable();
    }

    protected override void Update()
    {
        base.Update();
        if (_photonView.IsMine)
        {
            _isGrounded = _controller.isGrounded;

            MovePlayer();
            ApplyGravity();
            UpdateAnimator();
        }
    }

    private void MovePlayer()
    {
        Vector3 camForward = Vector3.ProjectOnPlane(CameraTransform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(CameraTransform.right, Vector3.up).normalized;

        Vector3 move = camForward * _moveInput.y + camRight * _moveInput.x;

        float speed = GetCurrentSpeed();
        if (IsInWater)
            speed *= 0.5f;
        _controller.Move(move.normalized * speed * Time.deltaTime);
    }
    private float GetCurrentSpeed()
    {
        if (_isSprinting)
        {
            var stamina = _owner.GetAbility<PlayerStamina>();
            if (stamina != null && stamina.TryConsume(_owner.PlayerStat.RunStaminaPerSecond * Time.deltaTime))
            {
                return SprintSpeed;
            }
            else
            {
                _isSprinting = false;
            }
        }
        return WalkSpeed;
    }
    private void ApplyGravity()
    {
        float gravityMultiplier = IsInWater ? 0.2f : 1f;

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        _velocity.y += Gravity * gravityMultiplier * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void Jump()
    {
        var stamina = _owner.GetAbility<PlayerStamina>();
        if (IsInWater || _isGrounded)
        {
            
            if (stamina.TryConsume(_owner.PlayerStat.JumpStamina))
            {
                _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                Animator?.SetTrigger("IsJumping");
            }

        }
    }

    private float _currentAnimSpeed = 0f;
    [SerializeField] private float speedSmoothTime = 10f;

    private void UpdateAnimator()
    {
        Vector3 flatInput = new Vector3(_moveInput.x, 0f, _moveInput.y);
        float targetSpeed = flatInput.magnitude * (_isSprinting ? SprintSpeed : WalkSpeed);

        _currentAnimSpeed = Mathf.Lerp(_currentAnimSpeed, targetSpeed, Time.deltaTime * speedSmoothTime);
        float dividedAnimSpeed = _currentAnimSpeed / SprintSpeed;
        Animator?.SetFloat("Speed", dividedAnimSpeed);
        Animator?.SetBool("IsGrounded", _isGrounded);
    }

}
