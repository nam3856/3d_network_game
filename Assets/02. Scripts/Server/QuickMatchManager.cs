using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using System;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;
// 스크립트
public class QuickMatchManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private CanvasGroup _quickMatchCanvas;

    public static event Action<string> QuickMatchCallback;
    private TypedLobby _lobbyQuickMatching = new TypedLobby("QuickLobby", LobbyType.Default);
    private int _tryCount;
    public override void OnEnable()
    {
        base.OnEnable();
        QuickMatch();
    }

    public void QuickMatch()
    {
        PhotonNetwork.JoinLobby(_lobbyQuickMatching);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비 접속 완료!");

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4,
            IsOpen = true,
            IsVisible = true,
            CleanupCacheOnLeave = true
        };
        PhotonNetwork.JoinRandomOrCreateRoom(
            expectedCustomRoomProperties: null,
            expectedMaxPlayers: 4,
            matchingType: MatchmakingMode.FillRoom,
            typedLobby: _lobbyQuickMatching,
            sqlLobbyFilter: null,
            roomName: null,
            roomOptions: roomOptions
        );
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"CreatRoom Failed {returnCode}:{message}");
        _tryCount++;

        if (_tryCount > 5)
        {
            Debug.LogWarning("방 만들 수 없네");
            PhotonConnectionManager.Instance.Disconnect();
            return;
        }
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4,
            IsOpen = true,
            IsVisible = true,
            CleanupCacheOnLeave = true
        };

        PhotonNetwork.JoinRandomOrCreateRoom(
            expectedCustomRoomProperties: null,
            expectedMaxPlayers: 4,
            matchingType: MatchmakingMode.FillRoom,
            typedLobby: _lobbyQuickMatching,
            sqlLobbyFilter: null,
            roomName: null,
            roomOptions: roomOptions
        );
    }

    
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentLobby.Name == "CustomLobby") return;
        _tryCount = 0;
        Debug.Log("룸 입장 완료!");
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
        Debug.Log($"MaxPlayer = {PhotonNetwork.CurrentRoom.MaxPlayers}");

        TryStart();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentLobby.Name == "CustomLobby") return;
        TryStart();
    }

    private void TryStart()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable 
            {
                { "CurrentScene", 1 }
            });
            StartCoroutine(StartGameCountdown());
        }
    }

    [PunRPC]
    private void ShowCountdownMessage(int secondsLeft)
    {
        QuickMatchCallback?.Invoke($"{secondsLeft}초 후 게임이 시작됩니다.");
    }

    private IEnumerator StartGameCountdown()
    {
        int countdown = 5;

        while (countdown > 0)
        {
            // 모든 클라이언트에게 메시지 표시
            photonView.RPC(nameof(ShowCountdownMessage), RpcTarget.All, countdown);
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        // 씬 전환
        PhotonNetwork.LoadLevel(1); // 자동으로 모든 클라이언트 이동
    }

}
