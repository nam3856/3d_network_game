using UnityEngine;
using UnityEngine.UI;

public class ServerMessageUI : MonoBehaviour
{
    [SerializeField] private GameObject _messageTextPrefab;
    [SerializeField] private Transform _messageContainer;
    [SerializeField] private ScrollRect _chatScrollRect;
    [SerializeField] private ChatUIFader _chatUIFader;
    private void Awake()
    {
        PhotonCustomRoomEventHandler.ServerEvent += OnServerEvent;
        InRoomUI.OnRoomMessage += OnServerEvent;
    }
    private void OnDestroy()
    {
        PhotonCustomRoomEventHandler.ServerEvent -= OnServerEvent;
        InRoomUI.OnRoomMessage -= OnServerEvent;
    }

    private void OnServerEvent(string message)
    {
        var serverMessageText = Instantiate(_messageTextPrefab, _messageContainer);
        serverMessageText.GetComponent<ChattingMessage>().Text = message;
        _chatUIFader?.FadeIn();
        ScrollToBottom();
    }
    public void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        _chatScrollRect.verticalNormalizedPosition = 0f;
    }
}
