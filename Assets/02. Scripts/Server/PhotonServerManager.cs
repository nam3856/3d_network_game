using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System;
public class PhotonServerManager : MonoBehaviourPunCallbacks
{
    public static event Action<string> ServerEvent;
    private readonly string _version = "0.0.1";
    // Major, Minor, Patch
    //<전체를 뒤엎을 변화>.<새로운 기능 추가>.<버그 수정>

    TypedLobby _lobbyA = new TypedLobby("A", LobbyType.Default);
    TypedLobby _lobbyB = new TypedLobby("B", LobbyType.Default);


    private string _nickName = "Player";
    private void Start()
    {
        PhotonNetwork.GameVersion = _version;
        PhotonNetwork.NickName = _nickName;

        // 방장이 로드한 씬 게임에 참여한 다른 사용자들이 똑같이 로드할 수 있도록 동기화 해주는 옵션
        // 방장(마스터 클라이언트): 방을 만든 소유자. (방에는 하나의 마스터 클라이언트만 존재)
        PhotonNetwork.AutomaticallySyncScene = true;


        // 설정값들을 이용해 서버 접속 시도
        // 정확히는 네임 서버로 접속 시도
        PhotonNetwork.ConnectUsingSettings();
    }

    // 네임 서버에 접속한 후 호출되는 콜백 함수
    public override void OnConnected()
    {
        base.OnConnected();
        ServerEvent?.Invoke($@"{PhotonNetwork.CloudRegion} 지역에 접속했습니다.
마스터 서버에 접속하는 중...");
    }

    // 마스터 서버에 접속한 후 호출되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        ServerEvent?.Invoke($@"{PhotonNetwork.NickName} 님 어서오세요!
로비에 접속 중입니다...");
        Debug.Log("마스터 서버에 접속했습니다.");
        Debug.Log("현재 플레이어 닉네임: " + PhotonNetwork.NickName);
        // 방 만들기
        PhotonNetwork.JoinLobby();
    }

    // 로비에 접속한 후 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        Debug.Log("로비 접속 완료!");
        ServerEvent?.Invoke($@"{PhotonNetwork.NickName} 님 어서오세요!
방 입장을 시도중입니다...");
        Debug.Log(PhotonNetwork.InLobby);

        PhotonNetwork.JoinRandomRoom();
    }



    // 랜덤 룸 입장에 실패했을 경우 호출되는 콜백 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");

        if (returnCode == 32760) // 룸이 없을 경우
        {
            ServerEvent?.Invoke($@"{PhotonNetwork.NickName} 님 어서오세요!
방 생성을 시도중입니다...");
            CreateRoom();
        }
        else if(returnCode == 32758) // 최대 접속자 수 초과
        {
            ServerEvent?.Invoke("현재 접속자가 너무 많습니다. 잠시 후 다시 시도해주세요.");
        }
        else
        {
            ServerEvent?.Invoke($"룸 입장 실패: {message}");
        }
    }

    private static void CreateRoom()
    {
        // 룸 속성 정의
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;    // 룸에 입장할 수 있는 최대 접속자 수
        roomOptions.IsOpen = true;  // 룸의 오픈 여부
        roomOptions.IsVisible = true;  // 로비에서 룸 목록에 노출시킬지 여부

        // 룸 생성
        PhotonNetwork.CreateRoom("test", roomOptions);
        // 룸 입장 또는 생성
        // PhotonNetwork.JoinOrCreateRoom("test", roomOptions, TypedLobby.Default);
    }

    // 룸 생성에 실패했을 경우 호출되는 콜백 함수
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ServerEvent?.Invoke($"방 생성 실패: {message}");
        Debug.Log($"CreatRoom Failed {returnCode}:{message}");
    }

    // 룸 생성이 완료된 후 호출되는 콜백 함수
    public override void OnCreatedRoom()
    {
        ServerEvent?.Invoke($@"{PhotonNetwork.NickName} 님 어서오세요!
방 생성을 완료했습니다.");
        Debug.Log("Created Room");
        // 생성된 룸 이름 확인
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }


    // 룸에 입장한 후 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        ServerEvent?.Invoke($@"{PhotonNetwork.NickName} 님 어서오세요!
'{PhotonNetwork.CurrentRoom.Name}' 방에 입장했습니다.");
        Debug.Log("룸 입장 완료!");
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

        // 룸에 접속한 사용자 정보
        Dictionary<int, Player> roomPlayers = PhotonNetwork.CurrentRoom.Players;
        foreach (KeyValuePair<int, Player> player in roomPlayers)
        {
            Debug.Log($"{player.Value.NickName} : {player.Value.ActorNumber}");
        }
    }
}
