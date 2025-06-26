using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float AttackRange = 2f;
    public int AttackDamage = 10;
    public float AttackCooldown = 0.6f;
    private float _lastAttackTime = 0f;
    public LayerMask EnemyLayer;
    private Animator _playerAnimator;
    private InputSystem_Actions _inputActions;
    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
        if (_playerAnimator == null)
        {
            _playerAnimator = GetComponent<Animator>();
        }
    }
    private void OnEnable()
    {
        _inputActions.Player.Enable();
        _inputActions.Player.Attack.performed += OnAttackPerformed;
    }
    private void OnDisable()
    {
        _inputActions.Player.Attack.performed -= OnAttackPerformed;
        _inputActions.Player.Disable();
    }
    private void OnAttackPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if(_lastAttackTime + AttackCooldown > Time.time)
            return;
        _lastAttackTime = Time.time;
        int rand = Random.Range(1, 4);
        _playerAnimator.SetTrigger($"Attack{rand}");
        PerformAttack();
    }
    private void PerformAttack()
    {
        // 공격로직
    }
}