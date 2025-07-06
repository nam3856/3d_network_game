using Photon.Pun;
using UnityEngine;
public enum EItemType
{
    ScoreItem,
    PowerUpItem,
    HealItem,
    RecoverStaminaItem,
    SpeedUpItem,

}
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class ItemObject : MonoBehaviourPun
{
    [SerializeField]private EItemType _type;
    private string _pickupEffect;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _pickupEffect = $"PickupEffect_{_type}";
    }
    private void Start()
    {
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터만 초기 속도 부여
            Vector3 launchDir = Vector3.up * 4f;
            Launch(launchDir);
        }
    }

    private void Update()
    {
        if(transform.position.y < -5f)
        {
            Vector3 randomPosition = transform.position + Random.insideUnitSphere * 25f;
            randomPosition.y = 5f;
            transform.position = randomPosition;
        }
    }
    public void Launch(Vector3 baseForce)
    {
        if (_rb != null)
        {
            Vector3 randomDir = Random.insideUnitSphere;
            randomDir.y = Mathf.Abs(randomDir.y);
            Vector3 force = baseForce + randomDir * Random.Range(3f, 7f);
            _rb.AddForce(force, ForceMode.Impulse);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        int scoreAmount = 0;
        if (other.CompareTag("Player"))
        {
            PlayerContext player = other.GetComponent<PlayerContext>();
            
            switch (_type)
            {
                case EItemType.ScoreItem:
                    if (PhotonNetwork.IsMasterClient)
                    {
                        scoreAmount += 100;
                    }
                    break;
                case EItemType.PowerUpItem:
                    player.GetAbility<PlayerAttack>().SetBuffTimer();
                    break;
                case EItemType.HealItem:
                    player.GetAbility<PlayerHealth>().Heal(20);
                    break;
                case EItemType.RecoverStaminaItem:
                    player.GetAbility<PlayerStamina>().Recover(50);
                    break;
                case EItemType.SpeedUpItem:
                    player.GetAbility<PlayerMovement>().SetBuffTime();
                    break;
            }

            if (PhotonNetwork.IsMasterClient)
            {

                if (scoreAmount > 0)
                {
                    ScoreManager.Instance.photonView.RPC(nameof(ScoreManager.Instance.RPC_AddScore), player.View.Owner, scoreAmount);
                }
                PhotonNetwork.Instantiate(_pickupEffect, transform.position, Quaternion.identity);
                ItemObjectFactory.Instance.RequestDelete(photonView.ViewID);
            }
        }
    }

}
