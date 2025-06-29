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

    public void Initialize(RoomInfo roomInfo, System.Action<RoomInfo> onClick)
    {
        _roomInfo = roomInfo;
        _onClick = onClick;

        roomNameText.text = _roomInfo.Name;
        playerCountText.text = $"{_roomInfo.PlayerCount} / {_roomInfo.MaxPlayers}";
    }

    public void OnClick()
    {
        _onClick?.Invoke(_roomInfo);
    }
}
