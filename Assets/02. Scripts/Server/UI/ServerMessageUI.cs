using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ServerMessageUI : MonoBehaviour
{
    [SerializeField] private GameObject _messageTextPrefab;
    [SerializeField] private Transform _messageContainer;
    [SerializeField] private ScrollRect _chatScrollRect;
    [SerializeField] private ChatUIFader _chatUIFader;
    private void OnEnable()
    {
        PhotonCustomRoomEventHandler.ServerEvent += OnServerEvent;
        PlayerHealth.OnDied += HandlePlayerDied;
        InRoomUI.OnRoomMessage += OnServerEvent;
    }
    private void OnDisable()
    {
        PhotonCustomRoomEventHandler.ServerEvent -= OnServerEvent;
        InRoomUI.OnRoomMessage -= OnServerEvent;
        PlayerHealth.OnDied -= HandlePlayerDied;
    }

    private void HandlePlayerDied((int, int) data)
    {
        int killerId = data.Item1;
        int victimId = data.Item2;

        string victimPlayer = PhotonNetwork.PlayerList[victimId - 1].NickName;

        string killerPlayer = PhotonNetwork.PlayerList[killerId - 1].NickName;
        var serverMessageText = Instantiate(_messageTextPrefab, _messageContainer);
        serverMessageText.GetComponent<ChattingMessage>().Text = $"{killerPlayer} ´ÔÀÌ {victimPlayer} ´ÔÀ» Á×¿´½À´Ï´Ù.";
        Debug.Log($"{killerPlayer} ´ÔÀÌ {victimPlayer} ´ÔÀ» Á×¿´½À´Ï´Ù.");

        _chatUIFader?.FadeIn();
        ScrollToBottom();
        //var serverMessageText = Instantiate(_messageTextPrefab, _messageContainer);
        //serverMessageText.GetComponent<ChattingMessage>().Text = $"{victimPlayer} ´ÔÀÌ ¾îµò°¡¿¡¼­ ¾µ¾µÈ÷ Á×¾ú½À´Ï´Ù.";

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
