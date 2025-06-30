using System;
using Photon.Pun;
using Photon.Realtime;
using RainbowArt.CleanFlatUI;
using UnityEngine;

public class KickManager : MonoBehaviourPun
{
    public static KickManager Instance;
    
    private void Awake() => Instance = this;
    [SerializeField] private Notification _kickNoti;

    [PunRPC]
    public void ReceiveKickMessage()
    {
        _kickNoti.ShowNotification();
    }

    public void KickPlayer(Player targetPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC(nameof(ReceiveKickMessage), targetPlayer);
        PhotonNetwork.CloseConnection(targetPlayer);
    }
}
