using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TrUI_PuzzleHamburger : TrUI_PuzzleManager
{
    [SerializeField] ParticleSystem[] _btnParticles;

    bool _isPlay = true;

    static TrUI_PuzzleHamburger _instance;
    public static TrUI_PuzzleHamburger xInstance {  get { return _instance; } }

    void ySetParticle(int num)
    {
        ParticleSystem.EmissionModule _emission = _btnParticles[num].emission;
    }

    public void BtnInputIngredients(int num)
    {
        if (!GameManager.xInstance._isGameStarted) return;

        TrPuzzleHamburger.xInstance.zInputButton(num);
        _btnParticles[1].gameObject.SetActive(true);
        _btnParticles[1].Play();
        //TrAudio_SFX.xInstance.zzPlay_HamburgerInputBtn();
    }

    public void BtnAnswerCheck()
    {

        TrPuzzleHamburger.xInstance.zAnswerCheck();

            _btnParticles[0].gameObject.SetActive(true);
            _btnParticles[0].Play();

    }
    
    public void BtnBack()
    {
        if (!GameManager.xInstance._isGameStarted) return;
        //TrAudio_SFX.xInstance.zzPlay_BackBtn();
        TrPuzzleHamburger.xInstance.zBack();
    }

    public void BtnCoke()
    {
        TrPuzzleHamburger.xInstance.zInputCoke();
    }

    public void zDownCokeBtn(){
        TrPuzzleHamburger.xInstance._isDownCokeBtn = true;
    }

    public void zUpCokeBtn(){
        TrPuzzleHamburger.xInstance._isDownCokeBtn = false;
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
