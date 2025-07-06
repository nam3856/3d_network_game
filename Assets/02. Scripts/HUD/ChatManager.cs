using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System;

public class ChatManager : MonoBehaviourPun
{
    [SerializeField] private TMP_InputField _chatInputField;
    [SerializeField] private Button _chatButton;
    [SerializeField] private GameObject _chatMessagePrefab;
    [SerializeField] private Transform _chatContent;
    [SerializeField] private ScrollRect _chatScrollRect;
    [SerializeField] private ChatUIFader _chatUIFader;

    public static event Action<bool> OnChatInputActiveChanged; // New event
    private bool _isChatInputActive = false;

    private void Start()
    {
        //_chatInputField.onSubmit.AddListener((text) =>
        //{
        //    SendChat();
        //});
        _chatButton.onClick.AddListener(SendChat);
        _chatInputField.onSelect.AddListener(OnChatInputSelected);
        _chatInputField.onDeselect.AddListener(OnChatInputDeselected);
    }
    private void Update()
    {
        // 채팅 입력 필드가 활성화되어 있지 않고, Enter 키가 눌렸을 때
        if (!_isChatInputActive && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            ActivateChatInput();
        }
    }

    private void ActivateChatInput()
    {
        _chatInputField.ActivateInputField();
        _chatInputField.Select();
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
        _chatButton.onClick.RemoveListener(SendChat);
        _chatInputField.onSelect.RemoveListener(OnChatInputSelected);
        _chatInputField.onDeselect.RemoveListener(OnChatInputDeselected);
    }

    public void SendChat()
    {
        StartCoroutine(DelayedSendChat());
    }
    
    private IEnumerator DelayedSendChat()
    {
        yield return null;
        yield return null;


        _chatInputField.DeactivateInputField();
        string text = _chatInputField.text;
        Debug.Log(text);
        if (string.IsNullOrEmpty(text))
        {
            _chatInputField.text = "";
            _chatInputField.ActivateInputField();
            yield break;
        }

        photonView.RPC(nameof(ReceiveChatMessage), RpcTarget.All, PhotonNetwork.NickName, text);
        _chatInputField.text = "";
        _chatInputField.ActivateInputField();
    }

    [PunRPC]
    private void ReceiveChatMessage(string sender, string message)
    {
        var msgGO = Instantiate(_chatMessagePrefab, _chatContent);
        var chatMsg = msgGO.GetComponent<ChattingMessage>();
        chatMsg.Text = $"<b>{sender}</b>: {message}";

        _chatUIFader?.FadeIn();
        ScrollToBottom();
    }

    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        _chatScrollRect.verticalNormalizedPosition = 0f;
    }

    private void OnChatInputSelected(string arg0)
    {
        _isChatInputActive = true;
        OnChatInputActiveChanged?.Invoke(true);
    }

    private void OnChatInputDeselected(string arg0)
    {
        _isChatInputActive = false;
        OnChatInputActiveChanged?.Invoke(false);
    }

    public bool IsChatInputActive => _isChatInputActive;
}
