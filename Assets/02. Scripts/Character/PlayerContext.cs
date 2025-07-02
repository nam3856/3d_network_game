using UnityEngine;
using System;
using System.Collections.Generic;
using Photon.Pun;

public class PlayerContext : MonoBehaviourPun
{
    public PlayerStat PlayerStat;
    private Dictionary<Type, PlayerAbility> _abilitiesCache = new();
    public CharacterController CharacterController;
    public PhotonView View => photonView;
    public int Score { get; set; }
    public static event Action<int> OnScoreAdded;

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
    }

    [PunRPC]
    public void RPC_AddScore(int amount)
    {
        Score += amount;
        if (photonView.IsMine)
        {
            OnScoreAdded?.Invoke(amount);
        }
    }

    public T GetAbility<T>() where T : PlayerAbility
    {
        var type = typeof(T);

        if(_abilitiesCache.TryGetValue(type, out PlayerAbility ability))
        {
            return ability as T;
        }

        ability = GetComponent<T>();

        if(ability != null)
        {
            _abilitiesCache[ability.GetType()] = ability;
            return ability as T;
        }

        throw new Exception($"어빌리티 {type.Name}를 찾을 수 없습니다. PlayerContext에 추가되어 있는지 확인하세요.");
    }
}