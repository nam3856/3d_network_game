using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;


[RequireComponent(typeof(PhotonView))]
public class ScoreManager : MonoBehaviourPunCallbacks
{
    public static ScoreManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    private Dictionary<string, (int, int)> _scores = new Dictionary<string, (int, int)>();
    public Dictionary<string, (int, int)> Scores => _scores;

    public static event Action OnDataChanged;

    private int _score = 0;
    private int _killCount = 0;
    public int Score => _score + _killCount * 5000;
    public int KillCount => _killCount;

    public static event Action<int> OnScoreAdded;


    private void Start()
    {
        Refresh();
    }
    private void Refresh()
    {
        Hashtable hashTable = new Hashtable();
        hashTable.Add($"Score", Score);
        hashTable.Add($"Kill", _killCount);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashTable);
    }

    [PunRPC]
    public void RPC_AddKill(int amount)
    {
        if (photonView.IsMine)
        {
            _killCount += amount;
        }

        Refresh();
    }
    [PunRPC]
    public void RPC_AddScore(int amount)
    {
        if (amount == 0) return; 
        _score += amount;
        if (photonView.IsMine)
        {
            OnScoreAdded?.Invoke(amount);
        }

        Refresh();
    }

    [PunRPC]
    public void RPC_ResetScore(int player)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == player)
        {
            _score = 0;
        }

        Refresh();
    }
    // 플레이어의 커스텀 프로퍼티가 변경되면 호출되는 콜백 함수
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable hashtable)
    {
        Debug.Log($"Player {targetPlayer.NickName}의 점수: {hashtable["Score"]}");
        Debug.Log($"Player {targetPlayer.NickName}의 킬: {hashtable["Kill"]}");

        var roomPlayers = PhotonNetwork.PlayerList;
        foreach (Player player in roomPlayers)
        {
            if (player.CustomProperties.ContainsKey("Score"))
            {
                _scores[player.NickName] = ((int)player.CustomProperties["Score"], (int)player.CustomProperties["Kill"]);
            }
        }

        OnDataChanged?.Invoke();
    }
    public void StealHalfScore(int attackerActorNum, int victimActorNum, Vector3 victimPosition)
    {
        var players = PhotonNetwork.CurrentRoom.Players;

        if (!players.TryGetValue(victimActorNum, out var victimPlayer)) return;
        

        int victimScore = 0;
        int killCount = 0;
        if (victimPlayer.CustomProperties.TryGetValue("Score", out object scoreObj))
        {
            victimScore = (int)scoreObj;
        }
        if(victimPlayer.CustomProperties.TryGetValue("Kill", out object killObj))
        {
            killCount = (int)killObj;
        }

        victimScore -= killCount * 5000;
        int stolen = victimScore / 2;
        int dropped = victimScore - stolen;
        photonView.RPC(nameof(RPC_ResetScore), RpcTarget.All, victimActorNum);
        if (players.TryGetValue(attackerActorNum, out var attackerPlayer))
        {
            photonView.RPC(nameof(RPC_AddKillToActor), RpcTarget.All, attackerActorNum);
            if (stolen > 0)
            {
                photonView.RPC(nameof(RPC_AddScoreToActor), RpcTarget.All, attackerActorNum, stolen);
            }
        }
        
        
        if(dropped >= 100)
        {
            ItemObjectFactory.Instance.RequestCreate(EItemType.ScoreItem, victimPosition, Mathf.Max(1, dropped / 100));
        }
    }

    [PunRPC]
    private void RPC_AddKillToActor(int actorNum)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNum)
        {
            _killCount++;
            Refresh();
        }
    }

    [PunRPC]
    private void RPC_AddScoreToActor(int actorNum, int amount)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNum)
        {
            _score += amount;
            OnScoreAdded?.Invoke(amount);
            Refresh();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer.CustomProperties.ContainsKey("Score"))
        {
            _scores.Remove(otherPlayer.NickName);
            OnDataChanged?.Invoke();
        }
    }
}
