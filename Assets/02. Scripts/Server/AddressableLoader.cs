using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;


public class AddressableLoader : MonoBehaviourPunCallbacks
{
    public static AddressableLoader Instance { get; private set; }
    private string prefabAddress = "PlayerPrefab";

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

        pool.Preload("PlayerPrefab");
    }
    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1 && PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
            StartCoroutine(LoadAndSpawnPlayer());
        }
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            StartCoroutine(LoadAndSpawnPlayer());
        }
    }
    private IEnumerator LoadAndSpawnPlayer()
    {
        // pool�� �ε��ϱ� ���� Photon.Instantiate�� ȣ���ϸ� �� �� �� ��ٷ��� ��
        while (!(PhotonNetwork.PrefabPool is AddressablesPool pool) || !pool.IsLoaded(prefabAddress))
        {
            yield return null;
        }

        Debug.Log("Addressables: Player prefab preloaded, now instantiating.");

        PhotonNetwork.Instantiate(prefabAddress, GetSpawnPos(), Quaternion.identity);
    }

    private Vector3 GetSpawnPos()
    {
        return new Vector3(Random.Range(-3f, 3f), 0.1f, Random.Range(-30f, -23f));
    }
}
