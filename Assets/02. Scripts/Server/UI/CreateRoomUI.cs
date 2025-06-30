using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// 스크립트
public class CreateRoomUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _createRoomButton;
    [SerializeField] private CanvasGroup _createCanvasGroup;
    [SerializeField] private TMP_InputField _roomNameInput;
    [SerializeField] private Toggle _privateToggle;
    [SerializeField] private Toggle _breakInToggle;
    [SerializeField] private bool _isPrivate;
    [SerializeField] private bool _canBreakIn;
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] private Button _confirmButton;
    private CreateRoomManager _roomManager = new();
    private void Awake()
    {
        _createRoomButton.onClick.AddListener(ShowCreateMenu);
        _confirmButton.onClick.AddListener(TryCreateRoom);
        _privateToggle.onValueChanged.AddListener(SetRoomPrivate);
        _breakInToggle.onValueChanged.AddListener(SetBreakIn);
    }

    private void SetRoomPrivate(bool isPrivate)
    {
        _isPrivate = isPrivate;
        _passwordInput.gameObject.SetActive(isPrivate);
    }
    private void SetBreakIn(bool canBreakIn)
    {
        _canBreakIn = canBreakIn;
    }

    private void ShowCreateMenu()
    {
        _createCanvasGroup.alpha = 1.0f;
        _createCanvasGroup.blocksRaycasts = true;
        _createCanvasGroup.interactable = true;
    }

    private void HideCreateMenu()
    {
        _createCanvasGroup.alpha = 0.0f;
        _createCanvasGroup.blocksRaycasts = false;
        _createCanvasGroup.interactable = false;
    }

    private void TryCreateRoom()
    {
        string password = null;
        if (_isPrivate)
        {
            if (string.IsNullOrEmpty(_passwordInput.text))
            {
                Debug.LogWarning("패스워드가 없음");
                return;
            }

            password = _passwordInput.text;
        }

        string roomName = _roomNameInput.text;
        _roomManager.TryCreateRoom(roomName, maxPlayers: 20, password, _canBreakIn);
    }

    // 룸 생성에 실패했을 경우 호출되는 콜백 함수
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //ServerEvent?.Invoke($"방 생성 실패: {message}");
        Debug.Log($"CreatRoom Failed {returnCode}:{message}");
    }

    // 룸 생성이 완료된 후 호출되는 콜백 함수
    public override void OnCreatedRoom()
    {
        HideCreateMenu();
        Debug.Log("Created Room");
        // 생성된 룸 이름 확인
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }
}
