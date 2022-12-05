using UnityEngine;
using System;
public class TrProjectSettings : MonoBehaviour
{
    public static string _character = "moomoo";
	public static string _apiVersion = "_V22_10_12";

#if PLATFORM_IOS
    public static string _urlStore = "https://apps.apple.com/kr/app/moomooperplex/id1639990877";
#endif
#if PLATFORM_ANDROID
    public static string _urlStore = "https://play.google.com/store/apps/details?id=com.blazar.moomoo";
	public static string _googleUrl = "https://play.google.com/store/apps/details?id=com.blazar.moomoo";
	public static string _oneUrl = "https://m.onestore.co.kr/mobilepoc/apps/appsDetail.omp?prodId=0000764805";

#endif
    public static string _subjectForShare = "[Brainbow Arcade] 브레인보우 아케이드 무무펄플렉스";
    public static string _contentForShare = "[Brainbow Arcade] 브레인보우 아케이드 무무펄플렉스에 초대합니다!!!";

    public static string _urlRanking = "http://118.67.143.233:8080/brainbow/rank";

    public const string 
        strLOBBY = "LobbyPear",
		strMULTILOBBY = "MultiLobby",
        strPUZZLEPEAR = "PuzzlePearAnimals",
        strPUZZLEBASIC = "PuzzleAnimals",
        strPUZZLETEMPLE = "PuzzleTempleAnimals",
        strRESULT = "Result",
		strMULTIRESULT = "MultiResult";

	public const string
		FIREUSERID = "fireUserId",
		DEVICEID = "deviceId",
		PLATFORM = "platform",
		EMAIL = "email",
		NICKNAME = "nickName",
		MAXSCORE = "maxScore",
		SCORES = "scores",
		STAMINA = "stamina",
		STAMINADATE = "staminaDate",
		GUESTTOKEN = "guestToken",
		PLAYERID = "playerId",
		RECEIPT = "receipt",
		VERSION = "version",
		STATE = "state",
		ACTIVATE = "Activate",
		DEACTIVATE = "DeActivate",
		GOOGLE = "GooglePlay",
		APPLE = "AppStore",

		AUTOLOGINPLATFORM = "AutoLoginPlatform",
		AUTOLOGINID = "AUTO LOGIN ID",

		PROFILETYPE = "profileType";

	public static string _deviceId = SystemInfo.deviceUniqueIdentifier;


	public enum enumGameConcept { Basic, Pear, Temple }

	public static enumGameConcept _concept;

#region IAP
    public static string _iapStaminaId0 = "moomoo_stamina_0";
	public static string _iapStaminaId1 = "moomoo_stamina_1";
	public static string _iapStaminaId2 = "moomoo_stamina_2";
	public static string _iapStaminaId3 = "moomoo_stamina_3";

	public static int _iapStaminaNum0 = 10;
	public static int _iapStaminaNum1 = 30;
	public static int _iapStaminaNum2 = 50;
	public static int _iapStaminaNum3 = 100;
#endregion

	#region Check GPS And Time
	public static class UserGPS
	{
		public static double _firLat;
		public static double _firLong;
		public static bool _gpsStart;
		public static LocationInfo _location;
		public static WaitForSeconds _second;
	}

	public const float
	latNaju = 35.028333f, //����
	longNaju = 126.793670f, //�浵
	lattemple = 35.045799f,
	longtemple = 126.711433f,
	latTest = 35.021470f,
	longTest = 126.800790f;

	public static (float, float) zGetGPS()
	{
		Input.location.Start();
		UserGPS._location = Input.location.lastData;
		float _firLat = UserGPS._location.latitude * 1.0f;
		float _firLong = UserGPS._location.longitude * 1.0f;

		return (_firLat, _firLong);
	}

	public static bool zIsNight()
	{
		int hour = DateTime.Now.Hour;
		//Debug.Log("���� �ð� : " + hour);
		return hour >= 18 || hour < 6;
	}

	public static bool zIsNearTemple(float latDif, float longDif)
	{
		return Mathf.Abs(latDif) < 0.01f && Mathf.Abs(longDif) < 0.01f;
	}

	public static bool zIsCurrInNaju(float latDif, float longDif)
	{
		return Mathf.Abs(latDif) < 0.1f && Mathf.Abs(longDif) < 0.1f;
	}

	#endregion

	public static enumGameConcept zGetPlayAnimalMap()
	{
		enumGameConcept concept = 0;
		if (Input.location.isEnabledByUser)
		{
			(float userLat, float userLong) = zGetGPS();
			/*Debug.Log("���� ���� : " + userLat);
			Debug.Log("���� �浵 : " + userLong);*/
			float NajuLatDif = userLat - latNaju;
			float NajuLongDif = userLong - longNaju;
			float TempleLatDif = userLat - lattemple;
			float TempleLongDif = userLong - longtemple;
			/*float TestLatDif = userLat - latTest;
			float TestLongDif = userLong - longTest;
			Debug.Log(string.Format("������? : {0}", zIsNight()));
			Debug.Log(string.Format("��������? {0}", zIsCurrInNaju(NajuLatDif, NajuLongDif)));
			Debug.Log(string.Format("�� �ֺ�����? : {0}", zIsNearTemple(TempleLatDif, TempleLongDif)));
			Debug.Log(string.Format("�ĸ�? : {0}", zIsTest(TestLatDif, TestLongDif))); //�׽�Ʈ*/

			//if (zIsNight() || zIsNearTemple(TempleLatDif, TempleLongDif) || zIsTest(TestLatDif, TestLongDif)){
			if (zIsNight() || zIsNearTemple(TempleLatDif, TempleLongDif))
			{
				concept = enumGameConcept.Temple;
			}
			else
			{
				if (zIsCurrInNaju(NajuLatDif, NajuLongDif))
				{
					concept = enumGameConcept.Pear;
				}
				else
				{
					int rand = UnityEngine.Random.Range(0, 3);
					switch (rand)
					{
						case 0:
							concept = enumGameConcept.Basic;
							break;
						case 1:
							concept = enumGameConcept.Pear;
							break;
						case 2:
							concept = enumGameConcept.Temple;
							break;
					}
				}
			}
			Input.location.Stop();
		}
		else
		{
			if (zIsNight())
			{
				concept = enumGameConcept.Temple;
			}
			else
			{
				int rand = UnityEngine.Random.Range(0, 3);
				switch (rand)
				{
					case 0:
						concept = enumGameConcept.Basic;
						break;
					case 1:
						concept = enumGameConcept.Pear;
						break;
					case 2:
						concept = enumGameConcept.Temple;
						break;
				}
			}
		}
		return concept;
	}
}
