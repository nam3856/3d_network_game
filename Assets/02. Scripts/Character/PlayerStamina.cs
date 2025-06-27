using UnityEngine;

public class PlayerStamina : PlayerAbility
{
    private float _currentStamina;

    public float Current => _currentStamina;
    public float Max => _owner.PlayerStat.MaxStamina;

    private float _lastStamina = -1f;

    private float _recoverDelayTimer = 0f;

    protected override void Awake()
    {
        base.Awake();
        _currentStamina = Max;
    }

    protected override void Update()
    {
        base.Update();

        if (_photonView.IsMine)
        {
            if (_recoverDelayTimer > 0f)
            {
                _recoverDelayTimer -= Time.deltaTime;
            }
            else if (_currentStamina < Max)
            {
                _currentStamina += _owner.PlayerStat.StaminaRecoverPerSecond * Time.deltaTime;
                _currentStamina = Mathf.Min(_currentStamina, Max);
                if (Mathf.Abs(_currentStamina - _lastStamina) > 3f)
                {
                    _owner.GetAbility<PlayerUI>()?.UpdateStaminaUI(Current, Max);

                    HUDManager.Instance.UpdateStaminaUI(Current, Max);
                    _lastStamina = _currentStamina;
                }
            }
        }


    }

    public bool TryConsume(float amount)
    {
        if (_currentStamina >= amount && _photonView.IsMine)
        {
            _currentStamina -= amount;
            _recoverDelayTimer = _owner.PlayerStat.StaminaDelay;
            _owner.GetAbility<PlayerUI>()?.UpdateStaminaUI(Current, Max);
            HUDManager.Instance.UpdateStaminaUI(Current, Max);
            return true;
        }
        return false;
    }

    public void ForceConsume(float amount)
    {
        if (_photonView.IsMine)
        {
            _currentStamina = Mathf.Max(_currentStamina - amount, 0f);

            _owner.GetAbility<PlayerUI>()?.UpdateStaminaUI(Current, Max);
            HUDManager.Instance.UpdateStaminaUI(Current, Max);
        }
    }

    public void Recover(float amount)
    {
        if (_photonView.IsMine)
        {
            _currentStamina = Mathf.Min(_currentStamina + amount, Max);

            _owner.GetAbility<PlayerUI>()?.UpdateStaminaUI(Current, Max);
            HUDManager.Instance.UpdateStaminaUI(Current, Max);
        }
    }
}
