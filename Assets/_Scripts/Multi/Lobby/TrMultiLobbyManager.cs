using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

public class TrMultiLobbyManager : MonoBehaviourPunCallbacks
{
    static TrMultiLobbyManager _instance;
    public static TrMultiLobbyManager xInstance { get { return _instance; } }

    [SerializeField] byte _numMaxUser;
    int[] _playerIds;

    [SerializeField] TrMultiLobbyPlayerComponents[] _players;
    [SerializeField] CanvasGroup _imgFade;
    [SerializeField] float _speedFade;
    byte _indexHead;

    [SerializeField] Sprite _spBgNickMine;
    [SerializeField] Sprite _spBgNickNotMine;

    bool _isReady = false;
    bool[] _isPlayerReadyStates;

    public void xDebug(){
        Debug.Log(PhotonNetwork.IsMasterClient);
    }
    #region GetDatas
    byte yGetNumUsers()
    {
        byte result = 0;
        for(byte i=0; i<_numMaxUser; i++){
            if (_playerIds[i] != -1)
                result++;
        }
        return result;
    }
    #endregion
    #region SetDatas
    void ySetEmptyProfile(byte index)
    {
        TrMultiLobbyPlayerComponents player = _players[index];
        player._txtNickname.text = "";
        player._srCharacter.gameObject.SetActive(false);
        player._srBgNickname.gameObject.SetActive(false);
    }

    void ySetProfile(byte index, string name, byte imgType)
    {
        TrMultiLobbyPlayerComponents player = _players[index];
        player._txtNickname.text = name;
        player._srCharacter.gameObject.SetActive(true);

        player._srBgNickname.gameObject.SetActive(true);
        TrSO_MultiProfile profile = TrProfileDatas.xInstance.zGetProfileData(imgType);
        player._srCharacter.sprite = profile._spCharacter;
        player._srProfile.sprite = profile._spProfile;

        player._animLeaf.SetTrigger("Shake");

        if (index == TrNetworkManager._myIndex){
            player._srBgNickname.sprite = _spBgNickMine;
        }
    }

    void ySetHead(){
        for (byte i = 0; i < _numMaxUser; i++){
            if (i == _indexHead) {
                _players[i]._goImgHead.SetActive(true);
                _players[i]._goIsReady.gameObject.SetActive(false);
            }
            else{
                _players[i]._goImgHead.SetActive(false);
            }
        }

        if(TrNetworkManager._myIndex == _indexHead){
            for (byte i = 0; i < _numMaxUser; i++){
                if (i != TrNetworkManager._myIndex && _playerIds[i] != -1)
                    _players[i]._goBtnKick.SetActive(true);
                else
                    _players[i]._goBtnKick.SetActive(false);
            }
        }
        else{
            for (byte i = 0; i < _numMaxUser; i++){
                _players[i]._goBtnKick.SetActive(false);
            }
        }
    }

    public void zChangeProfile(byte index){
        TrNetworkManager._imgType = index;
        PlayerPrefs.SetInt(TrProjectSettings.PROFILETYPE, index);
        PlayerPrefs.Save();
        photonView.RPC("yRPCChangeProfile", RpcTarget.AllBuffered, 
            TrNetworkManager._myIndex, TrNetworkManager._imgType);
    }

    [PunRPC]
    void yRPCChangeProfile(byte indexPlayer, byte indexImg){
        TrSO_MultiProfile profile = TrProfileDatas.xInstance.zGetProfileData(indexImg);
        _players[indexPlayer]._srCharacter.sprite = profile._spCharacter;
        _players[indexPlayer]._srProfile.sprite = profile._spProfile;
        _players[indexPlayer]._animLeaf.SetTrigger("Shake");
    }

    #endregion
    #region join
    public void yJoinedRoom(){
        int id = PhotonNetwork.LocalPlayer.ActorNumber;

        if(PhotonNetwork.PlayerList.Length > 1)
            photonView.RPC("yRPCNoticeEnterToMaster", RpcTarget.MasterClient, id);
        else{
            _indexHead = 0;
            TrNetworkManager._myIndex = 0;
            // at first, enter room(first MasterClient)
            _playerIds[0] = id;
            ySetProfile(0, DatabaseManager._myDatas.nickName, TrNetworkManager._imgType);
            ySetHead();
            _imgFade.DOFade(0, _speedFade);
        }
    }

    [PunRPC]
    void yRPCSetUserInfo(byte index, string name, byte imgType){
        ySetProfile(index, name, imgType);
        _imgFade.DOFade(0, _speedFade);
        _players[index]._goIsReady.gameObject.SetActive(_isPlayerReadyStates[index]);
        ySetHead();
    }

    [PunRPC]
    void yRPCThrowPlayerDatas(int[] playerIds, bool[] readyStates, byte head) {
        if (!PhotonNetwork.IsMasterClient){
            _playerIds = playerIds;
            _isPlayerReadyStates = readyStates;
            _indexHead = head;
        }

        for (byte i=0; i<_numMaxUser; i++){
            if(_playerIds[i] == PhotonNetwork.LocalPlayer.ActorNumber){
                TrNetworkManager._myIndex = i;
                string name = DatabaseManager._myDatas.nickName;
                photonView.RPC("yRPCSetUserInfo", RpcTarget.All, i, name, TrNetworkManager._imgType);
                break;
            }
        }
    }

    [PunRPC]
    void yRPCNoticeEnterToMaster(int id)
    {
        bool isFill = false;
        for (byte i=0; i<_numMaxUser; i++){
            if(!isFill && _playerIds[i] == TrNetworkManager._initId){
                isFill = true;
                _playerIds[i] = id;
            }
        }
        photonView.RPC("yRPCThrowPlayerDatas", RpcTarget.All,
            _playerIds, _isPlayerReadyStates, _indexHead);
    }
    #endregion
    #region left 
    public void zDisconnectPlayer(int playerId){
        for(byte i=0; i<_numMaxUser; i++){
            if(playerId == _playerIds[i])
            {
                yRPCNoticeLefRoom(i);
                break;
            }
        }

    }

    public void zLeftRoom(){
        StartCoroutine(yCoLeaveRoom());
    }

    IEnumerator yCoLeaveRoom(){
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length > 1)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.MasterClient.GetNext());
        }

        _imgFade.alpha = 1;

        //photonView.RPC("yRPCSetReady", RpcTarget.All, TrNetworkManager._myIndex, false);
        if(PhotonNetwork.CurrentRoom.PlayerCount > 1)
            photonView.RPC("yRPCNoticeLefRoom", RpcTarget.All, TrNetworkManager._myIndex);
        PhotonNetwork.LeaveRoom();
        yield return new WaitUntil(() => !PhotonNetwork.InRoom);
        SceneManager.LoadScene(TrProjectSettings.strLOBBY);
    }

    [PunRPC]
    void yRPCNoticeLefRoom(byte leftIndex){
        ySetEmptyProfile(leftIndex);
        _playerIds[leftIndex] = TrNetworkManager._initId;
        yRPCSetReady(leftIndex, false);
        if(leftIndex == _indexHead){
            for(byte i=0; i<_numMaxUser; i++){
                if(_playerIds[i] != -1){
                    _indexHead = i;
                    ySetHead();
                    break;
                }
            }
        }
        _players[leftIndex]._goBtnKick.SetActive(false);
        
    }
    #endregion
    #region ready
    public void xOnClickReady(){
        if(TrNetworkManager._myIndex != _indexHead){
            _isReady = !_isReady;
            photonView.RPC("yRPCSetReady", RpcTarget.All, TrNetworkManager._myIndex, _isReady);
        }
        else{
            byte numUser = yGetNumUsers();
/*            if(numUser < 2){
                Debug.Log("can't play by alone");
                return;
            }*/

            bool isAllReady = true;
            for(byte i=0; i<_numMaxUser; i++){
                if (TrNetworkManager._myIndex != i && _playerIds[i] != -1) {
                    if (!_isPlayerReadyStates[i]){
                        isAllReady = false;
                        break;
                    }
                }
            }
            if (isAllReady){
                _players[TrNetworkManager._myIndex]._goIsReady.gameObject.SetActive(true);
                photonView.RPC("yRPCStartGame", RpcTarget.All, numUser);
            }
            else{
                Debug.Log("Not all ready");
            }
        }
    }

    [PunRPC]
    void yRPCSetReady(byte index, bool isReady){
        _players[index]._goIsReady.gameObject.SetActive(isReady);
        _isPlayerReadyStates[index] = isReady;
    }

    [PunRPC]
    void yRPCStartGame(byte numUser){
        GameManager._canBtnClick = false;
        for(int i=0; i<TrNetworkManager._numMaxUser; i++){
            if (_playerIds[i] != -1)
                _players[i]._goIsReady.SetActive(true);
        }
        TrNetworkManager._numCurrUser = numUser;
        StartCoroutine(yCoStartGame());
    }

    IEnumerator yCoStartGame(){
        if (PhotonNetwork.IsMasterClient){
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        TrNetworkManager._playerIds = _playerIds;
        GameManager._type = TT.enumGameType.Multi;
        _imgFade.DOFade(1, _speedFade);
        yield return TT.WaitForSeconds(_speedFade);

        if (PhotonNetwork.IsMasterClient && photonView.IsMine){
            PhotonNetwork.LoadLevel(TrProjectSettings.strPUZZLEPEAR);
        }
    }
    #endregion
    #region kick
    public void xKickPlayer(int index)
    {
        photonView.RPC("yRPCKickPlayer", RpcTarget.All, (byte)index);
    }

    [PunRPC]
    void yRPCKickPlayer(byte kickIndex)
    {
        yRPCNoticeLefRoom(kickIndex);
        if (kickIndex == TrNetworkManager._myIndex)
        {
            StartCoroutine(yCoKicked());
        }
    }

    IEnumerator yCoKicked()
    {
        _imgFade.alpha = 1;
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length > 1)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.MasterClient.GetNext());
        }
        PhotonNetwork.LeaveRoom();

        yield return new WaitUntil(() => !PhotonNetwork.InRoom);
        SceneManager.LoadScene(TrProjectSettings.strLOBBY);
    }
    #endregion
    #region init
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Offline : Connection Disabled {cause.ToString()}");
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        //_txtAnnounceInfo.text = "Online : Connected to Master Server";
        Debug.Log("Online : Connected to Master Sever");
        PhotonNetwork.AutomaticallySyncScene = true;
        zRandomMatching();
    }

    public void zRandomMatching()
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.JoinRandomRoom();
        else
        {
            Debug.Log("Failed Random room");
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("There is no empty room, Creating new Room.");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = _numMaxUser;
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Random Room");
        yJoinedRoom();
    }

    IEnumerator yConnectNetwork()
    {
        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);
        PhotonNetwork.GameVersion = TrNetworkManager._gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }
    void yInit()
    {
        TrChangeCharacter.xInstance.zInit(TrNetworkManager._imgType);
        GameManager._canBtnClick = true;
        for (byte i = 0; i < _numMaxUser; i++)
        {
            ySetEmptyProfile(i);
            _players[i]._goIsReady.gameObject.SetActive(false);
            _players[i]._srBgNickname.sprite = _spBgNickNotMine;
            _isPlayerReadyStates[i] = false;
            _playerIds[i] = TrNetworkManager._initId;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void yResetDomainCodes(){
        _instance = null;
    }
    void Awake(){
        if (_instance == null){
            _instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    void Start()
    {
        TrAudio_Music.xInstance.zzPlayMain(0.25f);
        GameManager.xInstance.zSetCamera();
        _imgFade.alpha = 1;
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        TrNetworkManager._liPlayersData = new List<TrPlayersData>();
        StartCoroutine(yConnectNetwork());

        TrNetworkManager._imgType = (byte)PlayerPrefs.GetInt(TrProjectSettings.PROFILETYPE, (int)TrMultiProfileType.moomoo);
        TrNetworkManager._numMaxUser = _numMaxUser;
        if (TrNetworkManager._playerIds == null)
            _playerIds = new int[_numMaxUser];
        else
            _playerIds = TrNetworkManager._playerIds;
        _isPlayerReadyStates = new bool[_numMaxUser];
        yInit();
    }
#endregion
    void Update(){
        if (PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState == ExitGames.Client.Photon.PeerStateValue.Disconnected){
            if (!PhotonNetwork.ReconnectAndRejoin())
            {
                Debug.Log("Failed reconnecting and joining!!", this);
            }
            else
            {
                Debug.Log("Successful reconnected and joined!", this);
            }
        }
    }
}
