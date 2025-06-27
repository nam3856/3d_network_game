using Photon.Pun;
using UnityEngine;
public class PlayerHealth : PlayerAbility
{
    private float _currentHealth;
    private float MaxHealth => _owner.PlayerStat.MaxHealth;

    private Animator _animator;
    private void Start()
    {
        _currentHealth = MaxHealth;
        _animator = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        if (!_photonView.IsMine) return;

        _currentHealth -= damage;
        _currentHealth = Mathf.Max(0, _currentHealth);
        _owner.GetAbility<PlayerUI>()?.UpdateHealthUI(_currentHealth, MaxHealth);

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _animator.SetTrigger("Die");
        _owner.GetAbility<PlayerAttack>().enabled = false;
        _owner.GetAbility<PlayerMovement>().enabled = false;
        _owner.GetAbility<PlayerRotateAbility>().enabled = false;
        _owner.GetAbility<PlayerStamina>().enabled = false;
        Debug.Log("플레이어 사망");
    }

}
