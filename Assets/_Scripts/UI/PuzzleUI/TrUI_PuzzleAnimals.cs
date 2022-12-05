using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[System.Serializable]
class TrPuzzleComboInfo
{
    public Vector2 _valueRange;
    public Color _colorInLine = new Color(1, 1, 1, 1);
    public Color _colorOutLine = new Color(1, 1, 1, 1);
}

public class TrUI_PuzzleAnimals : TrUI_PuzzleManager
{
    static TrUI_PuzzleAnimals _instance;
    public static TrUI_PuzzleAnimals xInstance { get { return _instance; } }

    [Header("좌, 우 이미지 하나씩 줄 것")]
    //[SerializeField] Image[] _imgButtonAnimals;
    [SerializeField] Image _imgBombFill;
    [SerializeField] Image _imgMaskFill;
    TrUI_AnimalCombo[] _combos;
    [SerializeField] Vector2 _minLeft, _maxLeft, _minRight, _maxRight;
    [SerializeField] CanvasGroup _imgTimeTenses;
    public Image[] _imgAnimalButtons;
    [SerializeField] Sprite[] _spButtonAnimals;
    [SerializeField] float _valueComboTextDegree;
    [SerializeField] float _valueMissImgDegree;
    [SerializeField] TrPuzzleComboInfo[] _comboInfos;
    int _indexCurrCombo;
    int _indexMaxCombo;

    // 시간 추가 이펙트
    [SerializeField] CanvasGroup _cgClock;
    float _posXPlusTime;
    float _originPosYPlusTime;
    float _targetPosYPlusTime;
    [SerializeField] float _valueDistanceYPlusTime = 2f;
    float _valueTimeUpPlusTime = 1f;

    public void zSetPlusTime(int num){
        TextMeshProUGUI txt = _cgClock.GetComponentInChildren<TextMeshProUGUI>();
        txt.text = string.Format("+{0}", num.ToString());
    }

    void yResetPlusTimeText(){
        _cgClock.transform.position = new Vector2(_posXPlusTime, _originPosYPlusTime);
        _cgClock.alpha = 0;
    }

    public void zEffectPlusTime(){
        TrAudio_UI.xInstance.zzPlay_PlusTime(0f);
        _cgClock.alpha = 1;
        _cgClock.DOFade(0, _valueTimeUpPlusTime);
        _cgClock.transform
            .DOMoveY(_targetPosYPlusTime, _valueTimeUpPlusTime)
            .OnComplete(()=>yResetPlusTimeText());
    }

    public void zSetBombGauge(int curr, int max)
    {
        _imgBombFill.fillAmount = (float)curr / max;
    }

    public void zSetMaskGauge(int curr, int max){
        _imgMaskFill.fillAmount = (float)curr / max;
    }

    IEnumerator yTimerTenseExec(){
        int reach = 1;
        while (GameManager.xInstance._isGameStarted){
            _imgTimeTenses.DOFade(reach, 0.5f);

            yield return TT.WaitForSeconds(0.5f);

            reach = reach == 1 ? 0 : 1;
        }
        _imgTimeTenses.alpha = 0;
    }

    public void zSetTimerTenseEffect(){
        StartCoroutine(yTimerTenseExec());
    }

    public void zEffectCombo(int num){
        if (_indexCurrCombo++ >= _indexMaxCombo) _indexCurrCombo = 0;

        int randLeftOrRight = Random.Range(0, 2);
        int randX, randY;

        if(randLeftOrRight == 0){
            randX = Random.Range((int)_minLeft.x, (int)_maxLeft.x+1);
            randY = Random.Range((int)_minLeft.y, (int)_maxLeft.y + 1);
        }
        else{
            randX = Random.Range((int)_minRight.x, (int)_maxRight.x + 1);
            randY = Random.Range((int)_minRight.y, (int)_maxRight.y + 1);
        }

        TrUI_AnimalCombo combo = _combos[_indexCurrCombo];
        Text textMesh = combo._txtCurrCombo;
        Outline ol = combo._olText;
        ParticleSystem ps = combo._psCombo;
        GameObject imgMiss = combo._imgMiss.gameObject;
        GameObject imgCorrect = combo._imgCorrect.gameObject;
        CanvasGroup canvasGroup = combo.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;

        for(int i=0; i< _comboInfos.Length; i++){
            if(num >= _comboInfos[i]._valueRange.x && num < _comboInfos[i]._valueRange.y){
                textMesh.color = _comboInfos[i]._colorInLine;
                ol.effectColor = _comboInfos[i]._colorOutLine;
                break;
            }
        }

        if (num == 0){
            textMesh.text = "";
            //textMesh.fontSize = _numFontSizeMiss;
            imgMiss.SetActive(true);
            imgCorrect.SetActive(false);
            imgMiss.transform.eulerAngles = new Vector3(0, 0, _valueMissImgDegree * (randLeftOrRight == 0 ? 1 : -1));
        }
        else
        {
            imgMiss.SetActive(false);
            imgCorrect.SetActive(true);
            ps.Play();
            textMesh.text = num.ToString();
            //textMesh.fontSize = _numFontSizeCorrect;
            textMesh.transform.eulerAngles = new Vector3(0, 0, _valueComboTextDegree * (randLeftOrRight == 0 ? 1 : -1));
        }
       
        combo.transform.localPosition = new Vector2(randX, randY);
        combo.transform.DOLocalMoveY(randY + 200, 1f);
        canvasGroup.DOFade(0, 1.5f);
    }

    public void zOnClickButton(int num){
        TrPuzzleAnimals.xInstance.zCheckAnswer(num);
        if (num > 0){
            TrAudio_UI.xInstance.zzPlay_AnimalsRBtnClick();
        }
        else{
            TrAudio_UI.xInstance.zzPlay_AnimalsLBtnClick();
        }
    }

    public void zSetButtonSp(int num, int numSp){
        _imgAnimalButtons[num].sprite = _spButtonAnimals[numSp];
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
        _combos = GetComponentsInChildren<TrUI_AnimalCombo>();
    }

    void Start(){
        _indexMaxCombo = _combos.Length-1;
        _imgBombFill.fillAmount = 0;
        _imgMaskFill.fillAmount = 0;

        Vector2 pos = TrUI_PuzzleTimer.xInstance._txtTimer.transform.position;
        pos -= new Vector2(0, 0.25f);
        _targetPosYPlusTime = pos.y;
        _originPosYPlusTime = pos.y - _valueDistanceYPlusTime;
        _posXPlusTime = pos.x;
        _cgClock.transform.position = new Vector2(_posXPlusTime, _originPosYPlusTime);
        _cgClock.alpha = 0;

    }
}
