using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.IO;
using UnityEngine.SceneManagement;

public class BaseMentUI_Manager : MonoBehaviour 
{
	public static BaseMentUI_Manager instance;

	public bool TextReady = false;

	//텍스트 키와 텍스트값을 저장하는 딕셔너리
	public Dictionary<string,string> LoadedText;

	//------------------------------------------
	//플레이어 안드로이드 데이터
	JsonData PlayerAndroidStatus;
	//총기 데이터
	JsonData GunData;
	//유닛이름을 저장하는 제이슨데이터
	JsonData UnitNameData;

	//------------------------------------------
	//총기 아이콘 매니저
	public WeaponIcon_Manager WIM;

	//------------------------------------------
	//유닛이름 텍스트
	public Text[] UnitName_txt = new Text[4];
	//코스트 텍스트 
	public Text[] Cost_txt = new Text[4];
	//유닛 장착 총기 아이콘
	public Image[] UnitAttachedGun_img = new Image[4];
	//------------------------------------------
	//안드로이드 이미지
	public Sprite[] AndroidSprites = new Sprite[7];
	//유닛 정보 안드로이드 이미지
	public Image[] UnitInfoAndroid_img = new Image[4];
	
	//------------------------------------------
	//현재 선택된 유닛 인덱스
	[SerializeField]
	int CurrentUnitIndex;

	//------------------------------------------
	//총기선택 패널 판단코스트 
	public Text[] GSP_JC_txt = new Text[6];
	//총기선택 패널 행동코스트
	public Text[] GSP_AC_txt = new Text[6];
	//총기선택 패널 노말코스트
	public Text[] GSP_NC_txt = new Text[6];
	//총기선택 패널 턴당발사수
	public Text[] GSP_RPT_txt = new Text[6];
	//총기선택 패널 탄환당데미지
	public Text[] GSP_DPB_txt = new Text[6];
	//총기선택 패널 속도감소
	public Text[] GSP_SD_txt = new Text[6];
	//불러올 총기 인덱스
	public int[] GunIndex = new int[6];

	//------------------------------------------
	//총기 선택 패널
	public GameObject GunSelectPanel;

	//------------------------------------------

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else if(instance != this)
		{
			Destroy(gameObject);
		}
		//라이브러리 할당
		LocalizedTextLoad();
		for(int i = 0 ; i < 4 ; i++)
		{
			//유닛 정보 할당
			UnitInfo_refrecher(i);
		}
		//총기선택 패널 텍스트 아웃
		GunSelectPanel_TextOut();
	}

	//LoadedText딕셔너리 초기화
	public void LocalizedTextLoad()
	{
		LoadedText = new Dictionary<string,string>();

		//LanguageCode에서 로드한 언어코드를 기반으로 제이슨 텍스트 파일 접근
		string filepath = File.ReadAllText
		(Application.dataPath + "/Data/Language/BaseMent_Lang/BMlang_"+LanguageCode.LCODE+".json");
		LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(filepath);

		for(int i = 0; i < loadedData.UIINFO.Length ; i++)
		{
			LoadedText.Add(loadedData.UIINFO[i].key,loadedData.UIINFO[i].value);
		}
		//플레이어 안드로이드 스테이터스 로드
		string playerandroidstatus = File.ReadAllText(Application.dataPath + "/Data/Status_save/PlayerAndroid_status.json");
		PlayerAndroidStatus = JsonMapper.ToObject(playerandroidstatus);
		//총기 스테이터스 로드
		string gundata = File.ReadAllText(Application.dataPath + "/Data/DataBase/Gun_Status/Gun_status.json");
		GunData = JsonMapper.ToObject(gundata);
		//유닛 이름 로드
		string androidname = File.ReadAllText(Application.dataPath + "/Data/Language/UnitName/UnitName_"+ LanguageCode.LCODE +".json");
		UnitNameData = JsonMapper.ToObject(androidname);

		TextReady = true;
	}

	//키를 입력받아 텍스트를 전달하는 메서드
	//입력 : 키 // 출력 : 키에 해당 하는 텍스트 (기본값 : MissingText)
	public string GetLocalizedValue(string key)
	{
		//텍스트 기본값 지정
		string OutText = "MissingText";
		//만약 대응하는 키가 존재할경우,
		if(LoadedText.ContainsKey(key))
		{
			OutText = LoadedText[key];
		}
		//텍스트 반환
		return OutText;
	}

	//유닛 정보 할당기
	void UnitInfo_refrecher(int UnitIndex)
	{
		//유닛 이름 할당
		UnitName_txt[UnitIndex].text = (string)UnitNameData[(int)PlayerAndroidStatus[UnitIndex]["code"]]["name"];
		//유닛 총기 이미지 할당
		UnitAttachedGun_img[UnitIndex].sprite = WIM.WeaponIcon[(int)PlayerAndroidStatus[UnitIndex]["GunIndex"]];
		//코스트 텍스트 할당
		Cost_txt[UnitIndex].text = "JC/AC/NC\n" + 
		PlayerAndroidStatus[UnitIndex]["JudgementCost"] + "/" + 
		PlayerAndroidStatus[UnitIndex]["BehaviorCost"] + "/" +
		PlayerAndroidStatus[UnitIndex]["NomalCost"];
		//안드로이드 이미지 할당
		UnitInfoAndroid_img[UnitIndex].sprite = AndroidSprites[(int)PlayerAndroidStatus[UnitIndex]["code"]];
	}

	//총기 선택 패널 택스트 아웃
	void GunSelectPanel_TextOut()
	{
		for(int i = 0 ; i < 6 ; i++)
		{
			GSP_JC_txt[i].text = LoadedText["judgementcost_txt"] + " : " + GunData[GunIndex[i]]["JudgementCost"];
			GSP_AC_txt[i].text = LoadedText["actingcost_txt"] + " : " + GunData[GunIndex[i]]["BehaviorCost"];
			GSP_NC_txt[i].text = LoadedText["normalcost_txt"] + " : " + GunData[GunIndex[i]]["NomalCost"];
			GSP_RPT_txt[i].text = LoadedText["roundperturn_txt"] + " : " + GunData[GunIndex[i]]["RPT"];
			GSP_DPB_txt[i].text = LoadedText["damageperbullet_txt"] + " : " + GunData[GunIndex[i]]["DPB"];
			GSP_SD_txt[i].text = LoadedText["speeddecrease_txt"] + " : " + GunData[GunIndex[i]]["SpeedDecrease"];
		}
	}

	//안드로이드 총기장착 데이터 처리 메서드
	public void AttachGunToAndroid(int WeaponIndex)
	{
		PlayerAndroidStatus[CurrentUnitIndex]["JudgementCost"] = GunData[WeaponIndex]["JudgementCost"];
		PlayerAndroidStatus[CurrentUnitIndex]["BehaviorCost"] = GunData[WeaponIndex]["BehaviorCost"];
		PlayerAndroidStatus[CurrentUnitIndex]["NomalCost"] = GunData[WeaponIndex]["NomalCost"];
		PlayerAndroidStatus[CurrentUnitIndex]["MaxBullet"] = GunData[WeaponIndex]["MaxBullet"];
		PlayerAndroidStatus[CurrentUnitIndex]["ReloadedBullets"] = GunData[WeaponIndex]["ReloadedBullets"];
		PlayerAndroidStatus[CurrentUnitIndex]["CounterShotDamage"] = GunData[WeaponIndex]["CounterShotDamage"];
		PlayerAndroidStatus[CurrentUnitIndex]["SurpriseShotDamage"] = GunData[WeaponIndex]["SurpriseShotDamage"];
		PlayerAndroidStatus[CurrentUnitIndex]["ResponseShotDamage"] = GunData[WeaponIndex]["ResponseShotDamage"];
		PlayerAndroidStatus[CurrentUnitIndex]["AttackRange"] = GunData[WeaponIndex]["AttackRange"];
		PlayerAndroidStatus[CurrentUnitIndex]["Speed"] = (int)PlayerAndroidStatus[CurrentUnitIndex]["Speed"] + (int)GunData[(int)PlayerAndroidStatus[CurrentUnitIndex]["GunIndex"]]["SpeedDecrease"];
		PlayerAndroidStatus[CurrentUnitIndex]["Speed"] = (int)PlayerAndroidStatus[CurrentUnitIndex]["Speed"] - (int)GunData[WeaponIndex]["SpeedDecrease"];
		PlayerAndroidStatus[CurrentUnitIndex]["RPT"] = GunData[WeaponIndex]["RPT"];
		PlayerAndroidStatus[CurrentUnitIndex]["DPB"] = GunData[WeaponIndex]["DPB"];
		PlayerAndroidStatus[CurrentUnitIndex]["Type"] = GunData[WeaponIndex]["Type"];
		PlayerAndroidStatus[CurrentUnitIndex]["GunIndex"] = WeaponIndex;
		PlayerAndroidStatus[CurrentUnitIndex]["ActualLoadedBullet"] = GunData[WeaponIndex]["ReloadedBullets"];
		PlayerAndroidStatus[CurrentUnitIndex]["CurrentBullet"] = GunData[WeaponIndex]["MaxBullet"];
		//AndroidStatusSave();
		//유닛 스탯 텍스트 초기화
		UnitInfo_refrecher(CurrentUnitIndex);
	}

	void AndroidStatusSave()
	{
		JsonData androidstat = JsonMapper.ToJson(PlayerAndroidStatus);
		File.WriteAllText(Application.dataPath +"/Data/Status_save/PlayerAndroid_status.json",androidstat.ToString());
	}

	//안드로이드 총기 선택 버튼 클릭시
	public void AndroidGunBtn_pressed(int AndroidIndex)
	{
		//총기 선택창 활성화
		GunSelectPanel.SetActive(true);
		//안드로이드 인덱스 할당
		CurrentUnitIndex = AndroidIndex;
	}
	//총기 선택 창에서 총기 버튼 클릭시
	public void GunSelectBtn_pressed(int GunIndex)
	{
		//총기 선택창 비활성화
		GunSelectPanel.SetActive(false);
		//총기 데이터 안드로이드에 할당
		AttachGunToAndroid(GunIndex);
	}
	//총기 선택 창에서 뒤로가기 버튼 클릭시
	public void GunSelectBackBtn_pressed()
	{
		//총기 선택창 비활성화
		GunSelectPanel.SetActive(false);
	}
	//월드 플레이트 버튼 클릭시
	public void ToWorldPlateBtn_pressed()
	{
		JsonData AndroidStat = JsonMapper.ToJson(PlayerAndroidStatus);
		File.WriteAllText(Application.dataPath +"/Data/Status_save/PlayerAndroid_status.json",AndroidStat.ToString());
		//로딩 인덱스 변경
		LoadingIndex.LoadSceneIndex = 3;
		//로딩씬 로드
		SceneManager.LoadScene("LoadingScene");
	}
	//저장 버튼 클릭시  
	public void SaveBtn_pressed()
	{
		JsonData AndroidStat = JsonMapper.ToJson(PlayerAndroidStatus);
		File.WriteAllText(Application.dataPath +"/Data/Status_save/PlayerAndroid_status.json",AndroidStat.ToString());
	}
}