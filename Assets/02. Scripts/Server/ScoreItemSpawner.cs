using Photon.Pun;
using UnityEngine;

public class ScoreItemSpanwer : MonoBehaviour
{
    public float Interval;            // 몇초마다 생성할 것이냐
    private float _intervalTimer = 0;
    public float Range;               // 랜덤한 범위

    private void Start()
    {
        Interval = Random.Range(5f, 20f);
        Range = Random.Range(20f, 25f);
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        _intervalTimer += Time.deltaTime;

        if (_intervalTimer >= Interval)
        {
            _intervalTimer = 0;

            Vector3 randomPosition = transform.position + Random.insideUnitSphere * Range;
            randomPosition.y = 2f;

            ItemObjectFactory.Instance.RequestCreate(GetRandomDropItem(), randomPosition, 1);
            Interval = Random.Range(5f, 20f);
            Range = Random.Range(20f, 25f);
        }
    }

    private EItemType GetRandomDropItem()
    {
        float rand = Random.Range(0f, 1f);

        if (rand <= 0.2f)
            return EItemType.HealItem;
        else if (rand <= 0.4f)
            return EItemType.RecoverStaminaItem;
        else if (rand <= 0.6f)
            return EItemType.PowerUpItem;
        else if (rand <= 0.8f)
            return EItemType.SpeedUpItem;
        else
            return EItemType.ScoreItem;
    }
}
