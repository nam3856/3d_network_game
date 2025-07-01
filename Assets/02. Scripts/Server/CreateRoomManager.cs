using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Hashtable = ExitGames.Client.Photon.Hashtable;
// 스크립트
public class CreateRoomManager
{
    [SerializeField] private bool _isPrivate;

    private RoomOptions _customRoomOptions;

    public void TryCreateRoom(string roomName= null, int maxPlayers = 20, string password = null, bool breakIn = false)
    {
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = $"{PhotonNetwork.NickName}의 게임";
        }
        bool isOpen = true;
        bool isVisible = true;
        bool hasPassword = string.IsNullOrEmpty(password);
        _customRoomOptions = new RoomOptions
        {
            MaxPlayers = maxPlayers,
            IsVisible = isVisible,
            IsOpen = isOpen,
            CustomRoomProperties = new Hashtable
            {
                {"Host", PhotonNetwork.NickName },
                {"Password", password },
                {"HasPassword", hasPassword },
                {"BreakIn", breakIn }
            },
            CustomRoomPropertiesForLobby = new[] { "Host", "HasPassword", "Password", "BreakIn" },
            PublishUserId = true,
            CleanupCacheOnLeave = true
        };

        CreateRoom(_customRoomOptions, roomName);
    }

    public void CreateRoom(RoomOptions roomOptions = null, string roomName = "")
    {

        if (roomOptions == null)
        {
            // 룸 속성 정의
            roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 20;    // 룸에 입장할 수 있는 최대 접속자 수
            roomOptions.IsOpen = true;  // 룸의 오픈 여부
            roomOptions.IsVisible = true;  // 로비에서 룸 목록에 노출시킬지 여부
        }

        if (string.IsNullOrEmpty(roomName))
        {
            roomName = PhotonNetwork.NickName + "의 게임";
        }

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
}
