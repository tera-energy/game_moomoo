using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrMultiLobbyCharSelectComponenets : MonoBehaviour
{
    [HideInInspector] public TrSO_MultiProfile _profileInfo;
    
    public Image _imgBg;
    public Button _btnSelect;

    public TextMeshProUGUI _txtName;
    public Image _imgCharacter;

    [HideInInspector] public Sprite _spBgActivate;
    [HideInInspector] public Color _colNameActivate;

    [HideInInspector] public Sprite _spBgDisable;
    [HideInInspector] public Color _colNameDisable;

    public void zSelectActivate()
    {
        _imgBg.sprite = _spBgActivate;
        _txtName.color = _colNameActivate;
    }

    public void zSelectDisable()
    {
        _imgBg.sprite = _spBgDisable;
        _txtName.color = _colNameDisable;
    }
}
