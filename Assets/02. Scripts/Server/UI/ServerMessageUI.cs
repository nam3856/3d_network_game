using TMPro;
using UnityEngine;

public class ServerMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _messageText;
    private void Awake()
    {
        PhotonServerManager.ServerEvent += OnServerEvent;
    }
    private void OnDestroy()
    {
        PhotonServerManager.ServerEvent -= OnServerEvent;
    }

    private void OnServerEvent(string message)
    {
        _messageText.text = message;
    }

}
