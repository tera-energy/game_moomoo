using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;

public class TrMultiResultManager : MonoBehaviourPunCallbacks
{
    static TrMultiResultManager _instance;
    public static TrMultiResultManager xInstance { get { return _instance; } }

    [SerializeField] TrMultiResultPlayerComponents[] _players;

    [SerializeField] TrMultiResultPlayerUIComponents[] _uiPlayers;

    [SerializeField] CanvasGroup _imgFade;

    bool _isAction = false;

    [SerializeField] Sprite[] _spBgRankMine;
    [SerializeField] Sprite[] _spBgRankNotMine;

    [SerializeField] GameObject[] _goRankMedals;
    [SerializeField] TextMeshProUGUI[] _txtWinnerScores;

    [SerializeField] Sprite _spBgProfileMine;
    [SerializeField] Sprite _spBgProfileNotMine;

    public void xBtnExit()
    {
        if (_isAction) return;
        _isAction = true;

        StartCoroutine(yCoExit());
    }

    IEnumerator yCoExit(){
        _imgFade.DOFade(1, 0.5f);

        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();

        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);
        yield return TT.WaitForSeconds(0.5f);

        SceneManager.LoadScene(TrProjectSettings.strLOBBY);
    }

    [PunRPC]
    void yRPCSetUserDatas(byte rank, byte index, string nick, byte imgType, short score){
        // Objects
        TrSO_MultiProfile profile = TrProfileDatas.xInstance.zGetProfileData(imgType);

        if (rank < _players.Length){
            TrMultiResultPlayerComponents player = _players[rank];
            player.gameObject.SetActive(true);
            player._txtNickname.text = nick;

            player._imgCharacter.sprite = profile._spResultCharacter;
            player._srProfileCharacter.sprite = profile._spProfile;

            player._srMedal.transform.localPosition = profile._posMedal;
            Sprite spMedal;
            if (rank == 0)
                spMedal = profile._spGoldMedal;
            else if (rank == 1)
                spMedal = profile._spSilverMedal;
            else
                spMedal = profile._spBrownMedal;

            player._srMedal.sprite = spMedal;
            player._srMedal.sortingOrder = player._imgCharacter.sortingOrder + 1;

            if (index == TrNetworkManager._myIndex)
                player._srBgNickname.sprite = _spBgProfileMine;
            else
                player._srBgNickname.sprite = _spBgProfileNotMine;
        }

        // UI
        _uiPlayers[rank].gameObject.SetActive(true);
        _uiPlayers[rank]._nickNames.text = nick;
        _uiPlayers[rank]._txtScores.text = score.ToString();
        _uiPlayers[rank]._imgCharacter.sprite = profile._spProfile;
        _uiPlayers[rank]._imgCharacter.SetNativeSize();

        if (index == TrNetworkManager._myIndex){
            _uiPlayers[rank]._imgBg.sprite = _spBgRankMine[rank];
            _goRankMedals[rank].SetActive(true);
            if(rank == 0)
                for(int i=0; i<_txtWinnerScores.Length; i++){
                    _txtWinnerScores[i].text = string.Format("{0}P", score.ToString());
                }

        }
    }

    [PunRPC]
    void yRPCFadeOut(){
        _imgFade.DOFade(0, 1f);
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
        for(int i=0; i<_players.Length; i++){
            _players[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < _uiPlayers.Length; i++){
            _uiPlayers[i].gameObject.SetActive(false);
            _goRankMedals[i].gameObject.SetActive(false);
            _uiPlayers[i]._imgBg.sprite = _spBgRankNotMine[i];
        }
    }
    
    void Start()
    {
        TrAudio_Music.xInstance.zzPlayMain(0.25f);
        GameManager.xInstance.zSetCamera();
        _imgFade.alpha = 1;
        PhotonNetwork.AutomaticallySyncScene = false;
        if (PhotonNetwork.IsMasterClient){
            for (byte i = 0; i < TrNetworkManager._liPlayersData.Count; i++){
                TrNetworkManager._liPlayersData[i]._score = 
                    TrNetworkManager._playerScores[TrNetworkManager._liPlayersData[i]._index];
            }
            List<TrPlayersData> orderPlayers = 
                TrNetworkManager._liPlayersData.OrderByDescending(x => x._score).ToList();

            for(byte i=0; i<_uiPlayers.Length; i++)
            {
                if (i >= orderPlayers.Count) break;
                photonView.RPC("yRPCSetUserDatas", RpcTarget.AllBuffered,
                    i, orderPlayers[i]._index, orderPlayers[i]._nickname, orderPlayers[i]._imgType, orderPlayers[i]._score);
            }

            photonView.RPC("yRPCFadeOut", RpcTarget.AllBuffered);
        }
    }
}
