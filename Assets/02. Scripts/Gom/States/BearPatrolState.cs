using UnityEngine;

public class BearPatrolState : IBearState
{
    private BearFSM _fsm;
    private float _waitTimer;
    private bool _waiting;

    private const float ArrivalThreshold = 0.5f;

    public BearPatrolState(BearFSM fsm)
    {
        _fsm = fsm;
    }

    public void Enter()
    {
        _fsm.Animator.SetBool(BearAnimBoolParam.WalkForward, true);
        _waiting = false;
        _waitTimer = 0f;
        _fsm.Agent.speed = _fsm.PatrolSpeed;

        MoveToNextPoint();
    }

    public void Exit()
    {
        _fsm.Animator.SetBool(BearAnimBoolParam.WalkForward, false);
        _fsm.Agent.ResetPath();
    }

    public void Update()
    {
        if (_fsm.CanSeePlayer())
        {
            _fsm.RequestStateChange(EBearState.Chase);
            return;
        }

        if (_waiting)
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= _fsm.PatrolWaitTime)
            {
                _waiting = false; 
                _fsm.Animator.SetBool(BearAnimBoolParam.WalkForward, true);
                MoveToNextPoint();
            }
            return;
        }

        if (!_fsm.Agent.pathPending && _fsm.Agent.remainingDistance <= ArrivalThreshold)
        {
            _waiting = true;
            _waitTimer = 0f;
            _fsm.Animator.SetBool(BearAnimBoolParam.WalkForward, false);
        }
    }

    private void MoveToNextPoint()
    {
        if (_fsm.PatrolPoints == null || _fsm.PatrolPoints.Length == 0)
            return;

        _fsm.Agent.SetDestination(_fsm.PatrolPoints[_fsm.PatrolIndex].position);
        _fsm.PatrolIndex = (_fsm.PatrolIndex + 1) % _fsm.PatrolPoints.Length;
    }
}
