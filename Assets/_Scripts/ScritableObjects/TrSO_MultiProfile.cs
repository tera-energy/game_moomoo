using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "profile", menuName = "New SO Multi Profile")]
public class TrSO_MultiProfile : ScriptableObject
{
    public TrMultiProfileType _type;
    
    public Sprite _spCharacter;
    public Sprite _spProfile;
    public Sprite _spResultCharacter;
    
    public string _name;

    public Sprite _spGoldMedal;
    public Sprite _spSilverMedal;
    public Sprite _spBrownMedal;

    public Vector3 _posMedal;

    public Sprite _spBgActivate;
    public Color _colNameActivate;

    public Sprite _spBgDisable;
    public Color _colNameDisable;
}
