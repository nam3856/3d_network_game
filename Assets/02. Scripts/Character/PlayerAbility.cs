using Photon.Pun;
using UnityEngine;

public abstract class PlayerAbility:MonoBehaviour
{
    protected PlayerContext _owner { get; private set; }
    protected PhotonView _photonView { get; private set; }
    

    protected virtual void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _owner = GetComponent<PlayerContext>();
        if (_owner == null)
        {
            Debug.LogError("PlayerAbility must be attached to a GameObject with PlayerContext component.");
        }
        if (_photonView == null)
        {
            Debug.LogError("PlayerAbility must be attached to a GameObject with PhotonView component.");
        }

    }

    protected virtual void OnEnable()
    {
        if (!_photonView.IsMine) return;
    }

    protected virtual void OnDisable()
    {
        if (!_photonView.IsMine) return;
    }

    protected virtual void Update()
    {
        if (!_photonView.IsMine) return;
    }
}