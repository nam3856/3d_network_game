using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
public class InRoomUI : MonoBehaviourPunCallbacks
{
    public static event Action<string> OnRoomMessage;
    [Header("In Room")]
    [SerializeField] private Button _startGameButton;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform _playerContainer;
    [SerializeField] private CanvasGroup _roomCanvasGroup;
    [SerializeField] private TextMeshProUGUI _roomText;
    private Dictionary<string, GameObject> _players = new Dictionary<string, GameObject>();


    private void Awake()
    {
        _startGameButton.onClick.AddListener(TryStartGame);
    }
    // 룸에 입장한 후 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentLobby.Name == "QuickLobby") return;
        Debug.Log("룸 입장 완료!");
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
        
        _roomCanvasGroup.alpha = 1;
        _roomCanvasGroup.blocksRaycasts = true;
        _roomCanvasGroup.interactable = true;
        _roomText.text = PhotonNetwork.CurrentRoom.Name;
        // 룸에 접속한 사용자 정보
        Dictionary<int, Player> roomPlayers = PhotonNetwork.CurrentRoom.Players;
        foreach (KeyValuePair<int, Player> player in roomPlayers)
        {
            Debug.Log($"{player.Value.NickName} : {player.Value.ActorNumber}");
            var playerSet = Instantiate(_playerPrefab, _playerContainer).GetComponent<PlayerlistItem>();
            playerSet.SetNickname(player.Value);
        }


        UpdateStartGameButton();
    }

    
    [PunRPC]
    private void ShowCountdownMessage(int secondsLeft)
    {
        OnRoomMessage?.Invoke($"{secondsLeft}초 후 게임이 시작됩니다...");
    }

    public void TryStartGame()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties["BreakIn"].Equals(false))
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        _startGameButton.interactable = false;
        StartCoroutine(StartGameCountdown());
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

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.CurrentLobby.Name == "QuickLobby") return;
        OnRoomMessage?.Invoke($"방장이 떠나 {newMasterClient.NickName} 님이 방장이 되었습니다.");

        // 게임 시작전에만 버튼 업데이트
        UpdateStartGameButton();
    }

    private void UpdateStartGameButton()
    {
        if (_startGameButton == null) return;
        if (PhotonNetwork.IsMasterClient)
            _startGameButton.gameObject.SetActive(true);
        else
            _startGameButton.gameObject.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentLobby.Name == "QuickLobby") return;
        OnRoomMessage?.Invoke($"{newPlayer.NickName} 님이 입장했습니다.");

        var player = Instantiate(_playerPrefab, _playerContainer);
        var playerSetting = player.GetComponent<PlayerlistItem>();
        playerSetting.SetNickname(newPlayer);
        if (!string.IsNullOrEmpty(newPlayer.UserId) && !_players.ContainsKey(newPlayer.UserId))
        {
            _players.Add(newPlayer.UserId, player);
        }
        else
        {
            Debug.LogWarning($"[OnPlayerEnteredRoom] UserId가 null이거나 중복됨: {newPlayer.NickName}");
            Debug.Log(newPlayer.UserId);
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.CurrentLobby.Name == "QuickLobby") return;
        OnRoomMessage?.Invoke($"{otherPlayer.NickName} 님이 퇴장했습니다.");


        if (_players.TryGetValue(otherPlayer.UserId, out var go))
        {
            _players.Remove(otherPlayer.UserId);
            Destroy(go);
        }
        else
        {
            Debug.LogWarning($"[OnPlayerLeftRoom] UserId '{otherPlayer.UserId}'가 _players에 존재하지 않음.");
        }

    }

    public override void OnLeftRoom()
    {
        _roomCanvasGroup.alpha = 0;
        _roomCanvasGroup.blocksRaycasts = false;
        _roomCanvasGroup.interactable = false;
        foreach (var player in _players.Values)
        {
            Destroy(player);
        }
        _players.Clear();
    }
}
