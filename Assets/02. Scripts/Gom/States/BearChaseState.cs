using UnityEngine;
using UnityEngine.AI;

public class BearChaseState : IBearState
{
    private BearFSM _fsm;
    private float _nextPathUpdateTime;

    private const float PathUpdateInterval = 0.2f;
    private Transform _target;
    public BearChaseState(BearFSM fsm)
    {
        _fsm = fsm;
    }

    public void Enter()
    {
        _fsm.Agent.isStopped = false;
        _fsm.Agent.speed = _fsm.ChaseSpeed;

        _fsm.Animator.SetBool(BearAnimBoolParam.Run, true);
        _fsm.Agent.speed = _fsm.ChaseSpeed;
        _nextPathUpdateTime = Time.time + PathUpdateInterval;
        _target = _fsm.GetClosestAlivePlayer();
    }

    public void Exit()
    {
        _fsm.Agent.ResetPath();
        _fsm.Animator.SetBool(BearAnimBoolParam.Run, false);
    }

    public void Update()
    {
        if (_target == null)
            return;

        float distance = Vector3.Distance(_fsm.transform.position, _target.position);

        // 일정 주기마다 SetDestination 갱신
        if (Time.time >= _nextPathUpdateTime)
        {
            _target = _fsm.GetClosestAlivePlayer();
            Debug.Log($"{_target}");
            if (_target != null)
            {
                Vector3 targetPos = _target.position;

                if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                {
                    _fsm.Agent.SetDestination(hit.position);
                    Debug.Log($"[BearChaseState] SetDestination: {hit.position}");
                }
                else
                {
                    _fsm.RequestStateChange(EBearState.Idle);
                }
                _nextPathUpdateTime = Time.time + PathUpdateInterval;
            }
            else
            {

                _fsm.RequestStateChange(EBearState.Idle);
                return;
            }
        }

        if (distance <= _fsm.AttackRange)
        {
            float rand = Random.Range(0f, 1f);
            if (!_fsm.IsEnraged && rand <= 0.1f)
            {
                _fsm.RequestStateChange(EBearState.Roar);
            }
            else
            {
                _fsm.RequestStateChange(EBearState.Attack);
            }
            return;
        }

        if (distance > _fsm.LoseSightRange)
        {

            _fsm.RequestStateChange(EBearState.Idle);
            return;
        }
    }
}
