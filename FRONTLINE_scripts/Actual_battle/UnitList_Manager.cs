//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

//유닛정보 저장 클래스 
//피아식별코드 //유닛인덱스 //유닛좌표 //현재체력 //발각상태 
public class UnitInfo 
{
	//피아식별 코드 / 아군 : 0 / 적군 : 1 /
	public int IdentifyCode;

	//정보를 참조할때 사용하는 인덱스
	//제이슨 파일로 부터 정보를 불러옵니다.
	public int Index;

	//유닛의 현재 위치 
	public int Coord_X;
	public int Coord_Y;

	//유닛의 현재 체력
	public int CurrentHealth;

	//발각상태 기본값 : 미발각
	public bool detected = false;

	//유닛의 속도
	public int Speed;

	//클래스 초기값 할당
	public UnitInfo(int identifycode,int index,int coord_x,int coord_y,int currenthealth,int speed)
	{
		IdentifyCode = identifycode;
		Index = index;
		Coord_X = coord_x;
		Coord_Y = coord_y;
		CurrentHealth = currenthealth;
		Speed = speed;
	}
}

//해당구조체는 유닛리스트 생성기와 로더의 반환값입니다.
//포함하는 변수 : 유닛리스트 , 유닛카운트(정수)
public struct UnitListStruct
{
	//유닛 정보 저장 클래스
	public UnitInfo[] UNITINFO;

	//전체유닛 수
	public int UNITCOUNT;
};

//총기스챗 구조체
//해당 배틀플레이트에 배치된 안드로이드의 총기만을 불러와야 합니다
/*public struct GunStat
{
	public int Name;//총기명
    public string Type;//총기종류
    public int DPB;//탄환당 데미지
    public int RPT;//턴당 발사수
    public int SpeedDecrease;//속도감소율
    public int AttackRange;//공격범위
    public int ResponseShotDamage;//대응사격 데미지
    public int SurpriseShotDamage;//기습사격 데미지
    public int CounterShotDamage;//기습시 대응사격 데미지
    public int RealodedBullets;//장전된 탄약수
    public int MaxBullet;//최대 탄약수
    public int JudgementCost;//판단 코스트
    public int BehaviorCost;//행동 코스트
    public int NomalCost;//일반 코스트

	public GunStat(int name,string type,int dpb,int rpt,int speeddecrease,
		int attackrange,int responseshotdamage,int surpriseshotdamage,int countershotdamage,
		int realodedbullets,int maxbullet,int judgementcost,int behaviorcost,int nomalcost)
		{
			Name = name;
			Type = type;
			DPB = dpb;
			RPT = rpt;
			SpeedDecrease = speeddecrease;
			AttackRange = attackrange;
			ResponseShotDamage = responseshotdamage;
			SurpriseShotDamage = surpriseshotdamage;
			CounterShotDamage = countershotdamage;
			RealodedBullets = realodedbullets;
			MaxBullet = maxbullet;
			JudgementCost = judgementcost;
			BehaviorCost = behaviorcost;
			NomalCost = nomalcost;
		}
}*/

public class UnitList_Manager : MonoBehaviour
{
	//플레이어 안드로이드 스테이터스
	public JsonData PlayerAndroid_status;

	//적 리트로이트 스테이터스
	public JsonData Enemy_status;

	//페이즈 UI 매니저---------------------------------
    public PhaseUI_Manager PUM;

    //------------------------------------------------

	//플레이어 안드로이드 스테이터스 데이터 할당 , 적 스테이터스 데이터 할당
	public void PlayerAndroidStat_setActual(int BattlePlateLevel)
	{
		//안드로이드 세이브 에서 아군 안드로이드 정보 불러오기
		string androidstatus = File.ReadAllText(Application.dataPath + "/Data/Status_save/PlayerAndroid_status.json");
		PlayerAndroid_status = JsonMapper.ToObject(androidstatus);

		//적 세이브 에서 적 정보 불러오기
		string enemystatus = File.ReadAllText
		(Application.dataPath + "/Data/DataBase/Enemy_Status/Level" + BattlePlateLevel.ToString() + "_Enemy_status.json");
		Enemy_status = JsonMapper.ToObject(enemystatus);
	}
	
	//안드로이드 스탯 세이브
	public void PlayerAndroidStat_save()
	{
		JsonData AndroidStat = JsonMapper.ToJson(PlayerAndroid_status);
		File.WriteAllText(Application.dataPath +"/Data/Status_save/PlayerAndroid_status.json",AndroidStat.ToString());
	}

	//유닛 리스트 세이브
	public void UnitList_save(UnitListStruct UnitListInfo)
	{
		JsonData UNITLIST = JsonMapper.ToJson(UnitListInfo);
		File.WriteAllText(Application.dataPath +
		"/Data/GameProgress_save/LastBattlePlate/UnitList.json",UNITLIST.ToString());
	}

	//거리 벡터 변환 산술식
	public Vector3 RangeToVector_calculator(int Range,bool isAttackRange)
	{
		Vector3 VectorForReturn;

		int vectorcord = (Range*2+1)*2;

		//공격 사거리 일경우,
		if(isAttackRange)
		{
			VectorForReturn = new Vector3(vectorcord,2,vectorcord);
		}	
		//탐지범위 사거리 일경우,
		else
		{
			VectorForReturn = new Vector3(vectorcord,0.5f,vectorcord);
		}
			
		return VectorForReturn;
	}

	//아군 경계,공격 거리 오브젝트 초기화 메서드
	public void FriendlyRangeSet_Actual(GameObject Friendly_obj1,GameObject Friendly_obj2,GameObject Friendly_obj3,GameObject Friendly_obj4)
	{
		GameObject[] FriendlyObejct = new GameObject[4];
		FriendlyObejct[0] = Friendly_obj1;
		FriendlyObejct[1] = Friendly_obj2;
		FriendlyObejct[2] = Friendly_obj3;
		FriendlyObejct[3] = Friendly_obj4;

		// 0번 자식 : IdentifiCationDistance / 1번 자식 : VisibleRange / 2번 자식 : AttackRange
		for(int i = 0; i < 4;i++)
		{
			//탐지범위 할당
			Vector3 IdentificationDistance_vec = RangeToVector_calculator((int)PlayerAndroid_status[i]["IdentificationDistance"],true);
			//Debug.Log(IdentificationDistance_vec);
			//Debug.Log(FriendlyObejct[i].gameObject.name);
			FriendlyObejct[i].transform.GetChild(0).transform.localScale = IdentificationDistance_vec;

			//가시범위 할당
			Vector3 VisibleRange_vec = RangeToVector_calculator((int)PlayerAndroid_status[i]["VisibleRange"],false);
			//Debug.Log(VisibleRange_vec);
			FriendlyObejct[i].transform.GetChild(1).transform.localScale = VisibleRange_vec;

			//공격범위 할당
			Vector3 AttackRange_vec = RangeToVector_calculator((int)PlayerAndroid_status[i]["AttackRange"],true);
			//Debug.Log(AttackRange_vec);
			FriendlyObejct[i].transform.GetChild(2).transform.localScale = AttackRange_vec;
		}
	}

	//유닛리스트 생성기 // 적의 좌표및 제이슨 파일 생성
	//출력 : 유닛리스트 구조체
	//BattlePLateCoord_To_MapFile로 맵파일을 할당하여 호출하십시오
	public UnitListStruct UNITLIST_GeneratorActual(JsonData MapFile)
	{
		//적의 수 제이슨 파일로 부터 불러오기
		string enemycount = File.ReadAllText(Application.dataPath +"/Data/EnemyCount.json");
		JsonData EnemyCountJson = JsonMapper.ToObject(enemycount);

		//적의 수 제이슨파일 기반으로 랜덤 생성
		int EnemyCount = Random.Range((int)EnemyCountJson
		[(int)MapFile["BattlePlateLevel"]-1]["MEC"],(int)EnemyCountJson
		[(int)MapFile["BattlePlateLevel"]-1]["LEC"]+1);

		//반환할 유닛 리스트 구조체
		UnitListStruct UnitList = new UnitListStruct();
		UnitList.UNITCOUNT = EnemyCount;
		UnitList.UNITINFO = new UnitInfo[EnemyCount];
	
		//배틀플레이트 사이즈 제이슨 파일로 부터 불러오기
		string worldsize = File.ReadAllText(Application.dataPath +"/Data/MapSize.json");
		JsonData WorldSize = JsonMapper.ToObject(worldsize);

		//월드레벨에 맞는 적 스탯 불러오기
		string enemystatus = File.ReadAllText(Application.dataPath 
		+ "/Data/DataBase/Enemy_Status/level" + (int)MapFile["BattlePlateLevel"] + "_Enemy_status.json");
		JsonData EnemyStatus = JsonMapper.ToObject(enemystatus);
		//Debug.Log("EnemyStatus Load succed!");

		//아군 수 만큼 도는 for문
		for(int FC = 0 ; FC < 4 ; FC++)
		{
			//각 안드로이드 개체 수치 입력
			UnitList.UNITINFO[FC] = new UnitInfo(0,(int)PlayerAndroid_status[FC]["code"],0,0,(int)PlayerAndroid_status[FC]["Health"],(int)PlayerAndroid_status[FC]["Speed"]);
		}

		//적 수 만큼 도는 for문
		for(int EC = 4 ; EC < EnemyCount ; EC++)
		{
			while(true)
			{
				//랜덤 좌표 할당
				int coord_x = Random.Range(0,(int)WorldSize[(int)MapFile["BattlePlateLevel"]-1]["size_x"]-1);
				int coord_y = Random.Range(5,(int)WorldSize[(int)MapFile["BattlePlateLevel"]-1]["size_y"]-1);

				//해당 좌표의 구조물이 장애물(배치불가)이 아닐떄
				if((int)MapFile["BattlePlateCoords"][coord_x * (int)WorldSize[(int)MapFile["BattlePlateLevel"]-1]["size_y"] + coord_y ]["BattlePlate_Type"] != 4)
				{
					//기존의 적과 같은 위치 인지 확인하는 코드
					bool same_coord = false;
					for(int i = 4 ; i < EC ; i++)
					{
						if(UnitList.UNITINFO[i].Coord_X == coord_x&&UnitList.UNITINFO[i].Coord_Y == coord_y)
						{
							same_coord = true;
						}
					}

					if(same_coord == false)
					{
						//적 인덱스 랜덤생성
						int enemyindex = Random.Range(0,6);

						//적을 유닛 리스트에 할당
						UnitList.UNITINFO[EC] = new UnitInfo
						(1,enemyindex,coord_x,coord_y,(int)EnemyStatus[enemyindex]["Health"],(int)EnemyStatus[enemyindex]["Speed"]);
						break;
					}
				}
			}
		}

		//배치된 적 제이슨 파일 생성
		JsonData UNITLIST = JsonMapper.ToJson(UnitList);
		File.WriteAllText(Application.dataPath +
		"/Data/GameProgress_save/LastBattlePlate/UnitList.json",UNITLIST.ToString());

		return UnitList;
	}

	//유닛리스트 로더 // 제이슨 파일을 기반으로 유닛리스트를 로드
	//출력 : 유닛리스트 구조체
	public UnitListStruct UNITLIST_Loader()
	{
		//유닛 리스트
		//UnitInfo[] UnitList;

		//제이슨파일에서 유닛 리스트 불러오기
		string unitlist = File.ReadAllText(Application.dataPath + "/Data/GameProgress_save/LastBattlePlate/UnitList.json");
		JsonData UNITLIST = JsonMapper.ToObject(unitlist);

		//반환할 유닛리스트 구조체
		UnitListStruct UnitList = new UnitListStruct();

		//유닛 리스트 초기화
		UnitList.UNITINFO = new UnitInfo[(int)UNITLIST["UNITCOUNT"]];
		UnitList.UNITCOUNT = (int)UNITLIST["UNITCOUNT"];

		//유닛리스트 개별 유닛 정보 할당
		for(int i = 0 ; i < UnitList.UNITCOUNT ; i++)
		{
			UnitList.UNITINFO[i] = new UnitInfo((int)UNITLIST["UNITINFO"][i]["IdentifyCode"]
			,(int)UNITLIST["UNITINFO"][i]["Index"],(int)UNITLIST["UNITINFO"][i]["Coord_X"]
			,(int)UNITLIST["UNITINFO"][i]["Coord_Y"],(int)UNITLIST["UNITINFO"][i]["CurrentHealth"]
			,(int)UNITLIST["UNITINFO"][i]["Speed"]);

			UnitList.UNITINFO[i].detected = (bool)UNITLIST["UNITINFO"][i]["detected"];
		}

		//유닛리스트구조체 반환
		return UnitList;
	}
	
	//적유닛 생성기 //처음에 호출된후, 호출될일이 없습니다
	//출력 : Transform리스트(유닛 오브젝트 리스트) 
	public Transform[] EnemySpawner_Actual(UnitListStruct UnitList,Transform EnemyObject,int BattlePlateLevel)
	{
		//반환할 Transform리스트 (유닛 오브젝트 리스트)
		Transform[] UnitObjList = new Transform[UnitList.UNITCOUNT];

		string enemyholder = "Enemys";

		//만일, 이미 오브젝트가 존재할경우, 해당 오브젝트를 삭제
		if(transform.Find(enemyholder))
		{
			DestroyImmediate(transform.Find(enemyholder).gameObject);
		}

		Transform EnemyHolder = new GameObject (enemyholder).transform;
		EnemyHolder.parent = transform;

		for(int i = 4 ; i < UnitList.UNITCOUNT ; i++)
		{
			//위치될 플레이트 위치
			Transform enemy_pos = GameObject.Find(UnitList.UNITINFO[i].Coord_X + "_" + UnitList.UNITINFO[i].Coord_Y).transform;

			//플레이트 위치 벡터화
			Vector3 EnemyPos_vector = enemy_pos.transform.position;
			//오브젝트 생성
			Transform Deployed_Enemy = Instantiate(EnemyObject,EnemyPos_vector,EnemyObject.transform.rotation) as Transform;
			//유닛 호출 인덱스 스크립트에 할당
			Deployed_Enemy.GetChild(2).GetComponent<Unit_tag>().Index = i;
			//유닛 오브젝트 이름 할당
			Deployed_Enemy.name = "Enemy_" + (i-3).ToString();
			//유닛 '적홀더' 에 자식 귀속
			Deployed_Enemy.parent = EnemyHolder;

			//체력바 수치값 할당
			PUM.EnemyHealthBar_loader(Deployed_Enemy.GetChild(3).transform.GetChild(4).gameObject,UnitList.UNITINFO[i].CurrentHealth);

			//공격 사거리 오브젝트 할당
			//Vector3 AttackRange = RangeToVector_calculator((int)Enemy_status[UnitList.UNITINFO[i].Index]["AttackRange"],true);
			//Deployed_Enemy.GetChild(0).gameObject.transform.localScale = AttackRange;

			//감지 사거리 오브젝트 할당
			Vector3 IdentificationDistance = 
			RangeToVector_calculator((int)Enemy_status[UnitList.UNITINFO[i].Index]["IdentificationDistance"],false);
			Deployed_Enemy.GetChild(1).gameObject.transform.localScale = IdentificationDistance;

			//유닛 오브젝트 리스트에 할당
			UnitObjList[i] = Deployed_Enemy;
		}

		return UnitObjList;
	}

	//배틀 플레이트 정보 로더
	//출력 : 배틀플레이트 정보
	//BattlePLateCoord_To_MapFile로 맵파일을 할당하여 호출하십시오
	public BattlePlateStruct BattlePlateListLoader(JsonData MapFile)
	{
		//제이슨 맵사이즈 파일에서 맵크기 불러오기
		string worldsize = File.ReadAllText(Application.dataPath +"/Data/MapSize.json");
		JsonData WS = JsonMapper.ToObject(worldsize);

		//배틀플레이트 구조체 생성
		//배틀플레이트 구조체 내 배틀플레이트 좌표 초기화
		BattlePlateStruct BPS = new BattlePlateStruct();
		BPS.BattlePlateLevel = (int)MapFile["BattlePlateLevel"];
		BPS.BattlePlateCoords = new BattlePlate_Class
		[(int)WS[BPS.BattlePlateLevel-1]["size_x"],(int)WS[BPS.BattlePlateLevel-1]["size_y"]];
		
		int dataindex = 0;

		//배틀플레이트 세이브파일을 기반으로 정보를 로드
		for(int x = 0; x < (int)WS[BPS.BattlePlateLevel-1]["size_x"]; x++)
		{
			for(int y = 0; y < (int)WS[BPS.BattlePlateLevel-1]["size_y"]; y++)
			{
				BPS.BattlePlateCoords[x,y] = new BattlePlate_Class(
				(int)MapFile["BattlePlateCoords"][dataindex]["BattlePlate_Type"],
				(int)MapFile["BattlePlateCoords"][dataindex]["Building_Index"]);
				dataindex ++;
			}
		}

		return BPS;
	}

	//월드플레이트 선택신에서 선택된 베틀플레이트를 기준으로 맵파일을 할당시키는 코드
	//반환값 : 제이슨 맵파일
	public JsonData BattlePLateCoord_To_MapFile()
	{
		string BattlePlateCoord = File.ReadAllText
		(Application.dataPath + "/Data/GameProgress_save/LastBattlePlate/BattlePlateCoord_ForLoad.json");
		JsonData BPCL = JsonMapper.ToObject(BattlePlateCoord);

		string BattlePlateJson = File.ReadAllText(Application.dataPath + 
		"/Data/MapCoords_save/wpc_" + BPCL["X"].ToString() + "_" + BPCL["Y"].ToString() + ".json");
		JsonData BPJ = JsonMapper.ToObject(BattlePlateJson);

		return BPJ;
	}

}