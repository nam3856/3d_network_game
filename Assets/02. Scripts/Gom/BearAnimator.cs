using Photon.Pun;
using UnityEngine;

public enum BearAnimTriggerParam
{
    Attack1 = 0,
    Attack2 = 1,
    Attack3 = 2,
    Attack4 = 3,
    Hit = 4,
    Buff = 5,
}

public enum BearAnimBoolParam { CombatIdle, WalkForward, Run, Death }
public class BearAnimator : MonoBehaviourPun
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        BearAnimBoolParam type = BearAnimBoolParam.CombatIdle;
        photonView.RPC(nameof(RPC_SetAniBool), RpcTarget.All, type, true);
    }

    public void SetTrigger(BearAnimTriggerParam type)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        photonView.RPC(nameof(RPC_SetAniTrigger), RpcTarget.All, type);
    }

    public void SetBool(BearAnimBoolParam param, bool value)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        photonView.RPC(nameof(RPC_SetAniBool), RpcTarget.All, param, value);
    }

    [PunRPC]
    private void RPC_SetAniBool(BearAnimBoolParam param, bool value)
    {
        string paramName = param.ToString();
        _animator?.SetBool(paramName, value);
    }



    [PunRPC]
    public void RPC_SetAniTrigger(BearAnimTriggerParam type)
    {
        string paramName = type.ToString();
        _animator?.SetTrigger(paramName);
    }
}
