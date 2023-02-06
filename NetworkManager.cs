using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using PN = Photon.Pun.PhotonNetwork;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Inst { get; private set; }

    string tempName;

    void Awake()
    {
        Inst = this;

        //기본값 60, 30 에서 절반으로 낮춤
        PN.SendRate = 30;
        PN.SerializationRate = 15; //모바일 대응

        PN.ConnectUsingSettings();
    }

    #region 포톤 콜백
    //서버 연결 시 로비까지 바로 연결
    public override void OnConnectedToMaster()
    {
        PN.JoinLobby();
    }
    //로비 입장 시 타이틀 씬 로드
    public override void OnJoinedLobby()
    {
        LoadingSceneManager.Inst.LoadTitle();
    }
    //방 생성 함수
    //타이틀 씬에서 입력한 아이디를 저장해두었다가 GetName() 함수로 반환
    public void MakeRoom(string name)
    {
        tempName = name;
        LoadingSceneManager.Inst.SetLoadingScreen(true);
        PN.JoinOrCreateRoom("room", new RoomOptions() { MaxPlayers = 4 }, null);
    }
    //방 입장 시 게임 스타트(플레이어 씬, 스테이지 1 씬 로드)
    public override void OnJoinedRoom()
    {
        LoadingSceneManager.Inst.GameStart();
    }
    //접속이 끊길 경우 재접속 시도
    public override void OnDisconnected(DisconnectCause cause)
    {
        PN.Reconnect();
    }

    //플레이어 입장, 퇴장 시
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("플레이어 입장");
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("플레이어 퇴장");
    }
    #endregion

    public GameObject StartCharacter()
    {
        return PN.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }

    public string GetName()
    {
        tempName = tempName.Trim();
        if (tempName == "")
            tempName = "player" + Random.Range(1000, 10000);
        if (tempName.Length > 10)
            tempName = tempName.Substring(0, 10);
        return tempName;
    }

    public GameObject Instantiate(string name)
    {
        return PN.Instantiate(name, Vector3.zero, Quaternion.identity);
    }

    public bool IsMaster()
    {
        return PN.IsMasterClient;
    }
}