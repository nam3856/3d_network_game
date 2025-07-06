using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// 스크립트
public class MatchSelector : MonoBehaviourPunCallbacks
{

    [SerializeField] private Button _quickMatchButton;
    [SerializeField] private Button _customRoomButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private CanvasGroup _quickMatchCanvas;
    [SerializeField] private CanvasGroup _customRoomCanvas;
    [SerializeField] private QuickMatchManager _quickMatchManager;
    [SerializeField] private RoomListManager _roomListManager;
    [SerializeField] private TMP_InputField _nicknameInput;


    private void Awake()
    {
        _quickMatchButton.onClick.AddListener(QuickMatch);
        _customRoomButton.onClick.AddListener(CustomRoom);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        _nicknameInput.text = PhotonNetwork.NickName;
        _nicknameInput.gameObject.SetActive(true);
        _customRoomButton.gameObject.SetActive(true);
        _quickMatchButton.gameObject.SetActive(true);

        _backButton.gameObject.SetActive(false);
    }


    private void DisableButtons()
    {
        _nicknameInput.gameObject.SetActive(false);
        _customRoomButton.gameObject.SetActive(false);
        _quickMatchButton.gameObject.SetActive(false);

        _backButton.gameObject.SetActive(true);
    }

    private void QuickMatch()
    {
        _quickMatchCanvas.alpha = 1.0f;
        _quickMatchCanvas.blocksRaycasts = true;
        _quickMatchCanvas.interactable = true;
        _quickMatchManager.gameObject.SetActive(true);

        PhotonNetwork.NickName = _nicknameInput.text.Trim();
        DisableButtons();
        gameObject.SetActive(false);
    }

    private void CustomRoom()
    {
        _roomListManager.gameObject.SetActive(true);
        _customRoomCanvas.alpha = 1.0f;
        _customRoomCanvas.interactable = true;
        _customRoomCanvas.blocksRaycasts = true;

        PhotonNetwork.NickName = _nicknameInput.text.Trim();
        DisableButtons();
        gameObject.SetActive(false);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        _nicknameInput.text = PhotonNetwork.NickName;
        _nicknameInput.gameObject.SetActive(true);
        _customRoomButton.gameObject.SetActive(true);
        _quickMatchButton.gameObject.SetActive(true);
        _quickMatchManager.gameObject.SetActive(false);

        _customRoomCanvas.alpha = 0f;
        _customRoomCanvas.interactable = false;
        _customRoomCanvas.blocksRaycasts = false;
        _roomListManager.gameObject.SetActive(false);

        _quickMatchCanvas.alpha = 0f;
        _quickMatchCanvas.blocksRaycasts = false;
        _quickMatchCanvas.interactable = false;
        _backButton.gameObject.SetActive(false);
    }

}
