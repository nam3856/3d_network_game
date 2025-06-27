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

    private void Start()
    {
        NicknameTextUI.text = _photonView.Owner.NickName;
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

        _photonView.RPC("RPC_UpdateStaminaUI", RpcTarget.All, current, max);
    }

    [PunRPC]
    public void RPC_UpdateStaminaUI(float current, float max)
    {
        if (StaminaBarImage != null)
            StaminaBarImage.fillAmount = Mathf.Clamp01(current / max);
    }
}
