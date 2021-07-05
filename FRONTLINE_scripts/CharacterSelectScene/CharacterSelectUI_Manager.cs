using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.IO;
using UnityEngine.SceneManagement;

public class CharacterSelectUI_Manager : MonoBehaviour 
{
	public static CharacterSelectUI_Manager instance;

	public bool TextReady = false;

	//텍스트 키와 텍스트값을 저장하는 딕셔너리
	public Dictionary<string,string> LoadedText;

	//-----------------------------------------------
	//플레이트 좌표 생성기
	public PlateCoords_Generator PCG;

	//-----------------------------------------------
	//선택된 아군 버튼
	public Button[] SelectedCharacter_btn = new Button[4];
	//선택된 아군 버튼 현재 인덱스
	[SerializeField]
	int Selectedbtn_currentIndex = -1;

	//-----------------------------------------------
	//선택가능한 아군 버튼
	public Button[] SelectableCharacter_btn = new Button[7];
	//선택가능한 아군 버튼 현재 인덱스
	[SerializeField]
	int Selectablebtn_currentIndex = -1;

	//-----------------------------------------------
	//아군 안드로이드 데이터
	JsonData AndroidStatus;
	//유닛이름을 저장하는 제이슨데이터
	JsonData UnitNameData;
	//총기 데이터
	JsonData GunData;

	//-----------------------------------------------
	//선택된 안드로이드 버튼 이미지 / 0번 : 비선택 / 1번 : 선택
	public Sprite[] SelectedBtn_img = new Sprite[2];

	//선택가능한 안드로이드 버튼 이미지 / 0번 : 비선택 / 1번 : 선택
	public Sprite[] SelectableBtn_img = new Sprite[2];

	//-----------------------------------------------
	//선택된 아군 안드로이드 인덱스
	public int[] SelectedIndex = new int[4] {-1,-1,-1,-1};
	//-----------------------------------------------
	//선택된 안드로이드 이미지
	public Image[] AndroidSelected_img = new Image[4];
	//선택 가능 안드로이즈 이미지
	public Image[] AndroidSelectable_img = new Image[7];
	//선택 가능 안드로이드 배경 이미지
	public Image[] AndroidSelectableBackground_img = new Image[7];

	//-----------------------------------------------
	//안드로이드 스탠드 이미지
	public Sprite[] AndroidStand_img = new Sprite[7];
	//안드로이드 자른 이미지
	public Sprite[] AndroidCuted_img = new Sprite[7];
	//안드로이드 자른 흑백 이미지
	public Sprite[] AndroidCutedBlack_img = new Sprite[7];
	//선택시 배경
	public Sprite SelectedBackground;
	//일반 배경
	public Sprite NormalBackground;

	//-----------------------------------------------
	//현재 안드로이즈 전신 이미지
	public GameObject CurrentStandImage;

	//-----------------------------------------------
	//유닛 스탯정보 슬라이더
	// 0번 : 장갑 / 1번 : 체력 / 2번 : 가시거리 / 3번 : 속도
	public Slider[] UnitStat_slider = new Slider[4];
	//유닛 이름 텍스트
	public Text UnitName_txt;
	//유닛 설명 텍스트
	public Text UnitExplain_txt;

	//-----------------------------------------------
	//확정버튼
	public GameObject Sure_btn;
	//유닛정보 관련 오브젝트들
	public GameObject[] UnitInfoGameobjects; 

	//-----------------------------------------------

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
		//언어팩 로드
		LocalizedTextLoad();
		//안드로이드 스테이터스 로드
		AndroidStatus_loader();
	}
	
	//언어팩 갱신 메서드
	public void LocalizedTextLoad()
	{
		LoadedText = new Dictionary<string,string>();

		//LanguageCode에서 로드한 언어코드를 기반으로 제이슨 텍스트 파일 접근
		string filepath = File.ReadAllText
		(Application.dataPath + "/Data/Language/CharacterSelect_Lang/CSlang_"+ LanguageCode.LCODE +".json");
		LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(filepath);

		for(int i = 0; i < loadedData.UIINFO.Length ; i++)
		{
			LoadedText.Add(loadedData.UIINFO[i].key,loadedData.UIINFO[i].value);
		}

		TextReady = true;
	}
	
	//안드로이드 스테이터스 로더
	void AndroidStatus_loader()
	{
		string androidstatus = File.ReadAllText(Application.dataPath + "/Data/DataBase/Android_Status/Android_status.json");
		AndroidStatus = JsonMapper.ToObject(androidstatus);

		string androidname = File.ReadAllText(Application.dataPath + "/Data/Language/UnitName/UnitName_"+ LanguageCode.LCODE +".json");
		UnitNameData = JsonMapper.ToObject(androidname);

		//총기 스테이터스 로드
		string gundata = File.ReadAllText(Application.dataPath + "/Data/DataBase/Gun_Status/Gun_status.json");
		GunData = JsonMapper.ToObject(gundata);
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

	//선택된 아군 버튼 클릭시
	public void SelectedCharacterBtn_pressed(int btn_num)
	{
		if(Selectedbtn_currentIndex != -1)
		{
			//이전 이미지 비선택으로 변경
			SelectedCharacter_btn[Selectedbtn_currentIndex].image.sprite = SelectedBtn_img[0];
		}
		//인덱스 할당
		Selectedbtn_currentIndex = btn_num;
		//현재 이미지 선택으로 변경
		SelectedCharacter_btn[Selectedbtn_currentIndex].image.sprite = SelectedBtn_img[1];
	}
	//선택 가능한 아군 버튼 클릭시
	public void SelectableCharacterBtn_pressed(int btn_num)
	{
		//현재 상태 선택 중일경우,
		if(Selectedbtn_currentIndex != -1)
		{
			//만약 이전에 이미 선택된 캐릭터가 있을경우,
			if(SelectedIndex[Selectedbtn_currentIndex] != -1)
			{
				//이전 이미지 비선택으로 변경
				//SelectableCharacter_btn[SelectedIndex[Selectedbtn_currentIndex]].image.sprite = SelectableBtn_img[0];
				//이전버튼 상호작용 가능
				SelectableCharacter_btn[SelectedIndex[Selectedbtn_currentIndex]].interactable = true;
				//이전 안드로이드 이미지 활성화
				AndroidSelectable_img[SelectedIndex[Selectedbtn_currentIndex]].sprite = AndroidCuted_img[SelectedIndex[Selectedbtn_currentIndex]];
				//이전 안드로이드 배경 이미지 활성화
				AndroidSelectableBackground_img[SelectedIndex[Selectedbtn_currentIndex]].sprite = NormalBackground;
			}
			//인덱스 할당
			Selectablebtn_currentIndex = btn_num;
			//선택된 안드로이드 인덱스 할당
			SelectedIndex[Selectedbtn_currentIndex] = Selectablebtn_currentIndex;
			//이전 안드로이드 이미지 비활성화
			AndroidSelectable_img[SelectedIndex[Selectedbtn_currentIndex]].sprite = AndroidCutedBlack_img[SelectedIndex[Selectedbtn_currentIndex]];
			//이전 안드로이드 배경 이미지 활성화
			AndroidSelectableBackground_img[SelectedIndex[Selectedbtn_currentIndex]].sprite = SelectedBackground;
			//버튼 상호작용 불가능
			SelectableCharacter_btn[SelectedIndex[Selectedbtn_currentIndex]].interactable = false;

			//선택된 버튼 이미지 변경
			AndroidSelected_img[Selectedbtn_currentIndex].gameObject.SetActive(true);
			AndroidSelected_img[Selectedbtn_currentIndex].sprite = AndroidCuted_img[Selectablebtn_currentIndex];

			//유닛 스탯 정보 로드
			UnitInfo_refresher(Selectablebtn_currentIndex);
			
			StartCoroutine(SureCheck());
		}
	}

	IEnumerator SureCheck()
	{
		//전부 선택 되었다면, 확정버튼 활성화
		bool ischeck = true;
		for(int i = 0 ;i < 4 ; i++)
		{
			if(SelectedIndex[i] == -1)
			{
				ischeck = false;
			}
		}
		yield return new WaitForSeconds(0.1f);
		if(ischeck)
		{
			Sure_btn.SetActive(true);
		}
	}

	//유닛 정보 UI로더
	void UnitInfo_refresher(int UnitIndex)
	{
		UnitInfoGameobjects[0].SetActive(true);
		UnitInfoGameobjects[1].SetActive(true);
		UnitInfoGameobjects[2].SetActive(true);

		//스탠드 이미지 활성화
		CurrentStandImage.GetComponent<Image>().sprite = AndroidStand_img[UnitIndex];

		//스탯 슬라이더 수치 할당
		UnitStat_slider[0].value = (int)AndroidStatus[UnitIndex]["Armor"];
		UnitStat_slider[1].value = (int)AndroidStatus[UnitIndex]["Health"];
		UnitStat_slider[2].value = (int)AndroidStatus[UnitIndex]["VisibleRange"];
		UnitStat_slider[3].value = (int)AndroidStatus[UnitIndex]["Speed"];
		//유닛 이름 텍스트 할당
		UnitName_txt.text = (string)UnitNameData[UnitIndex]["name"];
		//유닛 설명 텍스트 할당
		UnitExplain_txt.text = LoadedText["unitexplain"+UnitIndex+"_txt"];
	}

	//확정 버튼 클릭시
	//씬 다음으로 넘기기 , 맵파일 생성 , 플레이어 안드로이드 스테이터스 생성
	public void SureBtn_pressed()
	{
		StartCoroutine(SureBtnCorutine());
	}

	IEnumerator SureBtnCorutine()
	{
		//플레이트 좌표 생성
		PCG.WorldPlateCoord_GeneratorActual();

		//안드로이드 스탯 세이브 기본형 불러오기
		string playerandroid_statusempty = File.ReadAllText(Application.dataPath + "/Data/Status_save/PlayerAndroid_status_Empty.json");
		JsonData PlayerAndroid_status = JsonMapper.ToObject(playerandroid_statusempty);

		//데이터 복사 할당
		for(int i = 0 ; i < 4 ; i++)
		{
			//일반 스탯 할당
			PlayerAndroid_status[i]["Name"] = AndroidStatus[SelectedIndex[i]]["Name"];
			PlayerAndroid_status[i]["class"] = AndroidStatus[SelectedIndex[i]]["class"];
			PlayerAndroid_status[i]["code"] = AndroidStatus[SelectedIndex[i]]["code"];
			PlayerAndroid_status[i]["Speed"] = AndroidStatus[SelectedIndex[i]]["Speed"];
			PlayerAndroid_status[i]["Armor"] = AndroidStatus[SelectedIndex[i]]["Armor"];
			PlayerAndroid_status[i]["Health"] = AndroidStatus[SelectedIndex[i]]["Health"];
			PlayerAndroid_status[i]["MaxHealth"] = AndroidStatus[SelectedIndex[i]]["Health"];
			PlayerAndroid_status[i]["Scanradius"] = AndroidStatus[SelectedIndex[i]]["Scanradius"];
			PlayerAndroid_status[i]["IdentificationDistance"] = AndroidStatus[SelectedIndex[i]]["IdentificationDistance"];
			PlayerAndroid_status[i]["VisibleRange"] = AndroidStatus[SelectedIndex[i]]["VisibleRange"];

			//총기 스탯 할당
			PlayerAndroid_status[i]["JudgementCost"] = GunData[0]["JudgementCost"];
			PlayerAndroid_status[i]["BehaviorCost"] = GunData[0]["BehaviorCost"];
			PlayerAndroid_status[i]["NomalCost"] = GunData[0]["NomalCost"];
			PlayerAndroid_status[i]["MaxBullet"] = GunData[0]["MaxBullet"];
			PlayerAndroid_status[i]["ReloadedBullets"] = GunData[0]["ReloadedBullets"];
			PlayerAndroid_status[i]["CounterShotDamage"] = GunData[0]["CounterShotDamage"];
			PlayerAndroid_status[i]["SurpriseShotDamage"] = GunData[0]["SurpriseShotDamage"];
			PlayerAndroid_status[i]["ResponseShotDamage"] = GunData[0]["ResponseShotDamage"];
			PlayerAndroid_status[i]["AttackRange"] = GunData[0]["AttackRange"];
			PlayerAndroid_status[i]["Speed"] = (int)PlayerAndroid_status[i]["Speed"] - (int)GunData[0]["SpeedDecrease"];
			PlayerAndroid_status[i]["RPT"] = GunData[0]["RPT"];
			PlayerAndroid_status[i]["DPB"] = GunData[0]["DPB"];
			PlayerAndroid_status[i]["Type"] = GunData[0]["Type"];
			PlayerAndroid_status[i]["Accuracy"] = 100;
			PlayerAndroid_status[i]["ActualLoadedBullet"] = GunData[0]["ReloadedBullets"];
			PlayerAndroid_status[i]["CurrentBullet"] = GunData[0]["MaxBullet"];
		}

		//안드로이드 스탯 세이브 저장
		JsonData PlayerAndroid_statusActul = JsonMapper.ToJson(PlayerAndroid_status);
		File.WriteAllText(Application.dataPath +"/Data/Status_save/PlayerAndroid_status.json",PlayerAndroid_statusActul.ToString());

		string savecheck = File.ReadAllText(Application.dataPath + "/Data/Status_save/SaveCheck.json");
		JsonData SaveCheck = JsonMapper.ToObject(savecheck);

		SaveCheck["savecheck"] = 1;

		JsonData savecheck_ = JsonMapper.ToJson(SaveCheck);
		File.WriteAllText(Application.dataPath +"/Data/Status_save/SaveCheck.json",savecheck_.ToString());

		yield return null;

		//로딩 인덱스 변경
		LoadingIndex.LoadSceneIndex = 1;
		//로딩씬 로드
		SceneManager.LoadScene("LoadingScene");
	}
}