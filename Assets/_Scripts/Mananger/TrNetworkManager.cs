using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class TrPlayersData
{
    public TrPlayersData(byte index, string nickname, byte imgType, short score)
    {
        _index = index;
        _nickname = nickname;
        _imgType = imgType;
        _score = score;
    }

    public byte _index;
    public string _nickname;
    public short _score;
    public byte _imgType;
}

public class TrNetworkManager : MonoBehaviourPunCallbacks
{
    static TrNetworkManager _instance;
    public static TrNetworkManager xInstance { get { return _instance; } }

    public static readonly string _gameVersion = "1";
    public static byte _numMaxUser;
    public static byte _numCurrUser;

    public static int[] _playerIds;
    public static int _initId = -1;

    public static ushort[] _scoers;

    public static byte _imgType;
    public static byte _myIndex;

    public static short[] _playerScores;

    public static List<TrPlayersData> _liPlayersData;

    private void OnApplicationPause(bool pause){
        if (pause){
            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1){
                PhotonNetwork.SetMasterClient(PhotonNetwork.MasterClient.GetNext());
            }
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (GameManager._state == TT.enumGameState.Lobby)
            TrMultiLobbyManager.xInstance.zDisconnectPlayer(otherPlayer.ActorNumber);
        else if (GameManager._state == TT.enumGameState.Play)
            _numCurrUser--;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void yResetDomainCodes()
    {
        _instance = null;
    }
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
