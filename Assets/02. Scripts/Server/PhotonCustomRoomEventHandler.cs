using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System;

// 스크립트
public class PhotonCustomRoomEventHandler : MonoBehaviourPunCallbacks
{

    public static event Action<string> ServerEvent;
    public GameObject ChatPrefab;
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if (returnCode == ErrorCode.GameFull) // 최대 접속자 수 초과
        {
            ServerEvent?.Invoke("현재 접속자가 너무 많습니다. 잠시 후 다시 시도해주세요.");
        }
        else if(returnCode == 32760)
        {
            ServerEvent?.Invoke("생성되어있는 방이 없습니다.");
        }
        else
        {
            ServerEvent?.Invoke($"룸 입장 실패: {message}");
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        switch (returnCode)
        {
            case ErrorCode.GameFull:
                ServerEvent?.Invoke("참가하려는 방이 이미 가득 찼습니다.");
                break;

            case ErrorCode.GameClosed:
                ServerEvent?.Invoke("해당 방은 이미 닫혔거나 게임이 시작되었습니다.");
                break;

            case ErrorCode.GameDoesNotExist:
                ServerEvent?.Invoke("해당 방은 더 이상 존재하지 않습니다.");
                break;

            default:
                ServerEvent?.Invoke($"방 참가 실패: {message}");
                break;
        }

    }

    public override void OnJoinedRoom()
    {
        ChatPrefab?.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        ChatPrefab?.SetActive(false);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        ServerEvent?.Invoke($"서버와 연결이 끊겼습니다: {cause}");
    }
}
