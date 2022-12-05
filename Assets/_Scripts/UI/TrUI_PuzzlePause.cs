using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;
using TMPro;


public class TrUI_PuzzlePause : MonoBehaviour
{
    static TrUI_PuzzlePause _instance;
    public static TrUI_PuzzlePause xInstance { get { return _instance; } }

    public GameObject _goPause; // ���� ������ ������ �޴� ���� ������Ʈ
    public CanvasGroup _fade;

    [SerializeField] GameObject _goQuitButton;
    [SerializeField] GameObject _goQuitAskWindow;

    bool _isAction = false;
    public bool _isWinOpen = false;

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
            yInitFade();
        }
        else
        {
            _instance.yInitFade();
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (GameManager._type != TT.enumGameType.Multi)
        {
            if (_goPause != null){
                yActiveGoPause(false);
                Time.timeScale = 1;

                //if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor)
                //    _goQuitButton.SetActive(true);
                //else
                //    _goQuitButton.SetActive(false);
            }
        }
    }

    void yInitFade()
    {
        _fade.alpha = 1;
        _fade.DOFade(0, 2f);
    }

    public void xAppearPauseWindow()
    {
        if (_isAction || _fade.alpha > 0.1f) return;
        GameManager.xInstance._isGameStopped = true;
        TrAudio_UI.xInstance.zzPlay_ClickButtonSmall();
        yActiveGoPause(true);
        TrUI_PuzzleBtnPause.xInstance.zActivePause();
        Time.timeScale = 0;

    }
    public void xDisappearPauseWindow()
    {
        if (_isAction) return;
        GameManager.xInstance._isGameStopped = false;
        TrAudio_UI.xInstance.zzPlay_ClickButtonSmall();
        yActiveGoPause(false);
        TrUI_PuzzleBtnPause.xInstance.zDisablePause();
        Time.timeScale = 1;

    }
    public void xClickRestart()
    {
        if (_isAction) return;
        _isAction = true;
        Time.timeScale = 1;

        TrAudio_UI.xInstance.zzPlay_ClickButtonSmall();
        if (GameManager._type == TT.enumGameType.Train)
            _fade.DOFade(1, 1f).OnComplete(() => GameManager.xInstance.zSetPuzzleGame());
        else if(GameManager._type == TT.enumGameType.Challenge)
            StartCoroutine(StaminaManager.xInstance.zCheckStamina(()=>
            _fade.DOFade(1, 1f).OnComplete(() => GameManager.xInstance.zSetPuzzleGame()),
                yFailedRestart));
    }

    void yFailedRestart(){
        Time.timeScale = 0;
        _isAction = false;
    }

    public void zOnClickSetting()
    {
        TrAudio_UI.xInstance.zzPlay_ClickButtonSmall();
        TrUI_Window_Setting.xInstance.zShow(true);
    }

    public void xClickExit()
    {
        if (_isAction) return;
        _isAction = true;

        Time.timeScale = 1;
        TrAudio_UI.xInstance.zzPlay_ClickButtonSmall();
        GameManager.xInstance._isGameStarted = false;
        _fade.DOFade(1, 2f).OnComplete(() => SceneManager.LoadScene(TrProjectSettings.strLOBBY));
    }

    public void zGameEndAndSceneMove()
    {
        StartCoroutine(yEndGame());
    }

    void yActiveGoPause(bool active)
    {
        _isWinOpen = active;
        _goPause.SetActive(active);
    }

    IEnumerator yEndGame()
    {
        TrAudio_Music.xInstance.zStopMusic();
        yield return TT.WaitForSeconds(2.5f);
        _fade.DOFade(1, 2f);
        yield return TT.WaitForSeconds(2f);
        SceneManager.LoadScene(TrProjectSettings.strRESULT);
    }

    public void zActiveQuitAskWindow(bool isActive)
    {
        TrAudio_UI.xInstance.zzPlay_ClickButtonSmall();
        _goQuitAskWindow.SetActive(isActive);
    }

    public void zQuitApplication()
    {
        TrAudio_UI.xInstance.zzPlay_ClickButtonSmall();
        Application.Quit();
    }
}