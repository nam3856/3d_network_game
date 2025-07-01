using Photon.Pun;
using UnityEngine;

public enum AnimTriggerParam { 
    Jump = 0,
    Attack1 = 1, 
    Attack2 = 2, 
    Attack3 = 3, 
    Hit = 4,
    Die = 5,
    Respawn = 6
}

public enum AnimBoolParam { IsGrounded }
public enum AnimFloatParam { Speed }
public class AnimationPlayer : PlayerAbility
{
    private Animator _animator;

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
    }
    protected override void OnEnable()
    {
        if (!_photonView.IsMine) return;
        AnimTriggerParam type = AnimTriggerParam.Respawn;
        _photonView.RPC(nameof(RPC_PlayAnimation), RpcTarget.All, type);
    }

    public void PlayAnimation(AnimTriggerParam type)
    {
        if (!_photonView.IsMine) return;
        _photonView.RPC(nameof(RPC_PlayAnimation), RpcTarget.All, type);
    }

    public void SetBool(AnimBoolParam param, bool value)
    {
        if (!_photonView.IsMine) return;
        _photonView.RPC(nameof(RPC_SetBool), RpcTarget.All, (int)param, value);
    }

    [PunRPC]
    private void RPC_SetBool(int paramInt, bool value)
    {
        string paramName = ((AnimBoolParam)paramInt).ToString();
        _animator.SetBool(paramName, value);
    }

    public void SetFloat(AnimFloatParam param, float value)
    {
        if (!_photonView.IsMine) return;
        _photonView.RPC(nameof(RPC_SetFloat), RpcTarget.All, (int)param, value);
    }

    [PunRPC]
    private void RPC_SetFloat(int paramInt, float value)
    {
        string paramName = ((AnimFloatParam)paramInt).ToString();
        _animator.SetFloat(paramName, value);
    }


    [PunRPC]
    public void RPC_PlayAnimation(AnimTriggerParam type)
    {
        switch (type)
        {
            case AnimTriggerParam.Jump:
                _animator.SetTrigger("IsJumping");
                break;
            case AnimTriggerParam.Attack1:
                _animator.SetTrigger("Attack1");
                break;
            case AnimTriggerParam.Attack2:
                _animator.SetTrigger("Attack2");
                break;
            case AnimTriggerParam.Attack3:
                _animator.SetTrigger("Attack3");
                break;
            case AnimTriggerParam.Hit:
                _animator.SetTrigger("Hit");
                break;
            case AnimTriggerParam.Die:
                _animator.SetTrigger("Die");
                break;
            case AnimTriggerParam.Respawn:
                _animator.SetTrigger("Respawn");
                break;
            default:
                Debug.LogError("No!!!!!!!!!!!!!!!!!!!!");
                break;
        }
    }
}
