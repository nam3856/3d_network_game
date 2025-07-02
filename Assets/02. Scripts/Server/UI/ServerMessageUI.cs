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
        PlayerContext.OnScoreAdded += HandleScoreUpdated;
        InRoomUI.OnRoomMessage += OnServerEvent;
    }
    private void OnDisable()
    {
        PhotonCustomRoomEventHandler.ServerEvent -= OnServerEvent;
        InRoomUI.OnRoomMessage -= OnServerEvent;
        PlayerHealth.OnDied -= HandlePlayerDied;

        PlayerContext.OnScoreAdded -= HandleScoreUpdated;
    }

    private void HandleScoreUpdated(int score)
    {
        var serverMessageText = Instantiate(_messageTextPrefab, _messageContainer);
        serverMessageText.GetComponent<ChattingMessage>().Text = $"{score}�� ȹ��!";

        _chatUIFader?.FadeIn();
        ScrollToBottom();
    }

    private void HandlePlayerDied((int, int) data)
    {
        int killerId = data.Item1;
        int victimId = data.Item2;

        string victimPlayer = PhotonNetwork.CurrentRoom.GetPlayer(victimId)?.NickName ?? "???";


        var serverMessageText = Instantiate(_messageTextPrefab, _messageContainer);
        if (killerId > 0)
        {
            string killerPlayer = PhotonNetwork.CurrentRoom.GetPlayer(killerId)?.NickName ?? "???";
            serverMessageText.GetComponent<ChattingMessage>().Text = $"{killerPlayer} ���� {victimPlayer} ���� �׿����ϴ�.";
            Debug.Log($"{killerPlayer} ���� {victimPlayer} ���� �׿����ϴ�.");
        }
        else
        {
            serverMessageText.GetComponent<ChattingMessage>().Text = $"{victimPlayer} ���� ��򰡿��� ������ �׾����ϴ�.";
        }

        _chatUIFader?.FadeIn();
        ScrollToBottom();

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
