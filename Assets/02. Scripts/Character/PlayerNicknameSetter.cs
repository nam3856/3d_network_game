using TMPro;
using UnityEngine;
public class PlayerNicknameSetter : PlayerAbility
{
    public TextMeshProUGUI NicknameTextUI;

    private void Start()
    {
        NicknameTextUI.text = _photonView.Owner.NickName;
        if (_photonView.IsMine)
        {
            NicknameTextUI.color = Color.green;
        }
        else
        {
            NicknameTextUI.color = Color.red;
        }
    }

}
