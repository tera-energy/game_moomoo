using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrChangeCharacter : MonoBehaviour
{
    static TrChangeCharacter _instance;
    public static TrChangeCharacter xInstance { get { return _instance; } }
    TrMultiLobbyCharSelectComponenets[] _selects;
    [SerializeField] GameObject _goPreSelect;

    [SerializeField] Transform _trParent;

    byte _indexCurrSelect = 0;
    [HideInInspector] public byte _indexOriginSelect = 0;

    public void xOK(){
        _indexOriginSelect = _indexCurrSelect;

        TrMultiLobbyManager.xInstance.zChangeProfile(_indexOriginSelect);
    }

    public void xCancel(){
        zChangeSelect(_indexOriginSelect);
    }

    public void zChangeSelect(byte index){
        if (_indexCurrSelect != index)
            TrAudio_UI.xInstance.zzPlay_ClickButtonSmall();

        _selects[_indexCurrSelect].zSelectDisable();

        _indexCurrSelect = index;
        _selects[_indexCurrSelect].zSelectActivate();
    }

    public void zInit(byte index){
        _indexOriginSelect = index;
        _indexCurrSelect = _indexOriginSelect;

        _selects = new TrMultiLobbyCharSelectComponenets[TrProfileDatas._numProfiles];
        for (byte i = 0; i < TrProfileDatas._numProfiles; i++)
        {
            TrSO_MultiProfile data = TrProfileDatas.xInstance._profiles[i];

            TrMultiLobbyCharSelectComponenets sel = Instantiate(_goPreSelect, _trParent).GetComponent<TrMultiLobbyCharSelectComponenets>();
            sel._profileInfo = data;
            sel._imgCharacter.sprite = data._spCharacter;
            sel._imgCharacter.SetNativeSize();
            sel._txtName.text = string.Format("{0}{1}", '#', data._name);
            byte currIndex = i;
            sel._btnSelect.onClick.AddListener(() => zChangeSelect(currIndex));
            sel._spBgActivate = data._spBgActivate;
            sel._spBgDisable = data._spBgDisable;
            sel._colNameActivate = data._colNameActivate;
            sel._colNameDisable = data._colNameDisable;
            if (i != index)
                sel.zSelectDisable();
            else
                sel.zSelectActivate();
            _selects[i] = sel;
        }

    }
    
    private void Awake(){
        if (_instance == null){
            _instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        for (byte i = 0; i < TrProfileDatas._numProfiles; i++)
        {
            _selects[i]._btnSelect.onClick.RemoveListener(() => zChangeSelect(i));
        }
    }
}
