using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrMultiProfileType
{
    moomoo,
    enna,
    lex,
    sonson,
    peepee
}
public class TrProfileDatas : MonoBehaviour
{
    static TrProfileDatas _instance;
    public static TrProfileDatas xInstance { get { return _instance; } }

    public TrSO_MultiProfile[] _profiles;
    [HideInInspector] public static byte _numProfiles;

    public TrSO_MultiProfile zGetProfileData(byte imgType){
        TrSO_MultiProfile soProfile = null;
        for (int i = 0; i < _numProfiles; i++)
        {
            TrSO_MultiProfile profile = _profiles[i];
            if ((byte)profile._type == imgType)
            {
                soProfile = profile;
                break;
            }
        }
        return soProfile;
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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        _numProfiles = (byte)_profiles.Length;
    }
}
