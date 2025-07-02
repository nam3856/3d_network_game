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

    private void Awake()
    {
        _pickupEffect = $"PickupEffect_{_type}";
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
                scoreAmount += Random.Range(100, 201);

                player.View.RPC(nameof(PlayerContext.RPC_AddScore), player.View.Owner, scoreAmount);

                PhotonNetwork.Instantiate(_pickupEffect, transform.position, Quaternion.identity);
                ItemObjectFactory.Instance.RequestDelete(photonView.ViewID);
            }
        }
    }

}
