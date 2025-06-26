using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    private string prefabAddress = "PlayerPrefab";
    private void Awake()
    {
        var pool = new AddressablesPool();
        PhotonNetwork.PrefabPool = pool;

        pool.Preload("PlayerPrefab");
    }
    private void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
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
