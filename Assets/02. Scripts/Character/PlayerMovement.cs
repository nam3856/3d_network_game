using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerMovement : PlayerAbility
{
    [Header("References")]
    public Transform CameraTransform;
    public Animator Animator;

    [Header("Movement Settings")]
    public float WalkSpeed = 5f;
    public float SprintSpeed = 10f;
    public float JumpHeight = 2f;
    public float Gravity = -9.81f;

    private CharacterController _controller;
    private InputSystem_Actions _inputActions;

    private Vector2 _moveInput;
    private Vector3 _velocity;
    private bool _isSprinting;
    private bool _isGrounded;

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

        float speed = _isSprinting ? SprintSpeed : WalkSpeed;
        _controller.Move(move.normalized * speed * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        _velocity.y += Gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void Jump()
    {
        if (_isGrounded)
        {
            _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            Animator?.SetTrigger("IsJumping");
        }
    }

    private float _currentAnimSpeed = 0f;
    [SerializeField] private float speedSmoothTime = 10f;

    private void UpdateAnimator()
    {
        Vector3 flatInput = new Vector3(_moveInput.x, 0f, _moveInput.y);
        float targetSpeed = flatInput.magnitude * (_isSprinting ? SprintSpeed : WalkSpeed);

        _currentAnimSpeed = Mathf.Lerp(_currentAnimSpeed, targetSpeed, Time.deltaTime * speedSmoothTime);

        Animator?.SetFloat("Speed", _currentAnimSpeed);
        Animator?.SetBool("IsGrounded", _isGrounded);
    }

}
