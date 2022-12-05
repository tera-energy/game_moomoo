using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrAnimal : MonoBehaviour
{
    const string IDLE1 = "Idle1", IDLE2 = "Idle2", ISANGRY = "IsAngry", ISFEAR = "IsFear", ISHAPPINESS = "IsHappiness";
    Animator _anim;
    SpriteRenderer _sr;
    [SerializeField] ParticleSystem _psBomb;
    [SerializeField] ParticleSystem _psExplode;
    
    [SerializeField] ParticleSystemRenderer[] _psrBombs;
    public TrAnimalItems _item;
    // 0 : Angry, 1 : Default, 2 : Fear, 3 : Happiness, 4 : GoodItem, 5 : BadItem, 6 : Clock
    [SerializeField] Sprite[] _spAnimals;
    [SerializeField] int _numSps;
    Sprite[] _spCurrAnimal;
    TrAnimalStat _stat;
    enum TrAnimalStat
    {
        Idle,
        MoveHori,
        MoveVerti
    }

    bool _isRight;

    float _currAnimChangeTime;

    public void zExplode(){
        _psExplode.Play();
        
    }

    public void zInitViewAnimalMoveComplete(){
        ySetBoolAnim();
    }

    public void zSetAnimal(int numSp, bool isRight, int sort, TrAnimalItems item=0){
        numSp *= _numSps;
        _isRight = isRight;
        _spCurrAnimal = new Sprite[_numSps];
        _item = item;
        int index = 0;
        for(int i=numSp; i < numSp + _numSps; i++){
            _spCurrAnimal[index++] = _spAnimals[i];
        }

        _sr.sortingOrder = sort;
        if (_item == 0){
            _sr.sprite = _spCurrAnimal[1];
        }
        else if (_item == (TrAnimalItems)1){
            _sr.sprite = _spCurrAnimal[4];
        }
        else if (_item == (TrAnimalItems)2){
            _sr.sprite = _spCurrAnimal[5];
            _psBomb.Play();
            _psrBombs[0].sortingOrder = sort;
            _psrBombs[1].sortingOrder = sort;
        }else if (_item == (TrAnimalItems)3){
            _sr.sprite = _spCurrAnimal[6];
        }
    }

    public bool zGetAnimalIsRight() => _isRight;

    public void zIsCorrect(bool isCorrect, int num){
        _stat = TrAnimalStat.MoveHori;
        if (isCorrect){
/*            _psMove.transform.eulerAngles = new Vector3(180, num * 90, 0);
            _psMove.Play();*/
            ySetBoolAnim(ISHAPPINESS);
            ySetSprite(3);
        }
        else{
            //_psMove.Pause();
            ySetBoolAnim(ISANGRY);
            ySetSprite(0);
        }
    }

    public void zDropDown(bool isDrop){
        if (!gameObject.activeSelf || _stat == TrAnimalStat.MoveHori) return;
        ySetBoolAnim(ISFEAR, isDrop);
        if (isDrop){
            ySetSprite(2);
            _stat = TrAnimalStat.MoveVerti;
        }
        else{
            ySetSprite(1);
            _stat = TrAnimalStat.Idle;
        }
    }

    void yRandomSetIdleAnimation(){
        int randAnim = Random.Range(0, 3);
        if (randAnim == 1){
            ySetBoolAnim();
            _anim.SetTrigger(IDLE1);
        }
        else if (randAnim == 2){
            ySetBoolAnim();
            _anim.SetTrigger(IDLE2);
        }
    }

    void ySetBoolAnim(string name="", bool isTrue=true){
        _anim.SetBool(ISANGRY, false);
        _anim.SetBool(ISFEAR, false);
        _anim.SetBool(ISHAPPINESS, false);

        if (name != "")
            _anim.SetBool(name, isTrue);
    }
    void ySetSprite(int num){
        if (_item == 0)
            _sr.sprite = _spCurrAnimal[num];
    }

    void Awake(){
        _anim = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
        _currAnimChangeTime = Random.Range(1, 10);
    }

    void OnEnable(){
        ySetBoolAnim();
        _stat = TrAnimalStat.Idle;
        _sr.color = Color.white;
        _psBomb.Stop();
    }

    private void Start(){
        //ySetBoolAnim(ISFEAR);
    }

    void FixedUpdate()
    {
        if (!GameManager.xInstance._isGameStarted) return;
        
        if(_stat == TrAnimalStat.Idle){
            _currAnimChangeTime -= Time.fixedDeltaTime;
            if(_currAnimChangeTime <= 0){
                _currAnimChangeTime = Random.Range(1f, 10f);
                yRandomSetIdleAnimation();
            }
        }
    }
}
