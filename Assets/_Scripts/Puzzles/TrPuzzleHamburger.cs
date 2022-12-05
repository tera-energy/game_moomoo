using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
class TrHamburgerIngredients
{
    public GameObject _goHamburgerIngredients;
    public int _ingredientsNum;
    public float _ingredintsPosY = 0.18f;
}

[System.Serializable]
class TrViewCoke
{
    public GameObject _goViewCoke;
    public int _viewCokeNum;
}

[System.Serializable]
class TrInputCoke
{
    public GameObject _goInputCoke;
    public int _inputCokeNum;
}

public class TrPuzzleHamburger : TrPuzzleManager
{

    const string ISSAD = "IsSad";
    Animator _anim;
    
    SpriteRenderer _ingredientsSp;
    static TrPuzzleHamburger _instance;
    public static TrPuzzleHamburger xInstance { get { return _instance; } }


    [SerializeField] TrHamburgerIngredients[] _ingredientsInfo;
    [SerializeField] TrViewCoke[] _viewCokeInfo;
    [SerializeField] TrInputCoke[] _inputCokeInfo;

    [SerializeField] GameObject _cup;

    [SerializeField] GameObject[] _moomoo;

    [SerializeField] ParticleSystem _happyPar;

    [SerializeField] float _posYBaseCoke;
    [SerializeField] float _posYLimitCoke;
    [SerializeField] float _speedFillCoke;
    public bool _isDownCokeBtn;
    [SerializeField] Transform _trCokeViewMask;
    [SerializeField] Transform _trCokeSubmitMask;

    float _resetCokePositionY; 
    float _resetBurgerPositionY = -1f;

    float _inputIngredientsPosY = -1f;
    int _numCurrSort;

    float _viewIngredientsPosY = -1f;
    float _viewCokePosY;

    float _cokePosY = 0.5f;
    float _inputCokePosY;

    int _num = 8; // 예시 버거의 재료 수 난이도 조절
    int _cokeL = 5;
    int coke = 0;

    Stack<TrHamburgerIngredients> _viewStack = new Stack<TrHamburgerIngredients>();
    Stack<TrHamburgerIngredients> _inputStack = new Stack<TrHamburgerIngredients>();

    Stack<TrViewCoke> _viewCokeStack = new Stack<TrViewCoke>();
    Stack<TrInputCoke> _inputCokeStack = new Stack<TrInputCoke>();

    void yFillCoke(){
        if (_isDownCokeBtn){
            //float posY = _trCokeSubmitMask.position.y;
            //posY += Time.fixedDeltaTime * _speedFillCoke;
            _trCokeSubmitMask.position += Vector3.up * Time.fixedDeltaTime * _speedFillCoke;

            if (_trCokeSubmitMask.localPosition.y > _posYLimitCoke)
            {
                yWrong();
            }
        }
    }
   
    //무무 표정변화 코루틴
    IEnumerator yMooMoo(bool _isCheck)
    {
        if (_isCheck == true)
        {
            _moomoo[1].SetActive(true);
            _happyPar.gameObject.SetActive(true);
            _happyPar.Play();
            //TrAudio_SFX.xInstance.zzPlay_BurgerCorrect(0.1f);
        }
        else if (_isCheck == false)
        {
            _moomoo[2].SetActive(true);
            //TrAudio_SFX.xInstance.zzPlay_BurgerWrong(0.1f);
        }        

        yield return new WaitForSeconds(0.5f);
        _moomoo[1].SetActive(false);
        _moomoo[2].SetActive(false);
        
    }

    //햄버거 재료 생성 버튼
    public void zInputButton(int num){
        if (!GameManager.xInstance._isGameStarted) return;
        //TrAudio_SFX.xInstance.zzPlay_InputIngredients(0.2f);

        if (_inputStack.Count < 12f)
        {
            TrHamburgerIngredients InputBurger = new TrHamburgerIngredients();
            //재료생성
            GameObject _inputIngredients = Instantiate(_ingredientsInfo[num]._goHamburgerIngredients);                      
            _inputIngredients.transform.position = new Vector3(-1.5f, 8f, 0);
            //StartCoroutine(yInputIngredientsPosY(_inputIngredients));
            _inputIngredients.transform.DOLocalMoveY(_inputIngredientsPosY, 0.1f);
            InputBurger._ingredientsNum = _ingredientsInfo[num]._ingredientsNum;
            InputBurger._goHamburgerIngredients = _inputIngredients;
            _inputIngredientsPosY += InputBurger._ingredintsPosY;
            _inputStack.Push(InputBurger);
            _inputIngredients.transform.GetComponent<SpriteRenderer>().sortingOrder = _numCurrSort++;
        }
        else{
            yWrong();
        }
    }
    

    //버거가 내려오는 코루틴
/*    IEnumerator yInputIngredientsPosY(GameObject _Ingredients)
    {
        yield return new WaitForSeconds(0.1f);
        _Ingredients.transform.DOLocalMoveY(_inputIngredientsPosY, 0.1f);

        _inputIngredientsPosY += _ingredientsPosY;
    }*/

    void yWrong(){
        //TrAudio_UI.xInstance.zzPlay_Wrong();
        zWrong();
        StartCoroutine(yMooMoo(false));
        yResetGame();
    }

    //콜라 생성 버튼
    public void zInputCoke()
    {
        if (!GameManager.xInstance._isGameStarted) return;
        
        GameObject _inputCoke;
        //TrAudio_SFX.xInstance.zzPlay_Coke();

        if (coke < 4)
        {
            TrInputCoke InputCoke = new TrInputCoke();
            _inputCoke = Instantiate(_inputCokeInfo[coke]._goInputCoke);
            _inputCoke.transform.position = new Vector3(1.65f, _inputCokePosY, 0);
            InputCoke._inputCokeNum = _inputCokeInfo[coke]._inputCokeNum;
            InputCoke._goInputCoke = _inputCoke;

            
            _inputCokeStack.Push(InputCoke);
            Debug.Log(_inputCokeStack.Count);
            coke++;
        }
        else //콜라가 넘쳤을 경우 오답처리
        {
            Debug.Log("콜라 넘침");
            yWrong();
        }
    }

    void ySetIngredient(int numIngre){
        GameObject _viewIngredients;
        TrHamburgerIngredients viewBurger = new TrHamburgerIngredients();

        _viewIngredients = Instantiate(_ingredientsInfo[numIngre]._goHamburgerIngredients);
        _viewIngredients.transform.position = new Vector3(-6.5f, _viewIngredientsPosY, 0);
        viewBurger._goHamburgerIngredients = _viewIngredients;
        viewBurger._ingredientsNum = _ingredientsInfo[numIngre]._ingredientsNum;
        _viewIngredientsPosY += viewBurger._ingredintsPosY;

        _viewStack.Push(viewBurger);
        _viewIngredients.GetComponent<SpriteRenderer>().sortingOrder = _numCurrSort++;
    }

    //예시 버거 생성
    public void zSetViewBurger()
    {
        int randNum = Random.Range(2, _num);
        ySetIngredient(0);
        for (int i = 0; i <= randNum; i++){
            int rand = Random.Range(2, _ingredientsInfo.Length);
            ySetIngredient(rand);
        }
        ySetIngredient(1);
    }

    void ySetViewCoke(){
        int rand = Random.Range((int)_posYBaseCoke, (int)_posYLimitCoke-1);
        _trCokeViewMask.localPosition = new Vector3(0, rand, 0);
    }

    public void zViewCoke()
    {
        int randL = Random.Range(0, _cokeL);
        if (randL < 4)
        {
            GameObject _viewCokeL;
            TrViewCoke viewCoke = new TrViewCoke();
            _viewCokeL = Instantiate(_viewCokeInfo[randL]._goViewCoke);
            _viewCokeL.transform.position = new Vector3(1.65f, _viewCokePosY, 0);

            viewCoke._goViewCoke = _viewCokeL;
            viewCoke._viewCokeNum = _viewCokeInfo[randL]._viewCokeNum;
            for (int i = 0; i <= randL ;i++)
            {
                _viewCokeStack.Push(viewCoke);
                
            }           
        }   
    }
    public void zBack()
    {
        if (!GameManager.xInstance._isGameStarted) return;

        if (_inputStack.Count == 0){
            Debug.Log("뺄거 없음");
        }
        else{
            TrHamburgerIngredients ing = _inputStack.Pop();
            ing._goHamburgerIngredients.SetActive(false);
            _inputIngredientsPosY -= ing._ingredintsPosY;
        }
    }

    public bool yCokeCheck(){
        /*if (_viewCokeStack.Count > 0){
            if (_inputCokeStack.Count == 0)
            {
                Debug.Log("콜라 왜 안줌");
                return false;
            }
            TrInputCoke inputCokeStack = _inputCokeStack.Pop();
            TrViewCoke viewCokeStack = _viewCokeStack.Pop();

            viewCokeStack._goViewCoke.SetActive(false);
            inputCokeStack._goInputCoke.SetActive(false);

            if (_viewCokeStack.Count <= _inputCokeStack.Count)
            {
                return true;
            }
            else if (_viewCokeStack.Count > _inputCokeStack.Count)
            {
                Debug.Log("콜라 부족");
                return false;
            }
        }
        else if (_viewCokeStack.Count == 0)
        {
            if (_inputCokeStack.Count == 0)
            {
                Debug.Log("콜라 필요없음");
                return true;
            } else if (_inputCokeStack.Count > 0)
            {
                Debug.Log("콜라 안줘도 됨");
                return false;
            }
        }
        return true;*/
        return _trCokeSubmitMask.localPosition.y >= _trCokeViewMask.localPosition.y;
    }

    bool yCheckHamburger(){
        int numViewCokeStack = _viewCokeStack.Count;
        int numViewStack = _viewStack.Count;
        int numInputStack = _inputStack.Count;
        if (numInputStack != numViewStack){
            Debug.Log("햄버거 재료 수가 다름");
            return false;
        }
        else{
            while (true){
                if (_viewStack.Count == 0 || _inputStack.Count == 0)
                    break;
                TrHamburgerIngredients viewStack = _viewStack.Pop();
                TrHamburgerIngredients inputStack = _inputStack.Pop();
                viewStack._goHamburgerIngredients.SetActive(false);
                inputStack._goHamburgerIngredients.SetActive(false);
                if (viewStack._ingredientsNum != inputStack._ingredientsNum){
                    Debug.Log("재료가 다름");
                    return false;
                }
            }
        }
        return true;
    }

    public void zAnswerCheck()
    {
        if (!GameManager.xInstance._isGameStarted) return;
        
        int numViewStack = _viewStack.Count;

        if (yCokeCheck() && yCheckHamburger()) {
            int cokeScore = (int)(_trCokeViewMask.localPosition.y - _posYBaseCoke) / 4 + 1;
            Debug.Log("콜라 점수 : " + cokeScore);
            int score = numViewStack + cokeScore;
            Debug.Log("정답, 점수는 : " + score);
            zCorrect(true, score);
            StartCoroutine(yMooMoo(true));
            yResetGame();
        } else
            yWrong();
    }

    protected override void yBeforeReadyGame()
    {
        base.yBeforeReadyGame();
        TrAudio_Music.xInstance.zzPlayMain(0.25f);
        _isThridChallengeSame = true;
        zSetViewBurger();
        //zViewCoke();
        ySetViewCoke();
    }

    protected override void yAfterReadyGame()
    {
        base.yAfterReadyGame();        
        GameManager.xInstance._isGameStarted = true;
        
    }
    void yResetGame()
    {
        _inputIngredientsPosY = _resetBurgerPositionY;
        _viewIngredientsPosY = _resetBurgerPositionY;
        _inputCokePosY = _resetCokePositionY;
        _viewCokePosY = _resetCokePositionY;
        _trCokeSubmitMask.transform.localPosition = new Vector3(0, _posYBaseCoke, 0);
        _numCurrSort = 0;

        while (_viewStack.Count > 0){
            _viewStack.Pop()._goHamburgerIngredients.SetActive(false);
        }
        while (_inputStack.Count > 0)
        {
            _inputStack.Pop()._goHamburgerIngredients.SetActive(false);
        }
        while (_viewCokeStack.Count > 0)
        {
            _viewCokeStack.Pop()._goViewCoke.SetActive(false);
        }
        while(_inputCokeStack.Count > 0)
        {
            _inputCokeStack.Pop()._goInputCoke.SetActive(false);
        }

        coke = 0;

        _viewStack.Clear();
        _inputStack.Clear();        
        _inputCokeStack.Clear();
        _viewCokeStack.Clear();

        zSetViewBurger();
        //zViewCoke();
        ySetViewCoke();
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

    void FixedUpdate()
    {
        yFillCoke();
    }
}