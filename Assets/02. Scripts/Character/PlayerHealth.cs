using System;
using Photon.Pun;
using UnityEngine;
public class PlayerHealth : PlayerAbility, IPunObservable
{
    private float _currentHealth;
    private float MaxHealth => _owner.PlayerStat.MaxHealth;

    public static event Action<(int, int)> OnDied;

    public bool IsDead => _currentHealth <= 0;
    private AnimationPlayer _animationPlayer;
    private void Start()
    {
        _animationPlayer = _owner.GetAbility<AnimationPlayer>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _currentHealth = MaxHealth;
        _owner.GetAbility<PlayerUI>()?.UpdateHealthUI(_currentHealth, MaxHealth);
        if(_photonView!=null && _photonView.IsMine)
            HUDManager.Instance.UpdateHpUI(_currentHealth, MaxHealth);
    }
    public void TakeDamage(float damage, int attackerNum)
    {
        if (!_photonView.IsMine) return;
        _currentHealth -= damage;
        _currentHealth = Mathf.Max(0, _currentHealth);
        _owner.GetAbility<PlayerUI>()?.UpdateHealthUI(_currentHealth, MaxHealth);
        HUDManager.Instance.UpdateHpUI(_currentHealth, MaxHealth);
        
        if (_currentHealth <= 0)
        {
            Die(attackerNum);
        }
        else
        {
            _owner.GetAbility<PlayerShakingAbility>().Shake();
            _animationPlayer.PlayAnimation(AnimTriggerParam.Hit);
        }
    }


    private void Die(int attackerNum = 0)
    {
        _photonView.RPC(nameof(RPC_InvokeDie), RpcTarget.All, attackerNum);
        _animationPlayer.PlayAnimation(AnimTriggerParam.Die);
        _owner.GetAbility<PlayerAttack>().enabled = false;
        _owner.GetAbility<PlayerMovement>().enabled = false;
        _owner.GetAbility<PlayerStamina>().enabled = false;
        _owner.GetAbility<AnimationPlayer>().enabled = false;
        _owner.GetAbility<PlayerRespawn>().enabled = true;

        Debug.Log("플레이어 사망");
    }

    [PunRPC]
    private void RPC_InvokeDie(int attackerNum)
    {
        OnDied.Invoke((attackerNum, _photonView.OwnerActorNr));
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_currentHealth);
        }
        else
        {
            _currentHealth = (float)stream.ReceiveNext();
            _owner.GetAbility<PlayerUI>()?.UpdateHealthUI(_currentHealth, MaxHealth);
        }
    }
}
