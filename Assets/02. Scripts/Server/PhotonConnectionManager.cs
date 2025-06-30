using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
public class PhotonConnectionManager : MonoBehaviourPunCallbacks
{
    public static event Action<string> OnServerMessage;
    private readonly string _version = "0.0.1";
    public static PhotonConnectionManager Instance;
    private string _nickName = "Player";
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PhotonNetwork.GameVersion = _version;
        PhotonNetwork.NickName = _nickName + UnityEngine.Random.Range(0, 1000);
        
        // 방장이 로드한 씬 게임에 참여한 다른 사용자들이 똑같이 로드할 수 있도록 동기화 해주는 옵션
        // 방장(마스터 클라이언트): 방을 만든 소유자. (방에는 하나의 마스터 클라이언트만 존재)
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 30;
        //if (PlayerPrefs.HasKey("USER_ID"))
        //    userId = PlayerPrefs.GetString("USER_ID");
        //else
        //{
        //    userId = Guid.NewGuid().ToString();
        //    PlayerPrefs.SetString("USER_ID", userId);
        //}
        string userId = $"USER_{UnityEngine.Random.Range(100000, 999999)}_{System.DateTime.UtcNow.Ticks}";

        PhotonNetwork.EnableCloseConnection = true;
        PhotonNetwork.AuthValues = new AuthenticationValues { UserId = userId };
        Debug.Log(PhotonNetwork.AuthValues);
        // 설정값들을 이용해 서버 접속 시도
        // 정확히는 네임 서버로 접속 시도
        PhotonNetwork.ConnectUsingSettings();
    }

    // 네임 서버에 접속한 후 호출되는 콜백 함수
    public override void OnConnected()
    {
        base.OnConnected();
        OnServerMessage?.Invoke($"마스터 서버에 접속하는 중...");
    }

    // 마스터 서버에 접속한 후 호출되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        OnServerMessage?.Invoke($"{PhotonNetwork.NickName} 님 어서오세요!");
        Debug.Log("마스터 서버에 접속했습니다.");
        Debug.Log("현재 플레이어 닉네임: " + PhotonNetwork.NickName);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }
}
