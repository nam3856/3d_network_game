using UnityEngine;
using Photon.Pun;

using Random = UnityEngine.Random;
public class BearSpawner : MonoBehaviourPun
{
    [SerializeField] private string bearPrefabName = "Bear";
    [SerializeField] private float spawnInterval = 15f;
    [SerializeField] private int maxBears = 2;
    [SerializeField] private Transform[] spawnPoints;
    private float _timer;
    private int _spawnedCount;

    private void OnEnable()
    {
        BearFSM.OnBearDied += HandleBearDied;
    }

    private void OnDisable()
    {
        BearFSM.OnBearDied -= HandleBearDied;
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        _timer += Time.deltaTime;

        if (_timer >= spawnInterval && _spawnedCount < maxBears)
        {
            SpawnBear();
            _timer = 0f;
        }
    }
    private void HandleBearDied()
    {
        _spawnedCount = Mathf.Max(0, _spawnedCount - 1);
    }

    private void SpawnBear()
    {
        Transform point = GetRandomSpawnPoint();
        if (point == null) return;

        GameObject bear = PhotonNetwork.InstantiateRoomObject(bearPrefabName, point.position, Quaternion.identity);
        _spawnedCount++;

    }

    private Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return null;

        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}
