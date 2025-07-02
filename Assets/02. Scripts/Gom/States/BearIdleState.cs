using UnityEngine;
public class BearIdleState : IBearState
{
    private BearFSM _fsm;
    private float _idleTime;
    private float _randomizedSwitchTime;

    public BearIdleState(BearFSM fsm)
    {
        _fsm = fsm;
    }
    public void Enter()
    {
        if (_fsm.Animator != null)
        {
            _fsm.Animator.SetBool(BearAnimBoolParam.CombatIdle, true);
        }
        else
        {
            Debug.LogError("[BearIdleState] _fsm.Animator°¡ nullÀÓ");
        }
        _randomizedSwitchTime = _fsm.IdleSwitchTime + Random.Range(-1f, 1f);
        _idleTime = 0;
    }

    public void Exit()
    {
        _fsm.Animator.SetBool(BearAnimBoolParam.CombatIdle, false);
    }

    public void Update()
    {
        if (CheckPlayer())
        {
            return;
        }
        if (CheckIdleTimeout())
        {
            return;
        }
    }

    private bool CheckPlayer()
    {
        if (_fsm.CanSeePlayer())
        {
            _fsm.RequestStateChange(EBearState.Chase);
            return true;
        }
        return false;
    }

    private bool CheckIdleTimeout()
    {
        _idleTime += Time.deltaTime;
        if (_idleTime > _randomizedSwitchTime)
        {
            _fsm.RequestStateChange(EBearState.Patrol);
            return true;
        }
        return false;
    }
}
