using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
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
    private CinemachineInputAxisController _cinemachineInputAxisController;

    private Vector2 _moveInput;
    private Vector3 _velocity;
    private bool _isSprinting;
    private bool _isGrounded;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
        _moveInput = Vector2.zero;
        _inputActions.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _inputActions.Player.Move.canceled += ctx => _moveInput = Vector2.zero;


        _inputActions.Player.Jump.performed += ctx => Jump();
        _inputActions.Player.Sprint.performed += ctx => _isSprinting = true;
        _inputActions.Player.Sprint.canceled += ctx => _isSprinting = false;
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
    }

    private void Update()
    {
        _isGrounded = _controller.isGrounded;

        MovePlayer();
        ApplyGravity();
        UpdateAnimator();
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
