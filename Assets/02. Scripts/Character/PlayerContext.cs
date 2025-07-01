using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerContext : MonoBehaviour
{
    public PlayerStat PlayerStat;
    private Dictionary<Type, PlayerAbility> _abilitiesCache = new();
    public CharacterController CharacterController;

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
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