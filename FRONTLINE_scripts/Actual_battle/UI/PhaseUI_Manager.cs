using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.IO;

public class PhaseUI_Manager : MonoBehaviour 
{
	public static PhaseUI_Manager instance;

	public Phase_Manager PM;

	public WeaponIcon_Manager WIM;

	//-----------------------------------------------------------------

	//상황 확정 버튼 텍스트
	public Text PlateManagerSureBtn_txt;

	//이동포인트 텍스트
	public Text MovingPoint_txt;

	//-----------------------------------------------------------------
	public bool TextReady = false;

	//텍스트 키와 텍스트값을 저장하는 딕셔너리
	public Dictionary<string,string> LoadedText;

	//유닛이름을 저장하는 제이슨데이터
	public JsonData UnitNameData;

	//-----------------------------------------------------------------

	//중앙의 상황 전달 이미지	
	public GameObject Situation_img;

	//중앙의 상황 전달 텍스트
	public Text Situation_txt;

	//-----------------------------------------------------------------

	//아군턴 제어 전체 패널
	public GameObject FriendlyTurnPanel;

	//행동/판단 버튼 패널
	public GameObject[] InActionPanel = new GameObject[2];

	//코스트 텍스트 
	//0 : 행동 코스트 // 1 : 판단 코스트 // 2 : 노말 코스트
	public Text[] Cost_txt = new Text[3];

	//현재 무기 아이콘
	public GameObject CurrentWeaponIcon_Img;

	//이름 텍스트
	public Text Name_txt;

	//유닛 스탯정보 텍스트
	// 0 : 속도 // 1 : 장갑 // 2 : 턴당발사수(RPT) // 3 : 발당데미지(DPR)
	public Text[] UnitInfo_txt = new Text[4];

	//탄약 텍스트
	public Text Ammunation_txt;

	//체력 텍스트
	public Text UnitInfoHealth_txt;

	//체력바 슬라이더
	public Slider UnitInfoHealthBar_slider;

	//-----------------------------------------------------------------

	//상시 출력 체력바 패널
	public GameObject HealthBar_panel;

	//상시 출력 체력바
	public Slider[] HealthBar_slider = new Slider[4];

	//상시 출력 체력바 텍스트
	public Text[] HealthBar_text = new Text[4];

	//-----------------------------------------------------------------

	//로그 패널 애니메이터
	public GameObject LogPanel_animator;

	//로그 텍스트
	public Text LogText_txt;

	//-----------------------------------------------------------------
	
	//데미지 팝업 텍스트 오브젝트
	public GameObject DamagePopUpText_obj;

	//데미지 팝업 텍스트 텍스
	public Text DamagePopUpText_txt;
	
	//-----------------------------------------------------------------

	//재장전 패널
	public GameObject Reload_panel;

	//-----------------------------------------------------------------

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

		LocalizedTextLoad();
		HealthBarPanelLoad();
	}

	void Start()
	{
		HealthBarPanelLoad();
	}

	//-----------------------------------------------------------------
	//LoadedText딕셔너리 초기화 , 유닛 이름데이터 불러오기
	public void LocalizedTextLoad()
	{
		LoadedText = new Dictionary<string,string>();

		//LanguageCode에서 로드한 언어코드를 기반으로 제이슨 텍스트 파일 접근
		string filepath = File.ReadAllText
		(Application.dataPath + "/Data/Language/BattPlate_Lang/" + "BPlang_"+LanguageCode.LCODE+".json");
		//string filepath = File.ReadAllText(Application.dataPath +"/Data/Language/BattPlate_Lang/BPlang_"+LanguageCode.LCODE+".json");

		//string datajson = File.ReadAllText(filepath);
		LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(filepath);
		
		for(int i = 0; i < loadedData.UIINFO.Length ; i++)
		{
			LoadedText.Add(loadedData.UIINFO[i].key,loadedData.UIINFO[i].value);
		}

		//유닛이름 데이터 언어코드 식별로 호출
		string unitnamedata = File.ReadAllText(Application.dataPath +"/Data/Language/UnitName/UnitName_"+LanguageCode.LCODE+".json");
		UnitNameData = JsonMapper.ToObject(unitnamedata);

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
	//-----------------------------------------------------------------
	//SituationImg_AppearActual에게 현재 상황 string을 전달하는 코드 
	//입력 값 : 현재 상황 인덱스
	public string SiutationImg_ToString(int situationCode)
	{
		string SituationTxt = "MissingText"; 

		switch (situationCode)
		{
			//상황 : 안드로이드 배치 단계 일때,
			case 0 : 
				SituationTxt = GetLocalizedValue("deploying andoird_txt");
				break;

			//상황 : 적페이즈 일때,
			case 1:
				SituationTxt = GetLocalizedValue("enemy action detected_txt");
				break;

			//상황 : 아군페이즈 일때,
			case 2:
				SituationTxt = GetLocalizedValue("friendly in action_txt");
				break;

			//상황 작전보고 일때,
			case 3:
				SituationTxt = GetLocalizedValue("report operation_txt");
				break;
		}

		return SituationTxt;
	}

	//중앙의 상황전달 이미지 애니메이션 출력 및 텍스트 아웃
	//입력 값 : 상황전달 텍스트
	public IEnumerator SituationImg_AppearActual_IE(int situationCode)
	{
		//중앙 이미지 활성화
		Situation_img.SetActive(true);
		
		//중앙 텍스트 출력
		Situation_txt.text = SiutationImg_ToString(situationCode);

		yield return new WaitForSeconds(3);

		//중앙 이미지 비활성화
		Situation_img.SetActive(false);
	}
	//-----------------------------------------------------------------
	//행동 버튼 클릭 메서드
	//행동 버튼 패널 활성화 / 판단 버튼 패널 비활성화
	public void ActingBtn_pressed()
	{
		InActionPanel[0].SetActive(true);
		InActionPanel[1].SetActive(false);
	}
	//판단 버튼 클릭 메서드
	//판단 버튼 패널 활성화 / 행동 버튼 패널 비활성화
	public void JudgingBtn_pressed()
	{
		InActionPanel[1].SetActive(true);
		InActionPanel[0].SetActive(false);
	}
	//-----------------------------------------------------------------
	//명령하달 완료 버튼 클릭 메서드
	public void CommandCompleteBtn_pressed()
	{
		//다음턴으로 넘기기
		PM.TurntoNextable = true;
	}
	//-----------------------------------------------------------------
	//유닛 스탯 정보 갱신메서드
	//FriendlyTurnPanel_Actual내에서만 호출합니다
	void FriendlyInfo_refresher(int FriendlyIndex,int FriendlyCode)
	{
		//유닛 이름 텍스트 할당
		Name_txt.text = UnitNameData[FriendlyCode]["name"].ToString();
		//유닛 스탯 텍스트 할당
		UnitInfo_txt[0].text = PM.PlayerAndroid_status[FriendlyIndex]["Speed"].ToString();
		UnitInfo_txt[1].text = PM.PlayerAndroid_status[FriendlyIndex]["Armor"].ToString();
		UnitInfo_txt[2].text = PM.PlayerAndroid_status[FriendlyIndex]["RPT"].ToString();
		UnitInfo_txt[3].text = PM.PlayerAndroid_status[FriendlyIndex]["DPB"].ToString();
		//총기 아이콘 이미지 할당
		CurrentWeaponIcon_Img.GetComponent<Image>().sprite = WIM.WeaponIcon[(int)PM.PlayerAndroid_status[FriendlyIndex]["GunIndex"]];
		//체력 텍스트 할당
		UnitInfoHealth_txt.text = 
		PM.UnitListInfo.UNITINFO[FriendlyIndex].CurrentHealth.ToString() + "/" + PM.PlayerAndroid_status[FriendlyIndex]["Health"].ToString();
		//체력바 게이지 값 할당
		UnitInfoHealthBar_slider.maxValue = (int)PM.PlayerAndroid_status[FriendlyIndex]["Health"];
		UnitInfoHealthBar_slider.value = (int)PM.UnitListInfo.UNITINFO[FriendlyIndex].CurrentHealth;
		//탄약 텍스트 할당
		Ammunation_txt.text = 
		PM.PlayerAndroid_status[FriendlyIndex]["ActualLoadedBullet"].ToString() + "/" + PM.PlayerAndroid_status[FriendlyIndex]["CurrentBullet"].ToString();
		//코스트 텍스트 할당
		Cost_txt[0].text = PM.PlayerAndroid_status[FriendlyIndex]["BehaviorCost"].ToString();
		Cost_txt[1].text = PM.PlayerAndroid_status[FriendlyIndex]["JudgementCost"].ToString();
		Cost_txt[2].text = PM.PlayerAndroid_status[FriendlyIndex]["NomalCost"].ToString();
	}

	//코스트 텍스트 갱신메서드
	public void CostText_refresher()
	{
		Cost_txt[0].text = PM.ActingCost.ToString();
		Cost_txt[1].text = PM.JudgingCost.ToString();
		Cost_txt[2].text = PM.NomalCost.ToString();
	}

	//탄약 텍스트 갱신 메서드
	//공격중일때는 데이터상의 탄환이 아닌 데이터에서 받은 값을 더한 값을 대입합니다
	public void BulletText_refresher(int FriendlyIndex,bool whileAttack,int plusAMMU)
	{
		//공격중 계산 일때,
		if(whileAttack)
		{
			//데이터값 + 
			Ammunation_txt.text = 
			((int)PM.PlayerAndroid_status[FriendlyIndex]["ActualLoadedBullet"] + plusAMMU).ToString() + 
			"/" + PM.PlayerAndroid_status[FriendlyIndex]["CurrentBullet"].ToString();
		}
		//비공격중 계산일때,
		else
		{
			//단순 데이터값 대입
			Ammunation_txt.text = 
			PM.PlayerAndroid_status[FriendlyIndex]["ActualLoadedBullet"].ToString() + 
			"/" + PM.PlayerAndroid_status[FriendlyIndex]["CurrentBullet"].ToString();
		}
	}

	//아군턴시 전투창 UI 작동 메서드
	public void FriendlyTurnPanel_Actual(int FriendlyIndex,int FriendlyCode)
	{
		//유닛 스탯 정보 갱신
		FriendlyInfo_refresher(FriendlyIndex,FriendlyCode);
		//아군 턴 패널 활성화
		FriendlyTurnPanel.SetActive(true);
	}

	//아군 상시 출력 체력바 패널 초기화
	void HealthBarPanelLoad()
	{
		for(int i = 0 ; i < 4 ; i++)
		{	
			//체력바 텍스트 할당
			HealthBar_text[i].text = UnitNameData[PM.UnitListInfo.UNITINFO[i].Index]["name"].ToString() + "\n" + 
			PM.PlayerAndroid_status[i]["Health"] + "/" + PM.PlayerAndroid_status[i]["MaxHealth"];

			//체력바 수치값 할당
			HealthBar_slider[i].maxValue = (int)PM.PlayerAndroid_status[i]["MaxHealth"];
			HealthBar_slider[i].value = (int)PM.PlayerAndroid_status[i]["Health"];
		}
	}

	//플레이트 매니저 에서 호출되는 상황 확정 버튼 텍스트 할당 메서드
	public void SituationSureBtnText_loader(int situationCode)
	{
		//0번 : 공격 / 1번 : 경계 / 2번 : 스캔 / 3번 : 이동 / 4번 : 공중지원
		switch(situationCode)
		{
			//공격
			case 0:
				PlateManagerSureBtn_txt.text = GetLocalizedValue("attacksure_txt");
				break;
			//경계
			case 1:
				PlateManagerSureBtn_txt.text = GetLocalizedValue("boundarysure_txt");
				break;
			//스캔
			case 2:	
				PlateManagerSureBtn_txt.text = GetLocalizedValue("scansure_txt");
				break;
			//이동
			case 3:
				PlateManagerSureBtn_txt.text = GetLocalizedValue("movesure_txt");
				break;
			//공중지원
			case 4:
				PlateManagerSureBtn_txt.text = GetLocalizedValue("airsupportsure_txt");
				break;
		}
	}

	//이동포인트 텍스트 갱신 메서드
	public void MovePoint_refresher(int leftPoint)
	{
		//텍스트 오브젝트 활성화
		MovingPoint_txt.gameObject.SetActive(true);
		//텍스트 할당
		MovingPoint_txt.text = leftPoint + "/" + PM.CurrentUnit.Speed/2;
	}

	//로그 출력 메서드
	// 0 : 공격 완료 // 1 : 발각된 적 없음 // 2 : 코스트 부족 // 3 : 경계종료 // 4 : 공중지원 종료 // 5 : 재장전할 탄약부족
	// 6 : 재장전이 필요없는 총기 // 7 : 이미 최대로 장전 // 8 : 이동완료 // 9 : 스캔종료 // 10 : 취소되었습니다
	// 11 : 장전된 탄약이 없습니다 // 12 : 재장전됨 // 13 : 더이상 이동 불가 // 14 번 : 적 사망 확인
	public IEnumerator LogOutputActual(int LogIndex)
	{
		switch (LogIndex)
		{
			//공격 완료
			case 0 :
				LogText_txt.text = GetLocalizedValue("attackcomplete_log");
				break;
			//발각된 적 없음
			case 1 :
				LogText_txt.text = GetLocalizedValue("nodetectedenemy_log");
				break;
			//코스트 부족
			case 2 :	
				LogText_txt.text = GetLocalizedValue("notenoughcost_log");
				break;
			//경계 종료
			case 3 :	
				LogText_txt.text = GetLocalizedValue("boundarycomplete_log");
				break;
			//공중지원 종료
			case 4 : 	
				LogText_txt.text = GetLocalizedValue("airsupportcomplete_log");
				break;
			//재장전할 탄약부족
			case 5 :
				LogText_txt.text = GetLocalizedValue("notenoughbullet_log");
				break;
			//재장전이 필요없는 총기
			case 6 :
				LogText_txt.text = GetLocalizedValue("unreloadablegun_log");
				break;
			//이미 최대로 장전
			case 7 :		
				LogText_txt.text = GetLocalizedValue("alreadyfullyload_log");
				break;
			//이동완료
			case 8 :	
				LogText_txt.text = GetLocalizedValue("movecomplete_log");
				break;
			//스캔종료
			case 9 :	
				LogText_txt.text = GetLocalizedValue("scancomplete_log");
				break;
			//취소되었습니다
			case 10 :	
				LogText_txt.text = GetLocalizedValue("canceled_log");
				break;
			//장전된 탄약이 없습니다
			case 11 :	
				LogText_txt.text = GetLocalizedValue("gunisempty_log");
				break;
			//재장전됨
			case 12:
				LogText_txt.text = GetLocalizedValue("reloadcomplete_log");
				break;
			//더이상 이동 불가
			case 13:
				LogText_txt.text = GetLocalizedValue("cantmovemore_log");
				break;
			//적 사망 확인
			case 14:
				LogText_txt.text = GetLocalizedValue("enemyeliminated_log");
				break;
		}
		//로그 애니메이션 실행
		LogPanel_animator.SetActive(false);
		LogPanel_animator.SetActive(true);
		yield return new WaitForSeconds(2);
	}
	
	//데미지 텍스트 팝업 메서드
	public void DamageTextPopupActual(Transform Target_transform,string Damage_str)
	{
		//데미지 텍스트 위치 할당
		Vector3 DamageText_pos = Camera.main.WorldToScreenPoint(Target_transform.position);
		//텍스트 할당
		DamagePopUpText_txt.text = Damage_str;
		//데미지 텍스트 생성
		Instantiate(DamagePopUpText_obj,DamageText_pos,Quaternion.identity,GameObject.FindGameObjectWithTag("Canvas").transform);
	}

	//적 체력바 갱신 메서드 
	public void EnemyHealthBar_refresher(GameObject EnemyHealthBar,int EnemyIndex)
	{
		//적 체력감소
		EnemyHealthBar.GetComponent<Slider>().value = PM.UnitListInfo.UNITINFO[EnemyIndex].CurrentHealth;
	}
	
	//적 체력바 로더
	//시작시 한번만 불러옵니다
	public void EnemyHealthBar_loader(GameObject EnemyHealthBar,int CurrentHealth)
	{
		//최대값 초기화
		EnemyHealthBar.GetComponent<Slider>().maxValue = CurrentHealth;
		//현재값 초기화
		EnemyHealthBar.GetComponent<Slider>().value = CurrentHealth;
	}

	//재장전 패널 애니메이션
	public IEnumerator ReloadPanelActual()
	{
		Reload_panel.SetActive(true);
		yield return new WaitForSeconds(1.2f);
		Reload_panel.SetActive(false);
	}
}