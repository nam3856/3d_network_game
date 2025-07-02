using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

public class AddressableLoader : MonoBehaviourPunCallbacks
{
    public static AddressableLoader Instance { get; private set; }
    private string[] prefabAddress = { "PlayerPrefab", "DeathEffect", "Bear" };
    private string HitEffectAddress = "HitEffect";
    private string _pickupEffectAddress = "PickupEffect";
    private string ItemAddress = "Item";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        var pool = new AddressablesPool();
        PhotonNetwork.PrefabPool = pool;

        foreach(var prefab in prefabAddress)
        {
            pool.Preload(prefab);
        }

        for(int i = 1; i <= 10; i++)
        {
            pool.Preload( $"{HitEffectAddress}_{i}");
        }

        foreach (var itemType in Enum.GetNames(typeof(EItemType)))
        {
            pool.Preload($"{_pickupEffectAddress}_{itemType}");
        }
        foreach (var itemType in Enum.GetNames(typeof(EItemType)))
        {
            pool.Preload( $"{ItemAddress}_{itemType}" );
        }


    }
    public override void OnEnable()
    {
        base.OnEnable();
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        //SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public override void OnJoinedRoom()
    {
        StartCoroutine(WaitUntilSceneLoadedThenSpawn());
    }

    private IEnumerator WaitUntilSceneLoadedThenSpawn()
    {
        while (SceneManager.GetActiveScene().buildIndex != 1)
            yield return null;

        yield return LoadAndSpawnPlayer();
    }
    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    StartCoroutine(LoadAndSpawnPlayer());
    //    if (scene.buildIndex == 1 && PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
    //    {
            
    //        Debug.Log("Start Spawn");
    //    }
    //    else
    //    {
    //        if (scene.buildIndex != 1) Debug.Log(scene.buildIndex);
    //        if (!PhotonNetwork.IsConnected) Debug.Log("PhotonNetwork not connected");
    //        if (!PhotonNetwork.InRoom) Debug.Log("Not in room");
    //    }
    //}
    private IEnumerator LoadAndSpawnPlayer()
    {
        // pool이 로딩하기 전에 Photon.Instantiate를 호출하면 안 됨 → 기다려야 함
        while (!(PhotonNetwork.PrefabPool is AddressablesPool pool) || !pool.IsLoaded(prefabAddress[0]))
        {
            Debug.Log("Not Loaded");
            yield return null;
        }

        Debug.Log("Addressables: Player prefab preloaded, now instantiating.");

        PhotonNetwork.Instantiate(prefabAddress[0], GetSpawnPos(), Quaternion.identity);
    }

    private Vector3 GetSpawnPos()
    {
        return new Vector3(Random.Range(-3f, 3f), 0.1f, Random.Range(-30f, -23f));
    }
}
