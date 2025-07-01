using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : PlayerAbility
{
    [Header("Attack Settings")]
    private float _lastAttackTime = 0f;
    public LayerMask EnemyLayer;

    private InputSystem_Actions _inputActions;

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
            _inputActions.Player.Enable();
            _inputActions.Player.Attack.performed += TryAttack;
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
            _inputActions.Player.Attack.performed -= TryAttack;
            _inputActions.Player.Disable();
        }
    }
    private void TryAttack(InputAction.CallbackContext context)
    {
        // 내꺼가 아니면 안돼
        if (!_photonView.IsMine) return;

        // 지상 아니면 안돼
        var movement = _owner.GetAbility<PlayerMovement>();
        if (!movement.IsGrounded) return;

        // 쿨 안돌았으면 안돼
        if (_lastAttackTime + _owner.PlayerStat.AttackCooldown > Time.time) return;

        var stamina = _owner.GetAbility<PlayerStamina>();
        if (stamina.TryConsume(_owner.PlayerStat.AttackStamina))
        {
            DoAttack();
        }
        else
        {
            // 스테미나 없으면 안돼
        }
    }
    private void DoAttack()
    {
        _lastAttackTime = Time.time;
        _photonView.RPC(nameof(RPC_DoAttack), RpcTarget.All);

        int rand = Random.Range(1, 4);
        _owner.GetAbility<AnimationPlayer>().PlayAnimation((AnimTriggerParam)rand);
    }
    [PunRPC]
    private void RPC_DoAttack()
    {
        Vector3 center = transform.position + transform.forward * _owner.PlayerStat.AttackRange * 0.5f;
        Collider[] hits = Physics.OverlapSphere(center, _owner.PlayerStat.AttackRange * 0.5f,EnemyLayer);
        foreach (var hit in hits)
        {
            if (hit.gameObject != gameObject && hit.TryGetComponent(out PlayerHealth health))
            {
                health.TakeDamage(_owner.PlayerStat.AttackDamage, _photonView.OwnerActorNr);
            }
        }
    }

}