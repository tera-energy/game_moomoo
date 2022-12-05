using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

class TrAnimalAnswer{
    public int _numSp;
    public bool _isRight;
}

public enum TrAnimalItems{
    BOMB = 1,
    SMOKE,
    CLOCK
}

public class TrPuzzleAnimals : TrPuzzleManager
{
    static TrPuzzleAnimals _instance;
    public static TrPuzzleAnimals xInstance { get { return _instance; } }
    [SerializeField] int _numViewAnimals;
    TrAnimal[] _arrAnimals;
    [SerializeField] float _posYInstantiateAnimals;
    [SerializeField] float _posYBaseViewAnimals;
    [SerializeField] float _posYDistanceViewAnimals;
    float _posYCurrMaxViewAnimal;
    [SerializeField] float _valueTimeAnimalPosYReach;
    [SerializeField] float _valueInitAnimalDropTerm;
    [SerializeField] float _valueSpeedAnimalDrop;
    [SerializeField] TrAnimal _animalView;
    Queue<TrAnimal> _queueAnimals = new Queue<TrAnimal>();

    [SerializeField] Transform _trAnimalsMoveParent;
    [SerializeField] Transform _trAnimalsNullParent;
    TrAnimalAnswer[] _animalAnswerInfos;
    [SerializeField] Sprite[] _spAnimals;
    int _numUseAnimals = 2;

    [SerializeField] float _posXTargetAnimal;
    [SerializeField] float _speedAnimalMoveX;

    [SerializeField] float _valueComboResetTime;
    float _valueCurrComboResetTime;

    [SerializeField] int _maxIndex;
    int _currIndex;
    public int _numStackMove;
    public bool _isParentMove;
    float _comboBonus = 0;
    int _currSort;
    bool _isOnVibration;


    [SerializeField] ParticleSystem _psComboBgEffect;
    
    // ����ź ������
    [SerializeField] GameObject[] _goSmokes;
    [SerializeField] int _maxSmokeGauge;
    int _currSmokeGauge;
    [SerializeField] int _valueSmokeTime;
    Coroutine _coroutineSmoke;
    
    // ��ź ������
    [SerializeField] ParticleSystem[] _psBombEffect;
    [SerializeField] ParticleSystem _psBombSmog;
    [SerializeField] int _maxBombGauge;
    int _currBombGauge;
    [SerializeField] int _numBombGaugeStartCombo;
    [SerializeField] int _numBombScore;
    
    // �ð� ������
    bool _isCurrBomb = true;
    [SerializeField] int _numPlusTime = 2;
    
    bool _isUsedItem;

    // ���ƴٴϴ� ĳ����(?)
    [SerializeField] TrWanderAnimal[] _wanderCharacters;
    [SerializeField] Vector2 _moveCharactersSpeedRange;
    [SerializeField] Vector2 _moveCharactersMoveMinRange;
    [SerializeField] Vector2 _moveCharactersMoveMaxRange;
    [SerializeField] float _moveCharactersStartXPos;
    [SerializeField] Vector2 _moveCharacterRespawnTime;

    [SerializeField] RectTransform[] _rectFlowers;

    [SerializeField] int _valueTimerTense;
    bool _isTimeTenseEffectExec;
    Coroutine _coMoveFlower;

    /*[SerializeField] TrUI_HoldButton _btnLeft;
    [SerializeField] TrUI_HoldButton _btnRight;*/

    void ySetComboParticle(int num){
        ParticleSystem.EmissionModule emission = _psComboBgEffect.emission;
        ParticleSystem.Burst burst = new ParticleSystem.Burst
        {
            count = num
        };
        emission.SetBurst(0, burst);
    }

    void yCheckCombo(){
        TrUI_PuzzleAnimals.xInstance.zEffectCombo(_currCombo);
        if (_currCombo == 0){
            _psComboBgEffect.Stop();
            _comboBonus = 0;
            return;
        }

        if(_currCombo == 10){
            _comboBonus = 0.2f;
            ySetComboParticle(10);
            _psComboBgEffect.Play();
        }
        else if(_currCombo == 50){
            _comboBonus = 0.4f;
            ySetComboParticle(30);
        }else if(_currCombo == 100){
            _comboBonus = 0.6f;
            ySetComboParticle(50);
        }else if(_comboBonus == 150){
            _comboBonus = 0.8f;
        }

        zCorrect(false, 1 + (int)(_comboBonus * (_currCombo / 5)));
        if(_currCombo >= _numBombGaugeStartCombo && !_isUsedItem)
        {
            TrUI_PuzzleAnimals.xInstance.zSetBombGauge(++_currBombGauge, _maxBombGauge);
        }
    }

    IEnumerator yExecSmokeItem(TrAnimal animal){
        int numSmoke = _goSmokes.Length;
        animal.zExplode();
        _psBombSmog.Play();
        SpriteRenderer[] srs = new SpriteRenderer[numSmoke];
        animal.zIsCorrect(false, 1);

        TrAudio_SFX.xInstance.zzPlay_AnimalsSmokeExec(0.1f);
        for (int i = 0; i < numSmoke; i++){
            TrAudio_SFX.xInstance.zzPlay_AnimalsSmoke(0.1f);
            srs[i] = _goSmokes[i].GetComponent<SpriteRenderer>();
            _goSmokes[i].transform.localScale = Vector2.zero;
            srs[i].color = new Color(1, 1, 1, 0);
            
            _goSmokes[i].transform.DOScale(1, 0.5f);
            srs[i].DOColor(new Color(1, 1, 1, 1), 0.5f);

            yield return TT.WaitForSeconds(0.25f);

        }
        //yield return TT.WaitForSeconds(0f);
        animal.gameObject.SetActive(false);
        _isUsedItem = false;
        _numStackMove++;
        yAddViewAnimal();
        yield return TT.WaitForSeconds(_valueSmokeTime);

        if(GameManager.xInstance._isGameStarted)
            for (int i=0; i< numSmoke; i++)
            {
                _goSmokes[i].transform.DOScale(0, 0.25f);
                srs[i].DOColor(new Color(1, 1, 1, 0), 0.25f);

                yield return TT.WaitForSeconds(0.1f);
            }

    }
    IEnumerator yExecBombItem(TrAnimal bombAnimal){
        bombAnimal.gameObject.SetActive(false);
        //TrAudio_SFX.xInstance.zzPlay_AnimalsBomb(0.1f);
        TrAudio_SFX.xInstance.zzPlay_AnimalsBombExec(0f);
        for (int i = 0; i < _psBombEffect.Length; i++){
            _psBombEffect[i].Play();
            ParticleSystem.MainModule ps = _psBombEffect[i].main;   
            ps.loop = true;
        }

        yield return TT.WaitForSeconds(0.5f);
        _numStackMove++;
        yAddViewAnimal();
        int count = _numBombScore;
        while (count-- > 0)
        {
            if (TrProjectSettings._concept == TrProjectSettings.enumGameConcept.Temple)
            {
                if (count % 2 == 1)
                {
                    TrAudio_SFX.xInstance.zzPlay_TempleBomb(0f);
                }
                
            }
            else{
                TrAudio_SFX.xInstance.zzPlay_AnimalsBomb(0.1f);
            }

            //TrAudio_SFX.xInstance.zzPlay_AnimalsBomb(0.1f);
            _numStackMove++;
            if (_isOnVibration)
                Vibration.Vibrate(0.2f, 1, 1);
            TrAnimal animal = _queueAnimals.Dequeue();
            _valueCurrComboResetTime = _valueComboResetTime;
            _currCombo++;
            int num = animal.zGetAnimalIsRight() ? 1 : -1;
            animal.zIsCorrect(true, num);
            yCheckCombo();
            animal.transform.parent = _trAnimalsNullParent;
            animal.transform.localPosition = new Vector3(0, _posYBaseViewAnimals, 0);
            animal.gameObject.SetActive(false);

            yAddViewAnimal();

            yield return TT.WaitForSeconds(0.1f);
            //TrAudio_SFX.xInstance.zzPlay_TempleBomb(0f);
        }
        for (int i = 0; i < _psBombEffect.Length; i++)
        {
            ParticleSystem.MainModule ps = _psBombEffect[i].main;
            ps.loop = false;
        }
        _isUsedItem = false;
    }

    public void zCheckAnswer(int num){
        if (!GameManager.xInstance._isGameStarted || _queueAnimals.Count == 0 || _isUsedItem) return;
        TrAnimal animal = _queueAnimals.Dequeue();
        if (_animalAnswerInfos[num == -1 ? 0 : 1]._numSp == 0) TrAudio_SFX.xInstance.zzPlay_AnimalsDog();
        bool isRight = num == 1 ? true : false;
        bool isCorrect = animal.zGetAnimalIsRight() == isRight;
        if (isCorrect){
            _valueCurrComboResetTime = _valueComboResetTime;
            _currCombo++;
            TrAudio_SFX.xInstance.zzPlay_AnimalsCombo(0.1f);
            if(animal._item == TrAnimalItems.BOMB){
                _isUsedItem = true;  
                StartCoroutine(yExecBombItem(animal));
                return;
            }
            else if (animal._item == TrAnimalItems.CLOCK){
                _currGameTime += _numPlusTime;
                TrUI_PuzzleAnimals.xInstance.zEffectPlusTime();
            }
        }
        else{
            TrAudio_SFX.xInstance.zzPlay_AnimalsWrong(0.1f);
            _currCombo = 0;
            if (_isOnVibration)
                Vibration.Vibrate(0.5f, 1, 1);

            zWrong(false);
            TrUI_PuzzleAnimals.xInstance.zSetMaskGauge(++_currSmokeGauge, _maxSmokeGauge);
            if(animal._item == TrAnimalItems.SMOKE){
                _isUsedItem = true;
                if(_coroutineSmoke != null){
                    StopCoroutine(_coroutineSmoke);
                }
                _coroutineSmoke = StartCoroutine(yExecSmokeItem(animal));
                return;
            }
        }
        
        _numStackMove++;
        animal.zIsCorrect(isCorrect, num);
        yCheckCombo();

        animal.transform.parent = _trAnimalsNullParent;
        animal.transform.localPosition = new Vector3(0, _posYBaseViewAnimals, 0);
        animal.transform.DOMoveX(_posXTargetAnimal * num, _speedAnimalMoveX).SetSpeedBased().OnComplete(()=>animal.gameObject.SetActive(false));

        yAddViewAnimal();
    }

    void yAddViewAnimal(){
        TrAnimal animal = _arrAnimals[_currIndex++];
        int rand = Random.Range(0, _numUseAnimals);
        TrAnimalAnswer answer = _animalAnswerInfos[rand];

        animal.transform.parent = _trAnimalsMoveParent;
        animal.transform.position = new Vector3(0, _posYInstantiateAnimals, 0);
        animal.gameObject.SetActive(true);
        if(GameManager.xInstance._isGameStarted)
            animal.transform.DOLocalMoveY(_posYCurrMaxViewAnimal, _valueTimeAnimalPosYReach);
        else
            animal.transform.DOLocalMoveY(_posYCurrMaxViewAnimal, _valueTimeAnimalPosYReach).OnComplete(()=>animal.zInitViewAnimalMoveComplete());

        if (_currBombGauge >= _maxBombGauge){
            _currBombGauge = 0;
            TrUI_PuzzleAnimals.xInstance.zSetBombGauge(0, _maxBombGauge);
            if (_isCurrBomb)
                animal.zSetAnimal(answer._numSp, answer._isRight, _currSort++, TrAnimalItems.BOMB);
            else
                animal.zSetAnimal(answer._numSp, answer._isRight, _currSort++, TrAnimalItems.CLOCK);
            _isCurrBomb = !_isCurrBomb;
        }
        else if(_currSmokeGauge >= _maxSmokeGauge){
            _currSmokeGauge = 0;
            TrUI_PuzzleAnimals.xInstance.zSetMaskGauge(0, _maxSmokeGauge);
            animal.zSetAnimal(answer._numSp, answer._isRight, _currSort++, TrAnimalItems.SMOKE);
        }else{
            animal.zSetAnimal(answer._numSp, answer._isRight, _currSort++);
        }
        _queueAnimals.Enqueue(animal);

        _posYCurrMaxViewAnimal += _posYDistanceViewAnimals;
        if (_currIndex >= _maxIndex) _currIndex = 0;
    }

    IEnumerator yInitViewAnimals(){
        float targetY = _posYBaseViewAnimals;
        _currIndex = 0;
        _posYCurrMaxViewAnimal = _posYBaseViewAnimals;
        for(int i=0; i<_numViewAnimals; i++){
            yAddViewAnimal();
            yield return TT.WaitForSeconds(_valueInitAnimalDropTerm);
        }
    }

    void ySetAnimalsInstantiate(){
        _arrAnimals = new TrAnimal[_maxIndex];
        for(int i=0; i< _maxIndex; i++){
            TrAnimal animal = Instantiate(_animalView);
            _arrAnimals[i] = animal;
            _arrAnimals[i].gameObject.SetActive(false);
        }
        _animalView.gameObject.SetActive(false);
    }

    void yCompleteYMove(){
        _numStackMove--;
        _isParentMove = false;
    }

    void ySelectAndSetRandomAnimals(){
        _animalAnswerInfos = new TrAnimalAnswer[_numUseAnimals];

        int numAnimalSps = _spAnimals.Length;
        int[] arrRand = new int[numAnimalSps];
        for(int i=0; i< numAnimalSps; i++){
            arrRand[i] = i;
        }

        int temp;
        for(int i=0; i< numAnimalSps; i++){
            int rand = Random.Range(0, _numUseAnimals);

            temp = arrRand[i];
            arrRand[i] = arrRand[rand];
            arrRand[rand] = temp; 
        }

        for (int i = 0; i < _numUseAnimals; i++)
        {
            TrAnimalAnswer trAnimalAnswer = new TrAnimalAnswer();
            _animalAnswerInfos[i] = trAnimalAnswer;
            _animalAnswerInfos[i]._numSp = arrRand[i];
            _animalAnswerInfos[i]._isRight = i%2 == 1;
            TrUI_PuzzleAnimals.xInstance.zSetButtonSp(i, arrRand[i]);
        }
    }

    void yWanderMove(){
        int randCharacter = Random.Range(0, _wanderCharacters.Length);
        int randIsRight = Random.Range(0, 2);
        randIsRight = randIsRight == 0 ? -1 : 1;

        Vector2[] vecs = new Vector2[4];
        for(int i=0; i<4; i++){
            float randX;
            if (i == 0 || i == 3)
                randX = _moveCharactersStartXPos;
            else
                randX = Random.Range(_moveCharactersMoveMinRange.x, _moveCharactersMoveMaxRange.x);
            randX *= randIsRight;
            float randY = Random.Range(_moveCharactersMoveMinRange.y, _moveCharactersMoveMaxRange.y);
            vecs[i] = new Vector2(randX, randY);
        }

        float randSpeed = Random.Range(_moveCharactersSpeedRange.x, _moveCharactersSpeedRange.y);

        _wanderCharacters[randCharacter].transform.position = vecs[0];
        _wanderCharacters[randCharacter].zSetMove(vecs, randSpeed);
        TrAudio_SFX.xInstance.zzPlay_AnimalsBee();
    }

    public void zSetWanderMove(){
        float randRespawnTime = Random.Range(_moveCharacterRespawnTime.x, _moveCharacterRespawnTime.y);
        TT.UtilDelayedFunc.zCreate(() => yWanderMove(), randRespawnTime);
    }

    IEnumerator yMoveFlowers(){
        int value = 5;
        while (true){
            _rectFlowers[0].DORotate(new Vector3(0, 0, value), 2f).SetSpeedBased();
            _rectFlowers[1].DORotate(new Vector3(0, 0, value), 2f).SetSpeedBased();
            yield return TT.WaitForSeconds(4f);
            value *= -1;
        }
    }

    protected override void yBeforeReadyGame(){
        base.yBeforeReadyGame();
        TrAudio_Music.xInstance.zzPlayMain(0.25f);
        _isThridChallengeSame = true;

        for (int i=0; i<_goSmokes.Length; i++){
            _goSmokes[i].transform.localScale = Vector2.zero;
            _goSmokes[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            _goSmokes[i].SetActive(true);
        }

        _isOnVibration = PlayerPrefs.GetInt(TT.strConfigVibrate, 1) == 1;
        _psComboBgEffect.Stop();
        ySetAnimalsInstantiate();
        ySelectAndSetRandomAnimals();
        StartCoroutine(yInitViewAnimals());
        if(_rectFlowers[0] != null)
            _coMoveFlower = StartCoroutine(yMoveFlowers());
        TrUI_PuzzleAnimals.xInstance.zSetPlusTime(_numPlusTime);

        Camera cam = Camera.main;
        _posXTargetAnimal = TrUI_PuzzleAnimals.xInstance._imgAnimalButtons[1].transform.position.x - ((cam.rect.x - (cam.rect.width - 1)) * 8);
    }

    protected override void yAfterReadyGame(){
        base.yAfterReadyGame();
        GameManager.xInstance._isGameStarted = true;
        zSetWanderMove();
    }

    protected override void zSetResultGame()
    {
        base.zSetResultGame();
        _psComboBgEffect.Stop();        
    }

    IEnumerator yExecTicTok(){
        while (GameManager.xInstance._isGameStarted){
            TrAudio_UI.xInstance.zzPlay_TimerTicTok(0.1f);
            yield return TT.WaitForSeconds(0.5f);
        }
    }

    private void OnDestroy()
    {
        if(_coMoveFlower != null)
            StopCoroutine(_coMoveFlower);
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

    protected override void Update()
    {
        base.Update();
        if (!GameManager.xInstance._isGameStarted) return;

        if (_numStackMove > 0){
            if (!_isParentMove){
                _isParentMove = true;
                float speed = _numStackMove * _valueSpeedAnimalDrop + 5;
                _trAnimalsMoveParent.DOLocalMoveY(_trAnimalsMoveParent.transform.position.y - _posYDistanceViewAnimals, speed).SetSpeedBased().OnComplete(() => yCompleteYMove());
            }
        }

        if(_currCombo > 0){
            _valueCurrComboResetTime -= Time.deltaTime;
            if (_valueCurrComboResetTime <= 0){
                if (_isOnVibration)
                    Vibration.Vibrate(0.5f, 1, 1);
                _currCombo = 0;
                yCheckCombo();
            }
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A)){
            //_btnLeft.zEffect();
            zCheckAnswer(-1);
        }
        if (Input.GetKeyDown(KeyCode.D)){
            //_btnRight.zEffect();
            zCheckAnswer(1);
        }
#endif

        if (_currGameTime <= _maxGameTime * 0.3f && !_isTimeTenseEffectExec){
            _isTimeTenseEffectExec = true;
            TrUI_PuzzleAnimals.xInstance.zSetTimerTenseEffect();
            StartCoroutine(yExecTicTok());
        }
    }
}