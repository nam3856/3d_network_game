using Photon.Pun;
using UnityEngine;
public class PlayerHealth : PlayerAbility
{
    private float _currentHealth;
    private float MaxHealth => _owner.PlayerStat.MaxHealth;
    private void Start()
    {
        _currentHealth = MaxHealth;
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
        Debug.Log("플레이어 사망");
    }

}
