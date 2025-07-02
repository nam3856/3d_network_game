using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
// 스크립트
public class BearUI : MonoBehaviourPun
{
    public Image HealthBarImage;
    public void UpdateHealthUI(float current, float max)
    {
        if (HealthBarImage != null)
        {
            HealthBarImage.fillAmount = Mathf.Clamp01(current / max);
        }

        if (photonView != null)
            photonView.RPC(nameof(RPC_UpdateHealthUI), RpcTarget.All, current, max);
    }

    [PunRPC]
    public void RPC_UpdateHealthUI(float current, float max)
    {
        if (HealthBarImage != null)
        {
            HealthBarImage.fillAmount = Mathf.Clamp01(current / max);
        }
    }
}
