using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
using System.Linq;
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(NavMeshAgent))]
public class BearFSM : MonoBehaviourPun
{
    public static event System.Action OnBearDied;
    private Dictionary<EBearState, IBearState> _states;
    private IBearState _currentState;
    private EBearState _currentStateType;
    public NavMeshAgent Agent { get; private set; }
    public BearAnimator Animator { get; private set; }
    public Collider CharacterController { get; private set; }

    public BearUI BearUI { get; private set; }

    public float MaxHealth { get; private set; } = 300f;
    public float Health { get; private set; }

    [Header("Idle")]
    public float IdleSwitchTime = 10;

    [Header("Patrol")]
    public int PatrolCount = 10;
    public float PatrolRange = 20f;
    public Transform[] PatrolPoints;
    public int PatrolIndex = 0;
    public float PatrolWaitTime = 2f;
    public float PatrolSpeed { get; private set; }

    [Header("Chase")]
    public float ChaseSpeed { get; private set; }
    public float AttackRange = 2.4f;
    public float LoseSightRange = 20f;

    [Header("Attack")]
    public float AttackDamage = 20;
    public float AttackCooldown = 1.5f;

    [Header("Enrage")]
    public float EnrageThreshold;
    public float BaseChaseSpeed = 3.5f;
    public float BasePatrolSpeed = 1.5f;
    public GameObject EnragedEffect;
    public bool IsEnraged { get; private set; } = false;

    void Awake()
    {
        Health = MaxHealth;
        CharacterController = GetComponent<Collider>();
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<BearAnimator>();
        BearUI = GetComponent<BearUI>();
        EnrageThreshold = Health / 3f;
        _states = new Dictionary<EBearState, IBearState>
        {
            { EBearState.Idle, new BearIdleState(this) },
            { EBearState.Chase, new BearChaseState(this) },
            { EBearState.Attack, new BearAttackState(this) },
            { EBearState.Enraged, new BearEnragedState(this) },
            { EBearState.Patrol, new BearPatrolState(this) },
            { EBearState.HitStun, new BearHitStunState(this) },
            { EBearState.Die, new BearDieState(this) },
        };
        ChaseSpeed = BaseChaseSpeed;
        PatrolSpeed = BasePatrolSpeed;
        RequestStateChange(EBearState.Idle);
    }
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GeneratePatrolPoints();
            BearUI.UpdateHealthUI(Health, MaxHealth);
        }
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        _currentState?.Update();
    }

    public void RequestStateChange(EBearState newState)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        photonView.RPC(nameof(RPC_ChangeState), RpcTarget.All, (int)newState);
    }

    [PunRPC]
    private void RPC_ChangeState(int newState)
    {
        if (_currentState != null)
            _currentState.Exit();

        Debug.Log($"[BearFSM] State Change → {(EBearState)newState}");
        _currentState = _states[(EBearState)newState];
        _currentStateType = (EBearState)newState;
        _currentState.Enter();
    }
    public Transform GetClosestAlivePlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (var obj in players)
        {
            PlayerHealth health = obj.GetComponent<PlayerHealth>();
            if (health == null || health.IsDead) continue;

            float dist = Vector3.Distance(transform.position, obj.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = obj.transform;
            }
        }

        return closest;
    }
    public bool CanSeePlayer()
    {
        Transform target = GetClosestAlivePlayer();
        if (target == null) return false;

        return Vector3.Distance(transform.position, target.position) < 15f;
    }

    public bool IsPlayerInAttackRange()
    {
        Transform target = GetClosestAlivePlayer();
        if (target == null) return false;

        return Vector3.Distance(transform.position, target.position) < AttackRange;
    }

    public void OnTakeDamage(float damage, int attackerActorNumber = -1)
    {
        if (_currentStateType == EBearState.Die) return;
        

        Health -= damage;
        Health = Mathf.Max(0, Health);

        BearUI.UpdateHealthUI(Health, MaxHealth);

        if (Health <= 0f)
        {
            OnDeath();
            return;
        }
        else
        {
            PhotonNetwork.Instantiate(
                $"HitEffect_{Random.Range(1, 11)}",
                transform.position + new Vector3(0, 0.5f, 0),
                Quaternion.identity);
        }

            CheckEnrageCondition();
        if (IsEnraged) return;
        RequestStateChange(EBearState.HitStun);
    }

    public void OnDeath()
    {
        RequestStateChange(EBearState.Die);

        if (PhotonNetwork.IsMasterClient)
        {
            OnBearDied?.Invoke();
        }
    }
    public void CheckEnrageCondition()
    {
        if (IsEnraged) return;

        if (Health < EnrageThreshold)
        {
            RequestStateChange(EBearState.Enraged);
        }
    }
    public void SetEnraged(bool value)
    {
        IsEnraged = value;

        if (value)
        {
            ChaseSpeed = BaseChaseSpeed * 1.5f;
            PatrolSpeed = BasePatrolSpeed * 1.5f;

            if (EnragedEffect != null)
                EnragedEffect.SetActive(true);
        }
        else
        {
            ChaseSpeed = BaseChaseSpeed;
            PatrolSpeed = BasePatrolSpeed;

            if (EnragedEffect != null)
                EnragedEffect.SetActive(false);
        }
    }

    private void GeneratePatrolPoints()
    {
        List<Transform> points = new List<Transform>();

        for (int i = 0; i < PatrolCount; i++)
        {
            Vector2 offset2D = Random.insideUnitCircle * PatrolRange;
            Vector3 targetPos = transform.position + new Vector3(offset2D.x, 0, offset2D.y);

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                GameObject point = new GameObject($"PatrolPoint_{i}");
                point.transform.position = hit.position;
                points.Add(point.transform);
            }
        }

        PatrolPoints = points.ToArray();
    }
}
