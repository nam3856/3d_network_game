using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
// 스크립트
public class MatchSelector : MonoBehaviourPunCallbacks
{

    [SerializeField] private Button _quickMatchButton;
    [SerializeField] private Button _customRoomButton;
    [SerializeField] private CanvasGroup _quickMatchCanvas;
    [SerializeField] private CanvasGroup _customRoomCanvas;
    [SerializeField] private QuickMatchManager _quickMatchManager;
    [SerializeField] private RoomListManager _roomListManager;


    private void Awake()
    {
        _quickMatchButton.onClick.AddListener(QuickMatch);
        _customRoomButton.onClick.AddListener(CustomRoom);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        _customRoomButton.gameObject.SetActive(true);
        _quickMatchButton.gameObject.SetActive(true);
    }


    private void DisableButtons()
    {
        _customRoomButton.gameObject.SetActive(false);
        _quickMatchButton.gameObject.SetActive(false);
    }

    private void QuickMatch()
    {
        DisableButtons();
        _quickMatchCanvas.alpha = 1.0f;
        _quickMatchCanvas.blocksRaycasts = true;
        _quickMatchCanvas.interactable = true;
        _quickMatchManager.gameObject.SetActive(true);

    }

    private void CustomRoom()
    {
        DisableButtons();
        _roomListManager.gameObject.SetActive(true);

        _customRoomCanvas.alpha = 1.0f;
        _customRoomCanvas.interactable = true;
        _customRoomCanvas.blocksRaycasts = true;
    }
}
