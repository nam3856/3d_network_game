using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using RainbowArt.CleanFlatUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class RoomListManager : MonoBehaviourPunCallbacks
{
    public static RoomListManager Instance;

    [Header("Lobbies")]
    private TypedLobby _lobbyCustomRoom = new TypedLobby("CustomLobby", LobbyType.SqlLobby);

    [SerializeField] private GameObject _roomListItemPrefab;
    [SerializeField] private Transform _roomListContent;
    [SerializeField] private RoomDetailPanel _detailPanel;
    [SerializeField] private Button _refreshButton;
    [SerializeField] private GameObject _loadingIndicator;
    [SerializeField] private Button _joinRoomButton;
    [SerializeField] private Button _joinRandomButton;
    [SerializeField] private Button _createRoomButton;
    [SerializeField] private CanvasGroup _customRoomCanvas;

    [SerializeField] private Button _confirmPasswordButton;
    [SerializeField] private GameObject _passwordPopup;
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] private GameObject _passwordWrongPopup;

    private string _pendingRoomName;
    private string _expectedPassword;

    private RoomInfo _selectedRoomInfo;
    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    private List<GameObject> _roomListItems = new List<GameObject>();
    private Coroutine _autoRefreshCoroutine;
    private readonly WaitForSeconds _refreshInterval = new WaitForSeconds(30f);
    private bool _isRefreshing = false;

    private void Awake()
    {
        _refreshButton.onClick.AddListener(OnClickRefreshRoomList);
        _joinRoomButton.onClick.AddListener(OnClickJoinRoom);
        _joinRandomButton.onClick.AddListener(OnClickJoinRandom);
        _createRoomButton.onClick.AddListener(OnClickCreateRoom);
    }
    private void OnClickRefreshRoomList()
    {
        if (PhotonNetwork.InLobby)
        {
            cachedRoomList.Clear();
            PhotonNetwork.GetCustomRoomList(PhotonNetwork.CurrentLobby, "1=1");
            if (_loadingIndicator != null) _loadingIndicator.SetActive(true);
            if (_refreshButton != null) _refreshButton.interactable = false;
        }
    }

    private void OnClickCreateRoom()
    {
        _selectedRoomInfo = null;
        _customRoomCanvas.alpha = 0f;
        _customRoomCanvas.interactable = false;
        _customRoomCanvas.blocksRaycasts = false;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        CustomRoom();
    }

    private void CustomRoom()
    {
        PhotonNetwork.JoinLobby(_lobbyCustomRoom);
    }


    // 로비에 접속한 후 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        Debug.Log("로비 접속 완료!");
        Debug.Log(PhotonNetwork.InLobby);

        _customRoomCanvas.alpha = 1f;
        _customRoomCanvas.interactable = true;
        _customRoomCanvas.blocksRaycasts = true;
        cachedRoomList.Clear();
        PhotonNetwork.GetCustomRoomList(PhotonNetwork.CurrentLobby, "1=1");

        if (_autoRefreshCoroutine != null)
            StopCoroutine(_autoRefreshCoroutine);
        _autoRefreshCoroutine = StartCoroutine(AutoRefreshRoomList());
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);
        RefreshRoomListUI();
        _isRefreshing = false;
        if (_loadingIndicator != null) _loadingIndicator.SetActive(false);
        if (_refreshButton != null) _refreshButton.interactable = true;
    }

    private void RefreshRoomListUI()
    {
        foreach (var item in _roomListItems)
            Destroy(item);
        _roomListItems.Clear();

        foreach (var roomInfo in cachedRoomList.Values)
        {
            GameObject obj = Instantiate(_roomListItemPrefab, _roomListContent);
            RoomListItem item = obj.GetComponent<RoomListItem>();
            item.Initialize(roomInfo, OnRoomItemClicked);
            _roomListItems.Add(obj);
        }
    }

    private IEnumerator AutoRefreshRoomList()
    {
        while (PhotonNetwork.InLobby)
        {
            if (!_isRefreshing)
            {
                _isRefreshing = true;
                if (_loadingIndicator != null) _loadingIndicator.SetActive(true);
                PhotonNetwork.GetCustomRoomList(PhotonNetwork.CurrentLobby, "1=1");
            }

            yield return _refreshInterval;
        }
    }

    private void OnRoomItemClicked(RoomInfo roomInfo)
    {
        _selectedRoomInfo = roomInfo;

        string hostName = roomInfo.CustomProperties.TryGetValue("Host", out var host)
            ? host.ToString() : "알 수 없음";

        _detailPanel.SetRoomDetail(roomInfo.Name, roomInfo.PlayerCount, roomInfo.MaxPlayers, hostName);
        _joinRoomButton.interactable = true;
    }
    private void OnClickJoinRoom()
    {
        if (_selectedRoomInfo == null)
        {
            Debug.LogWarning("선택된 방이 없습니다.");
            return;
        }

        if (_selectedRoomInfo.CustomProperties.ContainsKey("Password"))
        {
            string password = _selectedRoomInfo.CustomProperties["Password"] as string;
            if (!string.IsNullOrEmpty(password))
            {
                // 비밀번호 방이면 입력창 띄우기
                ShowPasswordPopup(_selectedRoomInfo.Name, _selectedRoomInfo.CustomProperties["Password"].ToString());
            }
            else
            {
                PhotonNetwork.JoinRoom(_selectedRoomInfo.Name);
                SetButtonsInteractable(false);
            }
        }
        else
        {
            // 일반 방이면 바로 입장
            PhotonNetwork.JoinRoom(_selectedRoomInfo.Name);
            SetButtonsInteractable(false);
        }
    }

    private void ShowPasswordPopup(string name, string password)
    {
        _pendingRoomName = name;
        _expectedPassword = password;
        _passwordPopup.SetActive(true);
        _passwordInput.text = "";
        _confirmPasswordButton.onClick.RemoveAllListeners();
        _confirmPasswordButton.onClick.AddListener(ConfirmPassword);
    }

    private void ConfirmPassword()
    {
        if (_passwordInput.text == _expectedPassword)
        {
            PhotonNetwork.JoinRoom(_pendingRoomName);
            SetButtonsInteractable(false);
            _passwordPopup.SetActive(false);
        }
        else
        {
            _passwordWrongPopup.SetActive(true);
        }
    }

    public void OnClickOk()
    {
        _passwordWrongPopup.SetActive(false);
    }

    private void OnClickJoinRandom()
    {
        SetButtonsInteractable(false);
        var expectedProps = new Hashtable
        {
            {"HasPassword", false },
            { "Password", null }
        };
        PhotonNetwork.JoinRandomRoom(expectedProps, 0);
    }

    private void SetButtonsInteractable(bool value)
    {
        _joinRoomButton.interactable = value;
        _joinRandomButton.interactable = value;
        _createRoomButton.interactable = value;
        _refreshButton.interactable = value;
    }

    // 랜덤 룸 입장에 실패했을 경우 호출되는 콜백 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");

        if (returnCode == ErrorCode.GameDoesNotExist) // 룸이 없을 경우
        {
            var cManager = new CreateRoomManager();
            cManager.CreateRoom();
            return;
        }

        SetButtonsInteractable(true);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // 버튼 복구
        SetButtonsInteractable(true);
        PhotonNetwork.GetCustomRoomList(PhotonNetwork.CurrentLobby, "1=1");

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

    public override void OnJoinedRoom()
    {
        _selectedRoomInfo = null;
        _customRoomCanvas.alpha = 0f;
        _customRoomCanvas.interactable = false;
        _customRoomCanvas.blocksRaycasts = false;
    }


    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();

        if (_autoRefreshCoroutine != null)
        {
            StopCoroutine(_autoRefreshCoroutine);
            _autoRefreshCoroutine = null;
        }

        SetButtonsInteractable(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        cachedRoomList.Clear();

        if (_autoRefreshCoroutine != null)
        {
            StopCoroutine(_autoRefreshCoroutine);
            _autoRefreshCoroutine = null;
        }
    }
}
