using Photon.Pun;
using UnityEngine;
using System.Collections;

public class BearDieState : IBearState
{
    private BearFSM _fsm;

    public BearDieState(BearFSM fsm)
    {
        _fsm = fsm;
    }

    public void Enter()
    {
        _fsm.Animator.SetBool(BearAnimBoolParam.Death, true);

        _fsm.Agent.isStopped = true;
        _fsm.Agent.ResetPath();

        _fsm.CharacterController.enabled = false;
        foreach (Collider col in _fsm.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        GameObject deathFx = Photon.Pun.PhotonNetwork.Instantiate(
            "DeathEffect",
            _fsm.transform.position + new Vector3(0,0.5f,0),
            Quaternion.identity
        );

        if (PhotonNetwork.IsMasterClient)
        {
            Vector3 dropPos = _fsm.transform.position + Vector3.up * 0.5f;
            EItemType dropItem = GetRandomDropItem();
            ItemObjectFactory.Instance.RequestCreate(dropItem, dropPos, 1);
        }

        _fsm.StartCoroutine(DestroyAfterDelay(3f));
    }

    public void Exit()
    {
    }

    public void Update()
    {
    }
    private EItemType GetRandomDropItem()
    {
        float rand = Random.Range(0f, 1f);

        if (rand <= 0.2f)
            return EItemType.HealItem;
        else if (rand <= 0.4f)
            return EItemType.RecoverStaminaItem;
        else if (rand <= 0.6f)
            return EItemType.PowerUpItem;
        else if (rand <= 0.8f)
            return EItemType.SpeedUpItem;
        else
            return EItemType.ScoreItem;
    }


    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(_fsm.gameObject);
        }
    }
}
