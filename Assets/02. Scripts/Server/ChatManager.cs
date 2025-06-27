using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class ChatManager : MonoBehaviourPun
{
    [SerializeField] private TMP_InputField _chatInputField;
    [SerializeField] private Button _chatButton;
    [SerializeField] private GameObject _chatMessagePrefab;
    [SerializeField] private Transform _chatContent;
    [SerializeField] private ScrollRect _chatScrollRect;

    private void Start()
    {
        //_chatInputField.onSubmit.AddListener((text) =>
        //{
        //    SendChat();
        //});
        _chatButton.onClick.AddListener(SendChat);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
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
        ScrollToBottom();
    }

    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        _chatScrollRect.verticalNormalizedPosition = 0f;
    }
}
