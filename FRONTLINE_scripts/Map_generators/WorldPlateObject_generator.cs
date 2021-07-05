using UnityEngine;
using LitJson;
using System.IO;

public class WorldPlateObject_generator : MonoBehaviour 
{
	//월드 플레이트 오브젝트
	public Transform[] WorldPlate;

	//배틀플레이트 배치 간격
	public float outlinePercent = 0.2f;

	void Awake()
	{
		Worldplate_Generator();
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
				newTile.GetComponent<WorldPlate_individual>().Coord_x = x;
				newTile.GetComponent<WorldPlate_individual>().Coord_y = y;
				newTile.GetComponent<WorldPlate_individual>().Type = (int)WPCJ[DataIndex]["WorldPlate_Type"];
				newTile.name = "worldplate_x" + x + "_y" + y;
				DataIndex ++;
			}
		}
		mapHolder.transform.position = new Vector3 (-19.5f,1,-0.7f);
		mapHolder.transform.localScale = new Vector3(1.35f,0.5f,1.35f);
	}

	//월드플레이트 생성기 에게 좌표를 부여하는 코드
	Vector3 CoordTo_WorldPlatePosition(int x,int y)
	{
		return new Vector3(15/2 +0.5f+x,0,-15/2+0.5f+y);
	}
}