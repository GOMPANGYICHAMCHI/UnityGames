using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AndroidDeploying_Manager : MonoBehaviour 
{	
	//현재 아군 순서 인덱스
	public int CurrentIndex = 0;

	[SerializeField]
	//배치된 아군수
	int DeployedFriendly = 0;

	int Startable = 0;
	
	//배치 비확정 메테리얼
	public Material UnsureDeploy_mat;

	//배치확정 매테리얼
	public Material SureDeploy_mat;

	//일반 플레이트 메테리얼
	public Material NomalPlate_mat;

	//선택가능한 플레이트 메테리얼
	public Material SelectablePlate_Mat;

	//-----------------------------------------------------------------

	//아군 오브젝트
	public GameObject FriendlyObject;

	//배치된 아군 오브젝트
	public GameObject[] FriendlyObject_Deployed = new GameObject[4];

	//배틀플레이트 마개
	public GameObject[] BattlePlateCap = new GameObject[3];

	//배치된 타일 마개
	public GameObject[] DeployedTileCap = new GameObject[4];

	//첫번째줄 타일 오브젝트
	public GameObject[] FirstLineTiles;

	//스탯 타일 오브젝트
	public GameObject[] StatTile = new GameObject[2];

	//현재 타일 오브젝트
	public GameObject[] CurrentTile_obj = new GameObject[4];

	//-----------------------------------------------------------------

	//페이즈 UI 매니저
	public PhaseUI_Manager PUM;

	//페이즈 매니저 
	public Phase_Manager PM;
	
	//웨폰 아이콘 매니저
	public WeaponIcon_Manager WIM;

	//메인카메라
	public Camera MainCamera;

	//현재 안드로이드 배치중
	public bool NowCurrentDeploying = false;

	//안드로이드가 배치되었는지 확인하는 변수
	public bool[] Deployed_Check = new bool[4];

	//-----------------------------------------------------------------

	//아군배치 전체 패널
	public GameObject FriendlyDeploying_panel;

	//유닛이름 텍스트 
	public GameObject[] UnitName_Btn = new GameObject[4];
	
	//선택된 유닛이름 식별 트래킹 이미지
	public GameObject[] NameLight_img = new GameObject[4];

	//아군배치 확정버튼
	public Button Deploying_btn;

	//아군배치 취소버튼
	public Button CancelDeploying_btn;

	//전투시작버튼
	public Button BattleStart_btn;

	//public GameObject[] DeployableTiles_go = new GameObject[];

	//-----------------------------------------------------------------

	//UnitInfo내의 유닛스탯 텍스트

	//유닛 스탯 텍스트
	public Text[] UnitStats_txt = new Text[4];

	//선택된 유닛 이름 텍스트
	public Text UnitInfoName_txt;

	//선택된 유닛 체력 텍스트 (형식 : 10/10)
	public Text UnitInfoHealth_txt;

	//선택된 유닛 탄약 잔량 텍스트 (형식 : 10/10)
	public Text UnitInfoBullets_txt;

	//현재 무기 아이콘
	public GameObject CurrentWeaponIcon;

	//선택된 유닛 체력 슬라이더
	public Slider UnitInfoHealthBar;

	//-----------------------------------------------------------------

	//아군 트래킹 UI 패널
	public GameObject FriendlyTracker_panel;

	//-----------------------------------------------------------------

	float starttime;

	void Start()
	{
		StartAbleIssue_Updater();
		starttime = Time.time;
		//첫번쨰 줄 메테리얼 변경
		FirstTileMaterial_changer();
		//배틀플레이트 캡 활성화
		DeployingBattleplateCap();
		//아군 안드로이드 오브젝트 생성
		GeneratingFriendly();
		//아군안드로이드 범위 오브젝트 초기화
		PM.FriendlyRangeSet_Actual(FriendlyObject_Deployed[0],FriendlyObject_Deployed[1],FriendlyObject_Deployed[2],FriendlyObject_Deployed[3]);
		//아군배치 코루틴 메서드 시작
		StartCoroutine(AndroidDeployingStart_Actual());
	}

	void Update()
	{
		//만약 현재 안드로이드 배치 상태 일경우,
		if(NowCurrentDeploying)
		{
			//마우스 클릭시
			if(Input.GetMouseButtonDown(0))
			{
				//레이캐스트 생성
				Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;

				if(Physics.Raycast(ray ,out hit)&&hit.transform.gameObject.tag == "BattlePlate")
				{
					//현재 아군 오브젝트를 클릭한 플레이트 위치와 일치화
					FriendlyObject_Deployed[CurrentIndex].transform.position = hit.transform.position;
					//현재 아군 오브젝트 활성화
					FriendlyObject_Deployed[CurrentIndex].gameObject.SetActive(true);

					//배치확정 버튼 활성화
					Deploying_btn.interactable = true;

					//현재 타일 오브젝트 할당
					CurrentTile_obj[CurrentIndex] = hit.transform.gameObject;
				}
			}
		}
		//Flicker
		float t = (Mathf.Sin(Time.time - starttime)) * 5f;
		SelectablePlate_Mat.color = Color.Lerp(Color.white,Color.yellow,t);
	}

	//전투시작버튼 클릭시
	public void BattleStartBtn_pressed()
	{
		Destroy(FriendlyDeploying_panel);
		for(int i = 0;i < FirstLineTiles.Length;i++)
		{
			FirstLineTiles[i].GetComponent<MeshRenderer>().material = NomalPlate_mat;
		}

		//페이즈 매니저 에게 아군 오브젝트 리스트 할당
		for(int i = 0;i < 4;i++)
		{
			PM.UnitObjList[i] = FriendlyObject_Deployed[i].transform;
		}

		

		//페이즈매니저 에서 페이즈메이커 액추얼 실행(이후부터 자동 반복문)
		PM.PhaseMaker_Actual();
		Destroy(this.gameObject);
	}

	//시작 가능한 아군 상태 확인
	void StartAbleIssue_Updater()
	{
		for(int i = 0 ;i < 4; i++)
		{
			if(PM.UnitListInfo.UNITINFO[i].CurrentHealth == 0)
			{
				UnitName_Btn[i].GetComponent<Button>().interactable = false;
			}
			else
			{
				Startable ++;
			}
		}
	}

	//첫번째줄 플레이트 메테리얼 변경
	void FirstTileMaterial_changer()
	{
		for(int i = 0;i < FirstLineTiles.Length;i++)
		{
			FirstLineTiles[i].GetComponent<MeshRenderer>().material = SelectablePlate_Mat;
		}
	}

	//배틀플레이트 캡 활성화
	void DeployingBattleplateCap()
	{
		BattlePlateCap[PM.CurrentBattlePlate.BattlePlateLevel-1].SetActive(true);
	}

	//아군 오브젝트 생성
	void GeneratingFriendly()
	{
		for(int i = 0 ; i < 4 ; i++)
		{
			//초기 좌표 0.0.0 으로 할당
			Vector3 Friendly_pos = new Vector3(0,0,0);

			//아군 유닛 오브젝트 생성
			GameObject DeployedFriendly = Instantiate(FriendlyObject,Friendly_pos,Quaternion.identity);
			//유닛 태그에 인덱스 할당
			DeployedFriendly.transform.GetChild(3).GetComponent<Unit_tag>().Index = i;
			//배치된 아군 오브젝트에 편성
			FriendlyObject_Deployed[i] = DeployedFriendly;
			//이름 변경
			DeployedFriendly.name = ("Friendly_" + i);
			//오브젝트 비활성화
			DeployedFriendly.gameObject.SetActive(false);
		}
	}

	//아군배치 관련 UI메서드 아군 이름 텍스트 할당//아군 배치 패널 활성화//첫번째 안드로이드 트래킹 이미지 활성화
	//입력값 : 아군 유닛코드
	void FriendlyNameText_set(int FriendlyCode_1,int FriendlyCode_2,int FriendlyCode_3,int FriendlyCode_4)
	{
		int[] FC = new int[4] {FriendlyCode_1,FriendlyCode_2,FriendlyCode_3,FriendlyCode_4};

		//아군 이름 텍스트 할당
		for(int i= 0;i<4;i++)
		{
			UnitName_Btn[i].GetComponentInChildren<Text>().text = PUM.UnitNameData[FC[i]]["name"].ToString();
		}

		//아군 배치 패널 활성화
		FriendlyDeploying_panel.SetActive(true);
	}

	//안드로이배치스타트 액추얼
	//코루틴으로 호출 //호출후 역할을 AndroidDeployingToNext로 넘겨주게 됩니다
	IEnumerator AndroidDeployingStart_Actual()
	{
		//중앙 이미지 활성화
		StartCoroutine(PUM.SituationImg_AppearActual_IE(0));

		yield return new WaitForSeconds(3);

		//아군배치 관련 UI메서드 아군 이름 텍스트 할당
		//아군 배치 패널 활성화
		//첫번째 안드로이드 트래킹 이미지 활성화
		FriendlyNameText_set(
		PM.UnitListInfo.UNITINFO[0].Index,PM.UnitListInfo.UNITINFO[1].Index,   
		PM.UnitListInfo.UNITINFO[2].Index,PM.UnitListInfo.UNITINFO[3].Index);
	}

	//배치 확정버튼 메서드
	void DeployingBtn_pressed()
	{
		
		//확정배치 아군 매테리얼 변경
		FriendlyObject_Deployed[CurrentIndex].transform.GetChild(3).GetComponent<MeshRenderer>().material = SureDeploy_mat;

		StatTile[0].gameObject.SetActive(false);
		StatTile[1].gameObject.SetActive(false);

		//배치확정된 타일 메테리얼 변경
		CurrentTile_obj[CurrentIndex].GetComponent<MeshRenderer>().material = NomalPlate_mat;

		//현재 선택된 타일 위치에 타일 마개 배치
		DeployedTileCap[CurrentIndex].transform.position = CurrentTile_obj[CurrentIndex].transform.position;

		//아군 식별트래킹 이미지 비활성화
		NameLight_img[CurrentIndex].gameObject.SetActive(false);

		Deployed_Check[CurrentIndex] = true;

		//버튼 비활성화
		Deploying_btn.interactable = false;
		//배치취소 버튼 비활성화
		CancelDeploying_btn.interactable = false;

		//현재 배치 상태 불가
		NowCurrentDeploying = false;
		DeployedFriendly +=1;

		if(DeployedFriendly == Startable)
		{
			BattleStart_btn.gameObject.SetActive(true);
		}
	}

	//배치 취소버튼 메서드
	void DeployingCancelBtn_pressed()
	{
		//현재 선택된 아군의 오브젝트가 활성화 상태일떄,
		if(FriendlyObject_Deployed[CurrentIndex].gameObject.activeSelf == true)
		{
			//배치확정된 타일 메테리얼 변경
			CurrentTile_obj[CurrentIndex].GetComponent<MeshRenderer>().material = SelectablePlate_Mat;
			//유닛 메테리얼 변경
			FriendlyObject_Deployed[CurrentIndex].transform.GetChild(3).GetComponent<MeshRenderer>().material = UnsureDeploy_mat;
			//현재 선택한 아군 오브젝트 비활성화
			FriendlyObject_Deployed[CurrentIndex].gameObject.SetActive(false);
			//아군 식별트래킹 이미지 비활성화
			NameLight_img[CurrentIndex].gameObject.SetActive(false);
			//현재 선택된 타일 위치에 타일 마개 비활성화
			DeployedTileCap[CurrentIndex].gameObject.SetActive(false);
			//배치확정 버튼 비활성화
			Deploying_btn.interactable = false;
			StatTile[0].gameObject.SetActive(false);
			StatTile[1].gameObject.SetActive(false);

			//아군 식별트래킹 이미지 비활성화
			NameLight_img[CurrentIndex].gameObject.SetActive(false);
			Deployed_Check[CurrentIndex] = false;

			//배치버튼 비활성화
			Deploying_btn.interactable = false;
			//배치취소 버튼 비활성화
			CancelDeploying_btn.interactable = false;
			//현재 배치 상태 불가능
			NowCurrentDeploying = false;
			DeployedFriendly -= 1;
			BattleStart_btn.gameObject.SetActive(false);
		}
	}

	//안드로이드 버튼 메서드
	//입력값 : 유닛 인덱스
	void AndroidBtn_pressed(int UnitIndex)
	{
		//배치확정 버튼 비활성화
		Deploying_btn.interactable = false;
		//--------------------------------------------------------------------------------
		//인덱스 변경전

		//아군 식별트래킹 이미지 비활성화
		NameLight_img[CurrentIndex].gameObject.SetActive(false);

		//만약 현재 아군이 배치되지 않은 상태라면,
		if(Deployed_Check[CurrentIndex] == false)
		{
			//배치된 아군 오브젝트 비활성화
			FriendlyObject_Deployed[CurrentIndex].gameObject.SetActive(false);
			//현재 배치 상태 가능
			NowCurrentDeploying = true;
		}

		//--------------------------------------------------------------------------------

		//현재 선택 아군인덱스 = 선택 인덱스
		CurrentIndex = UnitIndex;

		//만약 현재 아군이 배치되지 않은 상태라면,
		if(Deployed_Check[CurrentIndex] == false)
		{
			//현재 배치 상태 가능
			NowCurrentDeploying = true;
		}

		else
		{
			//현재 배치 상태 불가능
			NowCurrentDeploying = false;
			//배치취소 버튼 활성화
			CancelDeploying_btn.interactable = true;
		}

		//유닛 스탯 텍스트 할당
		StatText_setActual();

		//유닛인포 이름 텍스트 할당
		UnitInfoName_txt.text = PUM.UnitNameData[PM.UnitListInfo.UNITINFO[CurrentIndex].Index]["name"].ToString();

		//아군 식별트래킹 이미지 활성화
		NameLight_img[CurrentIndex].gameObject.SetActive(true);
	}

	//스탯 텍스트 할당 메서드
	void StatText_setActual()
	{
		StatTile[0].gameObject.SetActive(true);
		StatTile[1].gameObject.SetActive(true);
		UnitStats_txt[0].text = PM.PlayerAndroid_status[CurrentIndex]["Speed"].ToString();
		UnitStats_txt[1].text = PM.PlayerAndroid_status[CurrentIndex]["Armor"].ToString();
		UnitStats_txt[2].text = PM.PlayerAndroid_status[CurrentIndex]["RPT"].ToString();
		UnitStats_txt[3].text = PM.PlayerAndroid_status[CurrentIndex]["DPB"].ToString();

		CurrentWeaponIcon.GetComponent<Image>().sprite = WIM.WeaponIcon[(int)PM.PlayerAndroid_status[CurrentIndex]["GunIndex"]];

		UnitInfoHealth_txt.text = 
		PM.UnitListInfo.UNITINFO[CurrentIndex].CurrentHealth.ToString() + "/" + PM.PlayerAndroid_status[CurrentIndex]["Health"].ToString();

		UnitInfoHealthBar.maxValue = (int)PM.PlayerAndroid_status[CurrentIndex]["Health"];
		UnitInfoHealthBar.value = (int)PM.UnitListInfo.UNITINFO[CurrentIndex].CurrentHealth;

		UnitInfoBullets_txt.text = 
		PM.PlayerAndroid_status[CurrentIndex]["ActualLoadedBullet"].ToString() + "/" + PM.PlayerAndroid_status[CurrentIndex]["CurrentBullet"].ToString();
	}	
}