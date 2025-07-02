using UnityEngine;
using Photon.Pun;
using System.Collections;

public class BearAttackState : IBearState
{
    private BearFSM _fsm;
    private bool _isAttacking;
    private float _attackStartTime;
    private int _attackIndex;
    private Coroutine _attackRoutine;

    private readonly float[] _attackHitDelays = new float[]
    {
        0.21f, // Attack1
        0.21f, // Attack2
        0.20f, // Attack3
        0.40f  // Attack4
    };

    public BearAttackState(BearFSM fsm)
    {
        _fsm = fsm;
    }

    public void Enter()
    {
        _fsm.Agent.isStopped = true;
        _attackIndex = Random.Range(0, 4);
        _fsm.Animator.SetTrigger((BearAnimTriggerParam)_attackIndex);

        _isAttacking = true;
        _attackStartTime = Time.time;

        _attackRoutine = _fsm.StartCoroutine(DelayedHit(_attackHitDelays[_attackIndex]));
    }

    public void Exit()
    {
        _isAttacking = false;
        if (_attackRoutine != null)
            _fsm.StopCoroutine(_attackRoutine);
    }

    public void Update()
    {
        if (!_isAttacking) return;

        if (Time.time - _attackStartTime >= _fsm.AttackCooldown)
        {
            if (_fsm.IsPlayerInAttackRange())
            {
                _fsm.RequestStateChange(EBearState.Attack);
            }
            else
            {
                _fsm.RequestStateChange(EBearState.Chase);
            }
        }
    }

    private IEnumerator DelayedHit(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!PhotonNetwork.IsMasterClient)
            yield break;

        Vector3 hitPoint = _fsm.transform.position + _fsm.transform.forward * 1.5f + Vector3.up;
        float radius = 1.2f;

        Collider[] hits = Physics.OverlapSphere(hitPoint, radius);
        foreach (var col in hits)
        {
            if (col.CompareTag("Player"))
            {
                PlayerHealth health = col.GetComponent<PlayerHealth>();
                if (health != null && !health.IsDead)
                {
                    health.TakeDamage(_fsm.AttackDamage, attackerNum: -1);
                }
            }
        }
    }
}
