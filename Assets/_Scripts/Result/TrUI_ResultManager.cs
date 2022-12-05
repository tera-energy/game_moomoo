using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class TrUI_ResultManager : MonoBehaviour
{
    static TrUI_ResultManager _instance;
    public static TrUI_ResultManager xInstance { get { return _instance; } }
    [SerializeField] TextMeshProUGUI _txtScore;
    [SerializeField] TextMeshProUGUI _ribText;
    [SerializeField] CanvasGroup _imgFade;
    [SerializeField] RectTransform[] _rtCookies;
    [SerializeField] Sprite _srBurnCookie;
    [SerializeField] Sprite _srTwinkleCookie;
    [SerializeField] ParticleSystem[] _burnCookie;
    [SerializeField] ParticleSystem[] _twinkleCookie;
    [SerializeField] ParticleSystem[] _particle;
    [SerializeField] ParticleSystem _dust;
    [SerializeField] ParticleSystem[] _fireCracker;
    [SerializeField] RectTransform _newRecord;
    [SerializeField] RectTransform _bestScore;
    [SerializeField] Animator[] _bestScoreAnim;
    [SerializeField] ParticleSystem _firstPar;
    [SerializeField] AudioClip _acGood;
    [SerializeField] AudioClip _acBad;
    [SerializeField] AudioClip _acGreat;
    [SerializeField] GameObject _goFireCracker;
    [SerializeField] Vector2 _fireInitPos;
    [SerializeField] Vector2 _fireMinRange;
    [SerializeField] Vector2 _fireMaxRange;
    bool _isSetScoreCom = false;
    bool _isSkipScoreEffect = false;
    bool _isAction = false;

    [SerializeField] TrUI_HoldButton _btnRetry;


    public void xBtnExit()
    {
        if (_isAction) return;
        _isAction = true;
        TrAudio_UI.xInstance.zzPlay_ClickButtonSmall();
        _imgFade.DOFade(1, 0.5f).OnComplete(() => SceneManager.LoadScene(TrProjectSettings.strLOBBY));
        
    }
    public void xBtnRetry()
    {
        if (_isAction) return;
        _isAction = true;

        if (GameManager._type == TT.enumGameType.Train)
            _imgFade.DOFade(1, 0.5f).OnComplete(() => GameManager.xInstance.zSetPuzzleGame());
        else if (GameManager._type == TT.enumGameType.Challenge)
            StartCoroutine(StaminaManager.xInstance.zCheckStamina(()=> 
            _imgFade.DOFade(1, 0.5f).OnComplete(() => 
            GameManager.xInstance.zSetPuzzleGame())));
    }


    IEnumerator yEffectIncreaseScore(TextMeshProUGUI text, int score, bool isScore){
        float maxScore = score;
        float currScore = maxScore / 2;
        float speed = maxScore - currScore;
        int soundScore = 1;
        while (currScore < maxScore){
            if (_isSkipScoreEffect){
                text.text = ((int)maxScore).ToString();
                break;
            }
            currScore += Time.deltaTime * speed;
            if (isScore && currScore >= soundScore){
                while (currScore > soundScore)
                    soundScore++;

                if (speed > 20){
                    if ((int)speed % 3 == 0)
                        TrAudio_SFX.xInstance.zzPlay_AnimalsBomb();
                }
                else
                    TrAudio_SFX.xInstance.zzPlay_AnimalsBomb();
            }
            text.text = ((int)currScore).ToString();
            speed = (maxScore - currScore);
            if (speed < 1)
                speed = 1;
            yield return null;
        }
        text.text = score.ToString();

        if (isScore){
            yield return TT.WaitForSeconds(0.5f);
            _isSetScoreCom = true;
        }
    }

    IEnumerator yShotFireCracker(int n){
        GameObject shot = Instantiate(_goFireCracker);

        Color col = Color.white;
        switch (n){
            case 0:
                ColorUtility.TryParseHtmlString("#faeeb1", out col);
                break;
            case 1:
                ColorUtility.TryParseHtmlString("#a6d8df", out col);
                break;
            case 2:
                ColorUtility.TryParseHtmlString("#fac9b1", out col);
                break;
        }
        SpriteRenderer sr = shot.GetComponent<SpriteRenderer>();
        sr.color = col;

        Vector2[] vec = new Vector2[4];
        vec[0] = _fireInitPos;
        for (int i = 1; i < 3; i++){
            float randX = Random.Range(_fireMinRange.x, _fireMaxRange.x);
            float randY = Random.Range(_fireMinRange.y, _fireMaxRange.y);
            vec[i] = new Vector2(randX, randY);
        }
        vec[3] = _fireCracker[n].transform.position;

        float currValue = 0;
        float speed = Random.Range(0.3f, 0.6f);
        TrAudio_SFX.xInstance.zzPlay_Fire(0f);
        while (Vector2.Distance(shot.transform.position, vec[3]) > 0.1f)
        {
            currValue += Time.deltaTime * speed;
            shot.transform.position = TT.zBezierCurve(vec[0], vec[1], vec[2], vec[3], currValue);
            yield return null;
        }

        _fireCracker[n].Play();
        sr.color = new Color(1, 1, 1, 0);
        ParticleSystem ps = shot.GetComponentInChildren<ParticleSystem>();
        TrAudio_SFX.xInstance.zzPlay_FireCracker(-1f);
        ps.Stop();
    }

    IEnumerator ySetGameDatas(){
        int score = GameManager._score;

        int rank = -1;
        if (GameManager._type == TT.enumGameType.Challenge){
            if (DatabaseManager._myDatas != null){
                if (score >= DatabaseManager._myDatas.maxScore){
                    DatabaseManager._myDatas.maxScore = score;
                    yield return StartCoroutine(DatabaseManager.xInstance.zSetMaxScore());
                }
            }

            bool isChangeMyScore = false;
            if (DatabaseManager._liMyScores == null || DatabaseManager._liMyScores.Count == 0){
                isChangeMyScore = true;
                DatabaseManager._liMyScores = new List<int>();
                DatabaseManager._liMyScores.Add(score);
            }
            else{
                for (int i = 0; i < DatabaseManager._liMyScores.Count; i++)
                {
                    if (i >= 5) break;

                    if (DatabaseManager._liMyScores[i] < score)
                    {
                        isChangeMyScore = true;
                        DatabaseManager._liMyScores.Insert(i, score);
                        break;
                    }
                }

                if (DatabaseManager._liMyScores.Count >= 5)
                    DatabaseManager._liMyScores.RemoveAt(5);
            }

            for(int i= DatabaseManager._liMyScores.Count; i<5; i++){
                if (!isChangeMyScore) isChangeMyScore = true;
                DatabaseManager._liMyScores.Add(0);
            }

            if (isChangeMyScore)
            {
                yield return StartCoroutine(DatabaseManager.xInstance.zSetMyScores());

            }
        }

        GameManager._canBtnClick = true;

        _imgFade.DOFade(0, 1f);
        yield return new WaitUntil(() => _imgFade.alpha == 0);
        StartCoroutine(yEffectIncreaseScore(_txtScore, score, true));
        
        yield return new WaitUntil(() => _isSetScoreCom);
        Color color = Color.white;
        int burnCookie = 0;

        

        bool isNewRecord = rank == -1 ? false : true;
        bool isBestScore = rank == 0 ? true : false;
        if (isNewRecord){
            if (isBestScore){
                _bestScore.gameObject.SetActive(true);
            }
            else
                _newRecord.gameObject.SetActive(true);
            TrAudio_SFX.xInstance.zzPlayNewScore(0.1f);
            _dust.Play();
            yield return TT.WaitForSeconds(1f);
        }


        if (score <= 200) {
            burnCookie = 2;
            for(int i = 0; i < burnCookie; i++){
                _rtCookies[i].GetComponent<Image>().sprite = _srBurnCookie;
                _burnCookie[i].Play();
                TrAudio_SFX.xInstance.zzPlayBurnCookie(0f);
                yield return TT.WaitForSeconds(1f);
            }
            _ribText.text = "BAD";
            ColorUtility.TryParseHtmlString("#ffa2a2", out color);
            TrAudio_UI.xInstance.zzPlay_WangWaWang(0f);
            TrAudio_Music.xInstance.zzPlayMain(1.7f, _acBad);            
        }
        else if (score > 200 && score <= 400){
            burnCookie = 1;
            for (int i = 0; i < burnCookie; i++){
                _rtCookies[i].GetComponent<Image>().sprite = _srBurnCookie;
                _burnCookie[i].Play();
                TrAudio_SFX.xInstance.zzPlayBurnCookie(0f);
                yield return TT.WaitForSeconds(1f);
            }
            _ribText.text = "GOOD!";
            ColorUtility.TryParseHtmlString("#ffffff", out color);
            TrAudio_UI.xInstance.zzPlay_PangPang(1.2f);
            TrAudio_Music.xInstance.zzPlayMain(3f, _acGood);
            _firstPar.Play();

            yield return TT.WaitForSeconds(1f);
            for (int i = 0; i < _particle.Length; i++){
                _particle[i].Play();
            }
        }
        else if(score > 400){
            burnCookie = 0;
            _ribText.text = "GREAT!!";
            ColorUtility.TryParseHtmlString("#fff04e", out color);
            _ribText.color = TT.zSetColor(TT.enumTrRainbowColor.PURPLE);
            TrAudio_UI.xInstance.zzPlay_GreatSound(0.5f);
            
            TrAudio_Music.xInstance.zzPlayMain(3.8f, _acGreat);
            _firstPar.Play();

            yield return TT.WaitForSeconds(1f);
            for (int j = 0; j < _particle.Length; j++)            
                _particle[j].Play();
            for (int i = 0; i < _fireCracker.Length; i++)
                StartCoroutine(yShotFireCracker(i));
        }

        for (int i = burnCookie; i < _twinkleCookie.Length; i++)
        {          
            _twinkleCookie[i].Play();          
            _rtCookies[i].GetComponent<Image>().sprite = _srTwinkleCookie;
        }

        _ribText.color = color;
        _ribText.gameObject.SetActive(true);
        
        StartCoroutine(yStarsEffect(burnCookie));
        yield return TT.WaitForSeconds(10f);
    }

    IEnumerator yStarsEffect(int num){
        yield return TT.WaitForSeconds(1f);
        
        Vector3 target = new Vector3(1.3f, 1.3f, 1.3f);
        Vector3 origin = new Vector3(1f, 1f, 1f);
        while (true){
            for(int i=2; i >= num; i--)
            {
                _rtCookies[i].DOScale(target, 0.25f).OnComplete(()=> _rtCookies[i].DOScale(origin, 0.25f));
                yield return TT.WaitForSeconds(0.75f);
            }

            int randWaitTime = Random.Range(1, 5);
            yield return TT.WaitForSeconds(randWaitTime);
        }
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
        GameManager._canBtnClick = false;
        GameManager.xInstance.zSetCamera();
        TrAudio_Music.xInstance.zzPlayMain(0);
        _txtScore.text = "0";
        _imgFade.alpha = 1;

        StartCoroutine(ySetGameDatas());
    }

#if PLATFORM_ANDROID
    void Update(){
        if (Input.GetMouseButtonDown(0)){
            _isSkipScoreEffect = true;
        }
    }
#endif
}
