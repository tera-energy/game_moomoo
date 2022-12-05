using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;

public class TrPuzzleManager : MonoBehaviourPunCallbacks
{
    protected float _startTime = 2f;

    [SerializeField] protected int _maxGameTime = 30;
    protected float _currGameTime;

    protected int _currScore;

    // multi
    byte _numEnterUser;
    bool _isMultiGameStart;
    byte _numEndUser;

    protected float[] _numCurrChallenges = new float[3];
    protected bool _isThridChallengeSame;
    int _maxCombo = 0;
    protected int _currCombo;

    [SerializeField] protected TrUI_PuzzleManager _ui;

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if(GameManager._type != TT.enumGameType.Multi)
                TrUI_PuzzlePause.xInstance.xAppearPauseWindow();
        }
    }

    [PunRPC]
    protected void yRPCSetScore(byte index, short score){
        _ui.zSetUsersScore(index, score);
    }

    protected virtual void zWrong(bool isNotice = true, int score = -1)
    {
        if (!GameManager.xInstance._isGameStarted) return;
        if (isNotice)
            TrUI_PuzzleNotice.xInstance.zSetNotice("Wrong");
        _currScore += score;
        if (_currScore < 0) _currScore = 0;
        if (GameManager._type == TT.enumGameType.Multi){
            photonView.RPC("yRPCSetScore", RpcTarget.AllBuffered, TrNetworkManager._myIndex, (short)_currScore);
        }else
            TrUI_PuzzleScore.xInstance.zSetScore(_currScore);
    }
    protected virtual void zCorrect(bool isNotice = true, int score = 1)
    {
        if (!GameManager.xInstance._isGameStarted) return;
        if (isNotice)
            TrUI_PuzzleNotice.xInstance.zSetNotice("Correct!");

        _currScore += score;
        if(GameManager._type == TT.enumGameType.Multi){
            photonView.RPC("yRPCSetScore", RpcTarget.AllBuffered, TrNetworkManager._myIndex, (short)_currScore);
        }
        else
            TrUI_PuzzleScore.xInstance.zSetScore(_currScore);

        if (_currCombo > _maxCombo)
            _maxCombo = _currCombo;
    }

    protected virtual void zSetResultGame()
    {
    }

    void yEndGame()
    {
        GameManager.xInstance._isGameStarted = false;
        zSetResultGame();
        GameManager.xInstance._numMaxCombo = _maxCombo;
        GameManager._score = _currScore;

        TrUI_PuzzleNotice.xInstance.zSetNoticeWithMoomoo("GAME OVER", 23, 10f);

        if (GameManager._type != TT.enumGameType.Multi)
            TrUI_PuzzlePause.xInstance.zGameEndAndSceneMove();
        else
            photonView.RPC("yRPCEndGame", RpcTarget.AllBuffered, TrNetworkManager._myIndex);
    }

    [PunRPC]
    protected void yRPCEndGame(byte index){
        StartCoroutine(yCheckWaitEnd());
        _ui.zSetEndUser(index);
    }

    IEnumerator yCheckWaitEnd(){
        _numEndUser++;

        while (true){
            if (_numEndUser >= TrNetworkManager._numCurrUser) break;
            yield return null;
        }
        //yield return new WaitUntil(() => _numEndUser >= TrNetworkManager._numCurrUser);

        TrUI_PuzzlePause.xInstance._fade.DOFade(1, 1f);

        if (PhotonNetwork.IsMasterClient){
            yield return new WaitForSeconds(1f);
            UnityEngine.SceneManagement.SceneManager.LoadScene(TrProjectSettings.strMULTIRESULT);
        }

    }

    void yGameTimer()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Keypad9)) _currGameTime += 9;
#endif

        if (_currGameTime >= 0){
            _currGameTime -= Time.deltaTime;
            TrUI_PuzzleTimer.xInstance.zUpdateTimerBar(_maxGameTime, _currGameTime);   
        }
        else
        {
            // 점수 비교
            GameManager.xInstance._isGameStarted = false;
            TrUI_PuzzleTimer.xInstance.zUpdateTimerBar(_maxGameTime, 0);
            yEndGame();
        }


    }

    [PunRPC]
    protected void yRPCCheckEnter(){
        StartCoroutine(yCoCheckEnter());
    }

    IEnumerator yCoCheckEnter(){
        ++_numEnterUser;

        yield return new WaitUntil(() => _numEnterUser >= TrNetworkManager._numCurrUser);
        
        _isMultiGameStart = true;
    }
 
    [PunRPC]
    protected void yRPCSetUserDatas(byte index, string nick, byte imgType){
        if(TrNetworkManager._liPlayersData == null){
            TrNetworkManager._liPlayersData = new List<TrPlayersData>();
        }

        TrNetworkManager._liPlayersData.Add(new TrPlayersData(index, nick, imgType, 0));
        _ui.zSetUserInfos(index == TrNetworkManager._myIndex, index, nick, imgType);
    }

    protected virtual void yBeforeReadyGame() {
        if (GameManager._type == TT.enumGameType.Multi){
            TrUI_PuzzleBtnPause.xInstance._goPauseButtons[0].SetActive(false);
            TrUI_PuzzleScore.xInstance.gameObject.SetActive(false);


            TrNetworkManager._playerScores = new short[TrNetworkManager._numMaxUser];
            for(byte i = 0; i < TrNetworkManager._numMaxUser; i++){
                TrNetworkManager._playerScores[i] = 0;
            }

            photonView.RPC("yRPCSetUserDatas", RpcTarget.AllBuffered, 
                TrNetworkManager._myIndex, 
                DatabaseManager._myDatas.nickName, 
                TrNetworkManager._imgType);
        }
    }

    // 시작 카운트
    protected virtual IEnumerator yProcReadyGame(){
        yBeforeReadyGame();
        _maxCombo = 0;
        _isMultiGameStart = false;

        TT.UtilDelayedFunc.zCreateAtLate(() => TrAudio_UI.xInstance.zzSetFlatVolume());

        yield return new WaitUntil(() => TrUI_PuzzlePause.xInstance._fade.alpha == 0);
        
        if(GameManager._type == TT.enumGameType.Multi){
            photonView.RPC("yRPCCheckEnter", RpcTarget.AllBuffered);
            yield return new WaitUntil(() => _isMultiGameStart);
        }

        TrUI_PuzzleNotice.xInstance.zSetNotice("READY", _startTime);
        yield return TT.WaitForSeconds(_startTime);
        TrUI_PuzzleNotice.xInstance.zSetNoticeWithMoomoo("GO!", 45, 1f);
        yield return TT.WaitForSeconds(1f);
        yAfterReadyGame();
    }

    protected virtual void yAfterReadyGame() { }

    void Start()
    {
        GameManager.xInstance.zSetCamera();
        GameManager._canBtnClick = true;
        GameManager.xInstance._isGameStarted = false;
        TrAudio_UI.xInstance.zzPlay_Ready(0, 2f);
        TrAudio_UI.xInstance.zzPlay_Ready(1, 4f);

        _currGameTime = _maxGameTime;
        TrUI_PuzzleTimer.xInstance.zUpdateTimerBar(_maxGameTime, _currGameTime);

        StartCoroutine(yProcReadyGame());
    }

    protected virtual void Update()
    {
#if PLATFORM_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape) && GameManager._type != TT.enumGameType.Multi)
            TrUI_PuzzlePause.xInstance.xAppearPauseWindow();
#endif

        if (!GameManager.xInstance._isGameStarted) return;
        yGameTimer();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Z))
        {
            yEndGame();
        }
#endif
    }
}
