using Photon.Pun;
using UnityEngine;
public class PlayerHealth : PlayerAbility
{
    private float _currentHealth;
    private float MaxHealth => _owner.PlayerStat.MaxHealth;

    private AnimationPlayer _animationPlayer;
    private void Start()
    {
        _currentHealth = MaxHealth;
        _animationPlayer = _owner.GetAbility<AnimationPlayer>();
    }

    public void TakeDamage(float damage)
    {
        if (!_photonView.IsMine) return;

        _currentHealth -= damage;
        _currentHealth = Mathf.Max(0, _currentHealth);
        _owner.GetAbility<PlayerUI>()?.UpdateHealthUI(_currentHealth, MaxHealth);
        HUDManager.Instance.UpdateHpUI(_currentHealth, MaxHealth);
        
        if (_currentHealth <= 0)
        {
            Die();
        }
        else
        {
            _animationPlayer.PlayAnimation(AnimTriggerParam.Hit);
        }
    }


    private void Die()
    {
        _animationPlayer.PlayAnimation(AnimTriggerParam.Die);
        _owner.GetAbility<PlayerAttack>().enabled = false;
        _owner.GetAbility<PlayerMovement>().enabled = false;
        _owner.GetAbility<PlayerRotateAbility>().enabled = false;
        _owner.GetAbility<PlayerStamina>().enabled = false;
        Debug.Log("플레이어 사망");
    }

}
