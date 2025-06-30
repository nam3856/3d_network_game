using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;
// 스크립트
public class PlayerlistItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string _nickname;
    [SerializeField] private Player _currentPlayer;
    [SerializeField] private TextMeshProUGUI _nicknameText;
    [SerializeField] private Button _kickButton;
    private void Awake()
    {
        _kickButton.onClick.AddListener(KickPlayer);
    }

    private void OnDestroy()
    {
        _kickButton?.onClick.RemoveListener(KickPlayer);
    }

    private void KickPlayer()
    {
        KickManager.Instance.KickPlayer(_currentPlayer);
    }

    public void SetNickname(Player player)
    {
        _nickname = player.NickName;
        _nicknameText.text = _nickname;
        _currentPlayer = player;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (_currentPlayer == PhotonNetwork.MasterClient) return;
            _kickButton.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (_currentPlayer == PhotonNetwork.MasterClient) return;
            _kickButton.gameObject.SetActive(false);
        }
    }

}
