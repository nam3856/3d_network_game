using UnityEngine;
using System.Collections;

public class BearHitStunState : IBearState
{
    private BearFSM _fsm;
    private float _hitStunDuration = 0.5f;
    private bool _isRecovering;

    public BearHitStunState(BearFSM fsm)
    {
        _fsm = fsm;
    }

    public void Enter()
    {
        _fsm.Agent.isStopped = true;
        _fsm.Animator.SetTrigger(BearAnimTriggerParam.Hit);

        _isRecovering = true;
        _fsm.StartCoroutine(RecoverAfterDelay());
    }

    public void Exit()
    {
        _isRecovering = false;
    }

    public void Update()
    {
    }

    private IEnumerator RecoverAfterDelay()
    {
        yield return new WaitForSeconds(_hitStunDuration);

        if (!_isRecovering) yield break;

        if (_fsm.CanSeePlayer())
            _fsm.RequestStateChange(EBearState.Chase);
        else
            _fsm.RequestStateChange(EBearState.Idle);
    }
}
