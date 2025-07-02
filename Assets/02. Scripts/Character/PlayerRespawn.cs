using UnityEngine;
using Photon.Pun;

public class PlayerRespawn : PlayerAbility
{
    private float _respawnTimer;
    private bool _canSpawn;

    protected override void OnEnable()
    {
        _respawnTimer = 0;
        _canSpawn = true;
    }

    protected override void Update()
    {
        base.Update();
        if (_photonView.IsMine)
        {
            _respawnTimer += Time.deltaTime;
            if (_respawnTimer > _owner.PlayerStat.RespawnTime)
            {
                if (_canSpawn)
                {
                    _canSpawn = false;
                    Respawn();
                }
            }
        }
    }

    private void Respawn()
    {
        _photonView.RPC(nameof(RPC_Respawn), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_Respawn()
    {
        gameObject.SetActive(false);
        gameObject.transform.position = new Vector3(Random.Range(-38f, 28f), 10f, Random.Range(-38f, 38f));
        gameObject.SetActive(true);

        _owner.GetAbility<PlayerAttack>().enabled = true;
        _owner.GetAbility<PlayerMovement>().enabled = true;
        _owner.GetAbility<PlayerStamina>().enabled = true;
        _owner.GetAbility<AnimationPlayer>().enabled = true;
        _owner.CharacterController.enabled = true;

        var col = GetComponentsInChildren<Collider>();
        foreach (Collider c in col)
        {
            c.enabled = true;
        }
        enabled = false;
    }
}
