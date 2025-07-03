using System;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;
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
            PhotonNetwork.Instantiate($"HitEffect_{Random.Range(1, 11)}", transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }

    public void Heal(float value)
    {
        if (!_photonView.IsMine) return;
        _currentHealth += value;
        _currentHealth = Mathf.Min(MaxHealth, _currentHealth);

        _owner.GetAbility<PlayerUI>()?.UpdateHealthUI(_currentHealth, MaxHealth);
        HUDManager.Instance.UpdateHpUI(_currentHealth, MaxHealth);
    }
    private void Die(int attackerNum = -1)
    {
        var rand = Random.Range(0, 1f);
        EItemType itemType;
        if (rand<=0.2f)
        {
            itemType = EItemType.HealItem;
        }
        else if(rand<=0.5f)
        {
            itemType = EItemType.RecoverStaminaItem;
        }
        else
        {
            itemType = EItemType.ScoreItem;
        }
        _photonView.RPC(nameof(RPC_Die), RpcTarget.All, attackerNum, itemType);
    }
    [PunRPC]
    private void RPC_Die(int attackerNum = -1, EItemType type = EItemType.ScoreItem)
    {
        if (_photonView.IsMine && attackerNum >=0)
        {
            MakeItems(1, type);
            _photonView.RPC(nameof(RPC_InvokeDie), RpcTarget.All, attackerNum);
        }
        _owner.CharacterController.enabled = false;
        var col = GetComponentsInChildren<Collider>();
        foreach (Collider c in col)
        {
            c.enabled = false;
        }
        PhotonNetwork.Instantiate("DeathEffect", transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        
        _animationPlayer.PlayAnimation(AnimTriggerParam.Die);
        _owner.GetAbility<PlayerAttack>().enabled = false;
        _owner.GetAbility<PlayerMovement>().enabled = false;
        _owner.GetAbility<PlayerStamina>().enabled = false;
        _owner.GetAbility<AnimationPlayer>().enabled = false;
        _owner.GetAbility<PlayerRespawn>().enabled = true;

        Debug.Log("플레이어 사망");
    }

    private void MakeItems(int count, EItemType type = EItemType.ScoreItem)
    {
        var pos = transform.position + new Vector3(0, 0.5f, 0);
        ItemObjectFactory.Instance.RequestCreate(type, pos, count);
    }

    [PunRPC]
    private void RPC_InvokeDie(int attackerNum)
    {
        OnDied?.Invoke((attackerNum, _photonView.OwnerActorNr));
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
