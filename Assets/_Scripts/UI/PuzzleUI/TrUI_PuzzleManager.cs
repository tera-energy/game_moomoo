using System;
using UnityEngine;
using TMPro;

public class TrUI_PuzzleManager : MonoBehaviour
{
    [SerializeField] Transform _trPlayersParent;
    [SerializeField] GameObject _goPrePlayer;
    TrMultiInGamePlayersComponents[] _uiPlayers;

    [SerializeField] Sprite _spScoreMine;
    [SerializeField] Sprite _spScoreNotMine;

    byte _indexRank = 0;
    [SerializeField] GameObject[] _goTxtRanks;

    public void zSetEndUser(byte index){
        _uiPlayers[index]._txtNickname.color = Color.red;
        _uiPlayers[index]._txtScore.color = Color.red;
    }

    public void zSetUsersScore(byte index, short score){
        _uiPlayers[index]._txtScore.text = score.ToString();
        TrNetworkManager._playerScores[index] = score;

        int rank = 0;
        for(byte i = 0; i < TrNetworkManager._numMaxUser; i++){
            if(TrNetworkManager._playerScores[index] > TrNetworkManager._playerScores[i]){
                rank++;
            }
        }

        _uiPlayers[index].transform.SetSiblingIndex(TrNetworkManager._numMaxUser - 1 - rank);
    }

    public void zSetUserInfos(bool isMine, byte index, string nick, byte imgType){
        if (_uiPlayers == null)
        {
            _uiPlayers = new TrMultiInGamePlayersComponents[TrNetworkManager._numMaxUser];
        }

        _uiPlayers[index] = Instantiate(_goPrePlayer, _trPlayersParent).transform.GetComponent<TrMultiInGamePlayersComponents>();
        TrMultiInGamePlayersComponents player = _uiPlayers[index];
        player._txtNickname.text = nick;
        player._txtScore.text = "0";
        player._imgCharacter.sprite = TrProfileDatas.xInstance.zGetProfileData(imgType)._spProfile;
        player._imgCharacter.SetNativeSize();

        if (isMine)
            player._imgBg.sprite = _spScoreMine;
        else
            player._imgBg.sprite = _spScoreNotMine;
        
        _goTxtRanks[_indexRank++].SetActive(true);
    }

    private void Awake(){
        for(int i=0; i<_goTxtRanks.Length; i++)
        {
            _goTxtRanks[i].SetActive(false);
        }
    }
}
