using Photon.Pun;
using UnityEngine;

public class PlayerAttack : PlayerAbility
{
    [Header("Attack Settings")]
    private float _lastAttackTime = 0f;
    public LayerMask EnemyLayer;

    private Animator _playerAnimator;
    private InputSystem_Actions _inputActions;
    protected override void Awake()
    {
        base.Awake();
        _inputActions = new InputSystem_Actions();
        if (_playerAnimator == null)
        {
            _playerAnimator = GetComponent<Animator>();
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        if (_photonView.IsMine)
        {
            _inputActions.Player.Enable();
            _inputActions.Player.Attack.performed += OnAttackPerformed;
        }
        else
        {
            _inputActions.Player.Disable();
        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        if (_photonView.IsMine)
        {
            _inputActions.Player.Attack.performed -= OnAttackPerformed;
            _inputActions.Player.Disable();
        }
    }
    private void OnAttackPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!_photonView.IsMine) return;
        if (_lastAttackTime + _owner.PlayerStat.AttackCooldown > Time.time)
            return;
        _lastAttackTime = Time.time;
        int rand = Random.Range(1, 4);
        _playerAnimator.SetTrigger($"Attack{rand}");
        PerformAttack();
    }
    private void PerformAttack()
    {
        if (!_photonView.IsMine) return;
        // 공격로직
    }
}