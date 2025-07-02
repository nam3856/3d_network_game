using UnityEngine;
using System.Collections;

public class BearRoarState : IBearState
{
    private BearFSM _fsm;
    private float _roarDuration = 1.5f;

    public BearRoarState(BearFSM fsm)
    {
        _fsm = fsm;
    }

    public void Enter()
    {
        Debug.Log("[BearFSM] 포효 상태 진입");
        _fsm.Agent.ResetPath();
        _fsm.Agent.isStopped = true;
        _fsm.Animator.SetTrigger(BearAnimTriggerParam.Buff);

        _fsm.StartCoroutine(EndRoarAfterDelay());
    }

    public void Exit() { }

    public void Update() { }

    private IEnumerator EndRoarAfterDelay()
    {
        yield return new WaitForSeconds(_roarDuration);

        _fsm.RequestStateChange(EBearState.Attack);
    }
}
