using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class RoomListItem : MonoBehaviour
{
    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI playerCountText;
    private RoomInfo _roomInfo;
    private System.Action<RoomInfo> _onClick;
    [SerializeField] private Image _lockIcon;

    public void Initialize(RoomInfo roomInfo, System.Action<RoomInfo> onClick)
    {
        _roomInfo = roomInfo;
        _onClick = onClick;


        if (_roomInfo.CustomProperties.ContainsKey("Password"))
        {
            string password = _roomInfo.CustomProperties["Password"] as string;
            if (!string.IsNullOrEmpty(password))
            {
                Debug.Log(roomInfo.CustomProperties["Password"]);
                ShowLockIcon();
            }
        }

        roomNameText.text = _roomInfo.Name;
        playerCountText.text = $"{_roomInfo.PlayerCount} / {_roomInfo.MaxPlayers}";
    }

    public void OnClick()
    {
        _onClick?.Invoke(_roomInfo);
    }

    public void ShowLockIcon()
    {
        _lockIcon.gameObject.SetActive(true);
    }
}
