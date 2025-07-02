using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : PlayerAbility
{
    [Header("Attack Settings")]
    private float _lastAttackTime = 0f;
    public LayerMask EnemyLayer;

    private InputSystem_Actions _inputActions;

    private float _buffTime = 0;
    private bool _isBuffed = false;

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

    private void Start()
    {
        if (_photonView.IsMine)
        {
            _photonView.RPC("ReportMyStats", RpcTarget.MasterClient,
                _owner.PlayerStat.MoveSpeed, _owner.PlayerStat.AttackMinDamage, _owner.PlayerStat.AttackMaxDamage, _owner.PlayerStat.MaxHealth, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    protected override void Update()
    {
        base.Update();
        if (_buffTime > 0)
        {
            _buffTime -= Time.deltaTime;
            _isBuffed = true;
        }
        else
        {
            _isBuffed = false;
        }
    }
    [PunRPC]
    void ReportMyStats(float moveSpeed, float attackDamage, float attackDamage2, float maxHealth, int actorNumber)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (moveSpeed > 10f || attackDamage > 20 || attackDamage > 40 || maxHealth > 100f)
        {
            _photonView.RPC("ForceBadStats", PhotonNetwork.CurrentRoom.GetPlayer(actorNumber));
        }
    }

    [PunRPC]
    void ForceBadStats()
    {
        _owner.PlayerStat.MoveSpeed = 0f;
        _owner.PlayerStat.AttackMinDamage = -10;
        _owner.PlayerStat.AttackMaxDamage = -5;
        _owner.PlayerStat.MaxHealth = 1f;
        GetComponent<MeshRenderer>().material.color = Color.magenta;
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

    public void SetBuffTimer()
    {
        _buffTime = _owner.PlayerStat.AttackBuffTime;
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
        

        int rand = Random.Range(1, 4);
        float damage = Random.Range(_owner.PlayerStat.AttackMinDamage, _owner.PlayerStat.AttackMaxDamage);
        if (_isBuffed)
        {
            damage *= _owner.PlayerStat.AttackBuffStat;
        }
        _photonView.RPC(nameof(RPC_DoAttack), RpcTarget.All, damage);
        _owner.GetAbility<AnimationPlayer>().PlayAnimation((AnimTriggerParam)rand);
    }
    [PunRPC]
    private void RPC_DoAttack(float damage = 0)
    {
        Vector3 center = transform.position + transform.forward * _owner.PlayerStat.AttackRange * 0.5f;
        Collider[] hits = Physics.OverlapSphere(center, _owner.PlayerStat.AttackRange * 0.5f,EnemyLayer);
        foreach (var hit in hits)
        {
            if (hit.gameObject != gameObject && hit.TryGetComponent(out PlayerHealth health))
            {
                health.TakeDamage(damage, _photonView.OwnerActorNr);
            }

            if (hit.TryGetComponent(out BearFSM bear))
            {
                bear.OnTakeDamage(damage, _photonView.OwnerActorNr);
            }
        }
    }

}