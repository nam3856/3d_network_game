using Photon.Pun;
using UnityEngine;
// 스크립트

[RequireComponent(typeof (PhotonView))]
public class ItemObjectFactory : MonoBehaviourPun
{
    public static ItemObjectFactory Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RequestCreate(EItemType type, Vector3 dropPosition, int count)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            RPC_CreateItems(type, dropPosition, count);
        }
        else
        {
            photonView.RPC(nameof(RPC_CreateItems), RpcTarget.MasterClient, type, dropPosition, count);
        }
    }

    [PunRPC]
    private void RPC_CreateItems(EItemType type, Vector3 pos, int count)
    {
        for (int i = 0; i < count; i++)
        {
            PhotonNetwork.InstantiateRoomObject($"Item_{type.ToString()}", pos, Quaternion.identity);
        }
    }

    public void RequestDelete(int viewId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            RPC_Delete(viewId);
        }
        else
        {
            photonView.RPC(nameof(RPC_Delete), RpcTarget.MasterClient, viewId);
        }
    }

    [PunRPC]
    private void RPC_Delete(int viewId)
    {
        var objectToDelete = PhotonView.Find(viewId).gameObject;

        if (objectToDelete != null)
        {
            PhotonNetwork.Destroy(objectToDelete);
        }
    }
}
