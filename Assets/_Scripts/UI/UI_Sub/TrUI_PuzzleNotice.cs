using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrUI_PuzzleNotice : MonoBehaviour
{
    static TrUI_PuzzleNotice _instance;
    public static TrUI_PuzzleNotice xInstance { get { return _instance; } }

    [SerializeField] TextMeshProUGUI _txtNotice, _txtMoomoo;
    [SerializeField] GameObject _goMoomoo;

    Coroutine _textCoroutine;

    IEnumerator ySetText(string txt, float time)
    {
        _txtNotice.text = txt;

        yield return TT.WaitForSeconds(time);
        if (_txtNotice.text == txt)
        {
            _txtNotice.text = "";
            _goMoomoo.SetActive(false);
        }
    }

    public void zSetNotice(string content, float time=0.5f)
    {
        _txtNotice.text = content;

        if (_textCoroutine != null)
            StopCoroutine(_textCoroutine);
        _textCoroutine = StartCoroutine(ySetText(content, time));
    }

    public void zSetNoticeWithMoomoo(string content, int fontSize, float time)
    {
        _txtMoomoo.text = content;
        _txtMoomoo.fontSize = fontSize;
        _goMoomoo.SetActive(true);
        TT.UtilDelayedFunc.zCreate(() => _goMoomoo.SetActive(false), time);
    }

    void Awake(){
        if (_instance == null){
            _instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }
}
