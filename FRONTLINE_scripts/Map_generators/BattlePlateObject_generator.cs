using UnityEngine;
using LitJson;
using System.IO;

public class BattlePlateObject_generator : MonoBehaviour 
{
	//배틀 플레이트 오브젝트
	// 0 : 은폐구조물 / 1 : 전략구조물 /
	// 2 : 일반 구조물/ 3 : 일반 플레이트 /
	// 4 : 장애물
	public Transform[] BattlePlate;

	//안드로이드 디플로잉 매니저
	public AndroidDeploying_Manager ADM;

	//첫번쨰 라인 타일 오브젝트
	public GameObject[] FirstLineTiles;

	//배틀플레이트 배치 간격
	//public float outlinePercent = 0.2f;

	//배틀플레이트 배치 좌표
	Vector3[] BP_pos = new [] { new Vector3(-8,0,10), new Vector3(-12,0,12), new Vector3(-16,0,17) };

	void Awake()
	{
		//마지막 배틀플레이트 정보 불러오기
		string loadedmap = File.ReadAllText(Application.dataPath + "/Data/GameProgress_save/LastBattlePlate/BattlePlateCoord_ForLoad.json");
		JsonData LoadedMap = JsonMapper.ToObject(loadedmap);

		//마지막 배틀플레이트 정보를 토대로 배틀플레이트 정보 로드
		string bpcj = File.ReadAllText(Application.dataPath + 
		"/Data/MapCoords_save/wpc_" + LoadedMap["X"] + "_" + LoadedMap["Y"] + ".json");
		JsonData BPCJ = JsonMapper.ToObject(bpcj);

		//맵생성
		Battleplate_Generator(BPCJ);

		ADM.FirstLineTiles = FirstLineTiles;

		//현재 스크립트 파기
		Destroy(this);
	}

	//제이슨 세이브 파일 기반 배틀 플레이트 생성기
	public void Battleplate_Generator(JsonData BPCJ)
	{
		//맵을 묶어둘 오브젝트 명
		/*string holderName = "Generated Map";

		//만일, 이미 오브젝트가 존재할경우, 해당 오브젝트를 삭제
		if(transform.Find(holderName))
		{
			DestroyImmediate(transform.Find(holderName).gameObject);
		}

		Transform mapHolder = new GameObject (holderName).transform;
		mapHolder.parent = transform;*/

		int DataIndex = 0;

		//배틀플레이트 사이즈 제이슨 파일로 부터 불러오기
		string worldsize = File.ReadAllText(Application.dataPath +"/Data/MapSize.json");
		JsonData WS = JsonMapper.ToObject(worldsize);

		//첫번쨰 라인 타일 오브젝트 수량 할당
		FirstLineTiles = new GameObject[(int)WS[(int)BPCJ["BattlePlateLevel"]-1]["size_x"]];

		int LineCounter = 0;

		for(int x = 0;x<(int)WS[(int)BPCJ["BattlePlateLevel"]-1]["size_x"]; x++)
		{
			for(int y = 0; y < (int)WS[(int)BPCJ["BattlePlateLevel"]-1]["size_y"]; y++)
			{
				Vector3 tilePosition = CoordTo_BattlePlatePosition(x,y,(int)BPCJ["BattlePlateLevel"]);
				Transform newTile = Instantiate(BattlePlate[(int)BPCJ["BattlePlateCoords"][DataIndex]["Building_Index"]],tilePosition,Quaternion.identity);
				//newTile.localScale = Vector3.one * (1-outlinePercent);
				newTile.localScale = new Vector3 (0.04f,0.04f,0.04f);
				newTile.parent = this.gameObject.transform;
				newTile.name = x.ToString() + "_" + y.ToString();
				//좌표 값 할당
				newTile.gameObject.GetComponent<PlateIndividual_manager>().X = x;
				newTile.gameObject.GetComponent<PlateIndividual_manager>().Y = y;
				DataIndex ++;

				//첫번쨰 줄 타일 일 경우,
				if(y==0)
				{	
					//게임 오브젝트 할당
					FirstLineTiles[LineCounter] = newTile.transform.gameObject;
					LineCounter ++;
				}
			}
		}

		this.gameObject.transform.position = BP_pos[(int)BPCJ["BattlePlateLevel"]-1];
	}

	//배틀플레이트 생성기 에게 좌표를 부여하는 코드
	Vector3 CoordTo_BattlePlatePosition(int x,int y,int WorldLevel)
	{
		string worldsize = File.ReadAllText(Application.dataPath +"/Data/MapSize.json");
		JsonData WS = JsonMapper.ToObject(worldsize);
		return new Vector3((int)WS[WorldLevel-1]["size_x"]/2 +0.5f+x,0,-(int)WS[WorldLevel-1]["size_y"]/2+0.5f+y);
	}
}