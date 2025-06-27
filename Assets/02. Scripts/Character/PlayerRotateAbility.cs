using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotateAbility : PlayerAbility
{
    [Header("References")]
    public Transform CameraRoot;
    public CinemachineCamera VCamera;
    public Camera MinimapCamera;
    [Header("Rotation Settings")]
    public float MouseRotationSpeed = 100f;
    public float GamepadRotationSpeed = 500f;

    private InputSystem_Actions _inputActions;
    private float _mouseX;
    private float _mouseY;
    private float _currentCameraRotationX = 0f;

    private float _currentRotationSpeed;

    protected override void Awake()
    {
        base.Awake();
        _inputActions = new InputSystem_Actions();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        if (_photonView.IsMine)
        {
            VCamera.Follow = CameraRoot;
            _inputActions.Player.Enable();
            _inputActions.Player.Look.performed += OnLookPerformed;
            _inputActions.Player.Look.canceled += OnLookCanceled;
        }
        else
        {
            VCamera.gameObject.SetActive(false);
            MinimapCamera.gameObject.SetActive(false);
            _inputActions.Player.Disable();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (_photonView.IsMine)
        {
            _inputActions.Player.Look.performed -= OnLookPerformed;
            _inputActions.Player.Disable();
        }
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>();
        _mouseX = lookInput.x;
        _mouseY = lookInput.y;

        if (context.control.device is Mouse)
        {
            _currentRotationSpeed = MouseRotationSpeed;
        }
        else if (context.control.device is Gamepad)
        {
            _currentRotationSpeed = GamepadRotationSpeed;
        }
        else
        {
            _currentRotationSpeed = MouseRotationSpeed;
        }
    }

    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        _mouseX = 0f;
        _mouseY = 0f;
    }

    protected override void Update()
    {
        base.Update();
        if (_photonView.IsMine)
        {
            RotatePlayer();
        }
    }

    private void RotatePlayer()
    {
        float rotationAmountX = _mouseX * _currentRotationSpeed * Time.deltaTime;
        float rotationAmountY = _mouseY * _currentRotationSpeed * Time.deltaTime;

        transform.Rotate(Vector3.up, rotationAmountX);

        _currentCameraRotationX -= rotationAmountY;
        _currentCameraRotationX = Mathf.Clamp(_currentCameraRotationX, -90f, 90f);

        CameraRoot.localEulerAngles = new Vector3(_currentCameraRotationX, 0f, 0f);
    }
}