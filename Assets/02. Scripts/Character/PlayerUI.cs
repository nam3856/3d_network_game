using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerUI : PlayerAbility
{
    public TextMeshProUGUI NicknameTextUI;
    public TextMeshProUGUI MinimapNicknameTextUI;
    public Image[] MinimapIcons;
    public Image StaminaBarImage;
    public Image HealthBarImage;

    private void Start()
    {
        NicknameTextUI.text = _photonView.Owner.NickName;
        HUDManager.Instance.SetNickname(_photonView.Owner.NickName);
        MinimapNicknameTextUI.text = _photonView.Owner.NickName;
        if (_photonView.IsMine)
        {
            NicknameTextUI.color = Color.green;
            foreach (var minimapIcon in MinimapIcons)
            {
                minimapIcon.color = Color.green;
            }
        }
        else
        {
            NicknameTextUI.color = Color.red;
            foreach (var minimapIcon in MinimapIcons)
            {
                minimapIcon.color = Color.red;
            }
            MinimapNicknameTextUI.color = Color.white;
        }
    }
    public void UpdateStaminaUI(float current, float max)
    {
        if (StaminaBarImage != null)
        {
            StaminaBarImage.fillAmount = Mathf.Clamp01(current / max);
        }
        if (_photonView != null)
            _photonView.RPC(nameof(RPC_UpdateStaminaUI), RpcTarget.All, current, max);
    }

    public void UpdateHealthUI(float current, float max)
    {
        if (HealthBarImage != null)
        {
            HealthBarImage.fillAmount = Mathf.Clamp01(current / max);
        }

        if(_photonView!=null)
            _photonView.RPC(nameof(RPC_UpdateHealthUI), RpcTarget.All, current, max);
    }

    [PunRPC]
    public void RPC_UpdateHealthUI(float current, float max)
    {
        if (HealthBarImage != null)
        {
            HealthBarImage.fillAmount = Mathf.Clamp01(current / max);
        }
    }

    [PunRPC]
    public void RPC_UpdateStaminaUI(float current, float max)
    {
        if (StaminaBarImage != null)
            StaminaBarImage.fillAmount = Mathf.Clamp01(current / max);
    }
}
