using System.Collections;
using UnityEngine;
using System;
using LitJson;
using System.IO;

public class PlateObject_Generator : MonoBehaviour 
{
	void Start()
	{
		/* string bpcj = File.ReadAllText(Application.dataPath + "/Data/MapCoords_save/wpc_5_7.json");
		JsonData BPCJ = JsonMapper.ToObject(bpcj);
		Battleplate_Generator(BPCJ);*/
	}

	//배틀 플레이트 오브젝트
	public Transform[] BattlePlate;

	//월드 플레이트 오브젝트
	public Transform[] WorldPlate;

	//배틀플레이트 배치 간격
	public float outlinePercent = 0.2f;

	//제이슨 세이브 파일 기반 배틀 플레이트 생성기
	public void Battleplate_Generator(JsonData BPCJ)
	{
		//맵을 묶어둘 오브젝트 명
		string holderName = "Generated Map";

		//만일, 이미 오브젝트가 존재할경우, 해당 오브젝트를 삭제
		if(transform.Find(holderName))
		{
			DestroyImmediate(transform.Find(holderName).gameObject);
		}

		Transform mapHolder = new GameObject (holderName).transform;
		mapHolder.parent = transform;

		int DataIndex = 0;

		//배틀플레이트 사이즈 제이슨 파일로 부터 불러오기
		string worldsize = File.ReadAllText(Application.dataPath +"/Data/MapSize.json");
		JsonData WS = JsonMapper.ToObject(worldsize);

		for(int x = 0;x<(int)WS[(int)BPCJ["BattlePlateLevel"]-1]["size_x"]; x++)
		{
			for(int y = 0; y < (int)WS[(int)BPCJ["BattlePlateLevel"]-1]["size_y"]; y++)
			{
				Vector3 tilePosition = CoordTo_BattlePlatePosition(x,y,(int)BPCJ["BattlePlateLevel"]);
				Transform newTile = Instantiate(BattlePlate[(int)BPCJ["BattlePlateCoords"][DataIndex]["Building_Index"]],tilePosition,Quaternion.Euler(Vector3.right*90)) as Transform;
				newTile.localScale = Vector3.one * (1-outlinePercent);
				newTile.parent = mapHolder;
				newTile.name = x.ToString() + "_" + y.ToString();
				DataIndex ++;
			}
		}

		Vector3 mapHolder_vec = new Vector3(-8,0,10);
		mapHolder.transform.position = mapHolder_vec;
	}

	//제이슨 파일기반 월드 플레이트 생성기
	public void Worldplate_Generator()
	{
		//월드플레이트 제이슨 파일 불러오기
		string Jsonstring = File.ReadAllText(Application.dataPath +"/Data/MapCoords_save/WPC_list.json");
		JsonData WPCJ = JsonMapper.ToObject(Jsonstring);

		string holderName = "BattlePlate Map";

		if(transform.Find(holderName))
		{
			DestroyImmediate(transform.Find(holderName).gameObject);
		}

		Transform mapHolder = new GameObject (holderName).transform;
		mapHolder.parent = transform;

		int DataIndex = 0;

		for(int x = 0;x<15; x++)
		{
			for(int y = 0; y<15; y++)
			{
				Vector3 tilePosition = CoordTo_WorldPlatePosition(x,y);
				Transform newTile = Instantiate(WorldPlate[(int)WPCJ[DataIndex]["WorldPlate_Type"]],tilePosition,Quaternion.Euler(Vector3.right*90)) as Transform;
				newTile.localScale = Vector3.one * (1-outlinePercent);
				newTile.parent = mapHolder;
				DataIndex ++;
			}
		}
	}

	//배틀플레이트 생성기 에게 좌표를 부여하는 코드
	Vector3 CoordTo_BattlePlatePosition(int x,int y,int WorldLevel)
	{
		string worldsize = File.ReadAllText(Application.dataPath +"/Data/MapSize.json");
		JsonData WS = JsonMapper.ToObject(worldsize);
		return new Vector3((int)WS[WorldLevel-1]["size_x"]/2 +0.5f+x,0,-(int)WS[WorldLevel-1]["size_y"]/2+0.5f+y);
	}

	//월드플레이트 생성기 에게 좌표를 부여하는 코드
	Vector3 CoordTo_WorldPlatePosition(int x,int y)
	{
		return new Vector3(15/2 +0.5f+x,0,-15/2+0.5f+y);
	}

}