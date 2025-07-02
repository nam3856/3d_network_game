using Photon.Pun;
using UnityEngine;
[RequireComponent(typeof(PhotonView))]
public class ParticleEffect : MonoBehaviourPun
{
    public float Duration = 2f;
    private float _timer = 0;

    private void OnEnable()
    {
        _timer = 0;
    }
    private void Update()
    {
        if (!photonView.IsMine) return;
        _timer += Time.deltaTime;
        if (_timer > Duration)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
