using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Player = Photon.Realtime.Player;
public class PhotonServerManager : MonoBehaviourPunCallbacks
{
    private bool _isCustomRoom = false;
    public static event Action<string> ServerEvent;
    private readonly string _version = "0.0.1";
    // Major, Minor, Patch
    //<전체를 뒤엎을 변화>.<새로운 기능 추가>.<버그 수정>

    TypedLobby _lobbyQuickMatching = new TypedLobby("QuickLobby", LobbyType.Default);
    TypedLobby _lobbyCustomRoom = new TypedLobby("CustomLobby", LobbyType.Default);


    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    public static PhotonServerManager Instance;
    public Button startGameButton;
    public Button QuickMatchButton;
    public Button CustomRoomButton;
    [SerializeField] private GameObject roomListItemPrefab;
    [SerializeField] private Transform roomListContent;
    [SerializeField] private RoomDetailPanel detailPanel;
    [SerializeField] private Button refreshButton;
    private List<GameObject> roomListItems = new List<GameObject>();
    private Coroutine autoRefreshCoroutine;
    private readonly WaitForSeconds refreshInterval = new WaitForSeconds(30f);
    private bool _isRefreshing = false;
    [SerializeField] private GameObject loadingIndicator;
    private RoomInfo _selectedRoomInfo;
    [SerializeField] private Button joinRoomButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        QuickMatchButton.onClick.AddListener(QuickMatch);
        CustomRoomButton.onClick.AddListener(CustomRoom);
        refreshButton.onClick.AddListener(OnClickRefreshRoomList);
        joinRoomButton.onClick.AddListener(OnClickJoinRoom);
    }
    private void OnClickRefreshRoomList()
    {
        if (_isCustomRoom && PhotonNetwork.InLobby)
        {
            cachedRoomList.Clear();
            PhotonNetwork.GetCustomRoomList(PhotonNetwork.CurrentLobby, "1=1");
            if (loadingIndicator != null) loadingIndicator.SetActive(true);
            if (refreshButton != null) refreshButton.interactable = false;
        }
    }
    private void QuickMatch()
    {
        CustomRoomButton.gameObject.SetActive(false);
        QuickMatchButton.gameObject.SetActive(false);
        _isCustomRoom = false;
        PhotonNetwork.JoinLobby(_lobbyQuickMatching);
    }

    private void CustomRoom()
    {
        CustomRoomButton.gameObject.SetActive(false);
        QuickMatchButton.gameObject.SetActive(false);
        _isCustomRoom = true;
        PhotonNetwork.JoinLobby(_lobbyCustomRoom);
    }

    private string _nickName = "Player";
    private void Start()
    {
        PhotonNetwork.GameVersion = _version;
        PhotonNetwork.NickName = _nickName + UnityEngine.Random.Range(0, 1000);

        // 방장이 로드한 씬 게임에 참여한 다른 사용자들이 똑같이 로드할 수 있도록 동기화 해주는 옵션
        // 방장(마스터 클라이언트): 방을 만든 소유자. (방에는 하나의 마스터 클라이언트만 존재)
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 30;

        // 설정값들을 이용해 서버 접속 시도
        // 정확히는 네임 서버로 접속 시도
        PhotonNetwork.ConnectUsingSettings();
    }

    // 네임 서버에 접속한 후 호출되는 콜백 함수
    public override void OnConnected()
    {
        base.OnConnected();
        ServerEvent?.Invoke($"마스터 서버에 접속하는 중...");
    }

    // 마스터 서버에 접속한 후 호출되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        ServerEvent?.Invoke($"{PhotonNetwork.NickName} 님 어서오세요!");
        Debug.Log("마스터 서버에 접속했습니다.");
        Debug.Log("현재 플레이어 닉네임: " + PhotonNetwork.NickName);


        CustomRoomButton.gameObject.SetActive(true);
        QuickMatchButton.gameObject.SetActive(true);
    }

    // 로비에 접속한 후 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        Debug.Log("로비 접속 완료!");
        Debug.Log(PhotonNetwork.InLobby);

        if (_isCustomRoom)
        {
            // 커스텀룸: 방 리스트 초기화 및 요청
            cachedRoomList.Clear();
            PhotonNetwork.GetCustomRoomList(PhotonNetwork.CurrentLobby, "1=1");

            if (autoRefreshCoroutine != null)
                StopCoroutine(autoRefreshCoroutine);
            autoRefreshCoroutine = StartCoroutine(AutoRefreshRoomList());
        }
        else
        {
            // 퀵매치: 랜덤 룸 입장 시도
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);
        if (_isCustomRoom)
        {
            RefreshRoomListUI();
            _isRefreshing = false;
            if (loadingIndicator != null) loadingIndicator.SetActive(false);
            if (refreshButton != null) refreshButton.interactable = true;
        }
    }
    private void RefreshRoomListUI()
    {
        foreach (var item in roomListItems)
            Destroy(item);
        roomListItems.Clear();

        foreach (var roomInfo in cachedRoomList.Values)
        {
            GameObject obj = Instantiate(roomListItemPrefab, roomListContent);
            RoomListItem item = obj.GetComponent<RoomListItem>();
            item.Initialize(roomInfo, OnRoomItemClicked);
            roomListItems.Add(obj);
        }
    }

    private IEnumerator AutoRefreshRoomList()
    {
        while (_isCustomRoom && PhotonNetwork.InLobby)
        {
            if (!_isRefreshing)
            {
                _isRefreshing = true;
                if (loadingIndicator != null) loadingIndicator.SetActive(true);
                PhotonNetwork.GetCustomRoomList(PhotonNetwork.CurrentLobby, "1=1");
            }

            yield return refreshInterval;
        }
    }

    private void OnRoomItemClicked(RoomInfo roomInfo)
    {
        _selectedRoomInfo = roomInfo;

        string hostName = roomInfo.CustomProperties.TryGetValue("Host", out var host)
            ? host.ToString() : "알 수 없음";

        detailPanel.SetRoomDetail(roomInfo.Name, roomInfo.PlayerCount, roomInfo.MaxPlayers, hostName);
        joinRoomButton.interactable = true;
    }
    private void OnClickJoinRoom()
    {
        if (_selectedRoomInfo == null)
        {
            Debug.LogWarning("선택된 방이 없습니다.");
            return;
        }

        PhotonNetwork.JoinRoom(_selectedRoomInfo.Name);
        ServerEvent?.Invoke($"'{_selectedRoomInfo.Name}' 방에 참가를 시도합니다...");
        joinRoomButton.interactable = false; // 연타 방지
    }

    // 랜덤 룸 입장에 실패했을 경우 호출되는 콜백 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");

        if (returnCode == ErrorCode.GameDoesNotExist) // 룸이 없을 경우
        {
            ServerEvent?.Invoke($"방 생성을 시도중입니다...");
            CreateRoom();
        }
        else if(returnCode == ErrorCode.GameFull) // 최대 접속자 수 초과
        {
            ServerEvent?.Invoke("현재 접속자가 너무 많습니다. 잠시 후 다시 시도해주세요.");
        }
        else
        {
            ServerEvent?.Invoke($"룸 입장 실패: {message}");
        }
    }

    private static void CreateRoom()
    {
        // 룸 속성 정의
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;    // 룸에 입장할 수 있는 최대 접속자 수
        roomOptions.IsOpen = true;  // 룸의 오픈 여부
        roomOptions.IsVisible = true;  // 로비에서 룸 목록에 노출시킬지 여부
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable{
    { "Host", PhotonNetwork.NickName }
};
        roomOptions.CustomRoomPropertiesForLobby = new[] { "Host" };
        // 룸 생성
        PhotonNetwork.CreateRoom("test", roomOptions);
        // 룸 입장 또는 생성
        // PhotonNetwork.JoinOrCreateRoom("test", roomOptions, TypedLobby.Default);
    }

    // 룸 생성에 실패했을 경우 호출되는 콜백 함수
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ServerEvent?.Invoke($"방 생성 실패: {message}");
        Debug.Log($"CreatRoom Failed {returnCode}:{message}");
    }

    // 룸 생성이 완료된 후 호출되는 콜백 함수
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        // 생성된 룸 이름 확인
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }


    // 룸에 입장한 후 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        _selectedRoomInfo = null;
        joinRoomButton.interactable = false;
        if (_isCustomRoom)
        {
            ServerEvent?.Invoke($@"'{PhotonNetwork.CurrentRoom.Name}' 방에 입장했습니다.");
        }
        Debug.Log("룸 입장 완료!");
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

        // 룸에 접속한 사용자 정보
        Dictionary<int, Player> roomPlayers = PhotonNetwork.CurrentRoom.Players;
        foreach (KeyValuePair<int, Player> player in roomPlayers)
        {
            Debug.Log($"{player.Value.NickName} : {player.Value.ActorNumber}");
        }

        UpdateStartGameButton();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"방 참가 실패: {returnCode} - {message}");

        // 버튼 복구
        joinRoomButton.interactable = true;
        PhotonNetwork.GetCustomRoomList(PhotonNetwork.CurrentLobby, "1=1");
        switch (returnCode)
        {
            case ErrorCode.GameFull:
                ServerEvent?.Invoke("참가하려는 방이 이미 가득 찼습니다.");
                break;

            case ErrorCode.GameClosed:
                ServerEvent?.Invoke("해당 방은 이미 닫혔거나 게임이 시작되었습니다.");
                break;

            case ErrorCode.GameDoesNotExist:
                ServerEvent?.Invoke("해당 방은 더 이상 존재하지 않습니다.");
                break;

            default:
                ServerEvent?.Invoke($"방 참가 실패: {message}");
                break;
        }

        // 선택 초기화
        _selectedRoomInfo = null;
    }


    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} 님이 방에 입장했습니다.");
        if (_isCustomRoom)
        {
            ServerEvent?.Invoke($"{newPlayer.NickName} 님이 입장했습니다.");
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} 님이 퇴장했습니다.");
        if (_isCustomRoom)
        {
            ServerEvent?.Invoke($"{otherPlayer.NickName} 님이 퇴장했습니다.");
        }
    }
    [PunRPC]
    private void ShowCountdownMessage(int secondsLeft)
    {
        ServerEvent?.Invoke($"{secondsLeft}초 후 게임이 시작됩니다...");
    }

    public void TryStartGame()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            ServerEvent?.Invoke("방장만 게임을 시작할 수 있습니다.");
            return;
        }

        startGameButton.interactable = false;
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
        ServerEvent?.Invoke($"방장이 떠나 {newMasterClient.NickName} 님이 방장이 되었습니다.");

        // 게임 시작전에만 버튼 업데이트
        UpdateStartGameButton();
    }

    private void UpdateStartGameButton()
    {
        if (!_isCustomRoom || startGameButton == null) return;
        if (PhotonNetwork.IsMasterClient)
            startGameButton.gameObject.SetActive(true);
        else
            startGameButton.gameObject.SetActive(false);
    }

    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();

        if (autoRefreshCoroutine != null)
        {
            StopCoroutine(autoRefreshCoroutine);
            autoRefreshCoroutine = null;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        cachedRoomList.Clear();

        if (autoRefreshCoroutine != null)
        {
            StopCoroutine(autoRefreshCoroutine);
            autoRefreshCoroutine = null;
        }
    }
}
