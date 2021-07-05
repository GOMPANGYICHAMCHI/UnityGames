//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

//경고! 다음 스크립트는 세이브파일을 덮어씌웁니다 새로생성될때 한번만 구동하십시오!

//월드플레이트 클래스
class WorldPlate_Class
{
	//월드 플레이트 지정형식 /0 : 기지/ 1 : 숲속마을/ 2 : 주택가/ 3 : 대도시/
	public int WorldPlate_Type;

	//월드 플레이트 레벨 /1레벨 : 8x8 / 2레벨 : 12x12 / 3레벨 : 12x16 /
	public int WorldPlateLevel;

	//x좌표
	public int X_coord;

	//y좌표
	public int Y_coord;

	public WorldPlate_Class(int worldplate_type,int worldlevel,int x,int y)
	{
		WorldPlate_Type = worldplate_type;
		WorldPlateLevel = worldlevel;
		X_coord = x;
		Y_coord = y;
	}
}

public class PlateCoords_Generator : MonoBehaviour 
{
	/*void Start()
	{
		WorldPlateCoord_GeneratorActual();
	}*/

	//월드플레이트 좌표 생성기 / 하위 배틀플레이트 생성기 까지 전부 제어
	public void WorldPlateCoord_GeneratorActual()
	{
		//월드 플레이트 좌표 생성, 월드 크기 초기화
		WorldPlate_Class[,] WORLDPLATE_coords = new WorldPlate_Class[15,15];

		//맵세이브 파일 폴더
		string MapSaveFolder_dir;
		MapSaveFolder_dir = Application.persistentDataPath + "/Data/MapCoords_save";
		DirectoryInfo MAPSAVEFOLDER = new DirectoryInfo(MapSaveFolder_dir);

		//맵세이브 파일 폴더가 없을 경우, 폴더를 생성
		if (MAPSAVEFOLDER.Exists == false)
		{
			MAPSAVEFOLDER.Create();
		}

		//3레벨 월드플레이트 생성기
		for(int x = 0 ; x < 15 ; x++ )
		{
			for(int y = 0 ; y < 15 ; y++)
			{
				//3레벨 월드 플레이트 생성식
				//도시 100%
				WORLDPLATE_coords[x,y] = new WorldPlate_Class(3,3,x,y);
				BattlePlateCoord_City_GeneratorActual(3,x,y);
			}
		}

		//2레벨 월드플레이트 생성기
		for(int x = 3 ; x < 12 ; x++ )
		{
			for(int y = 3 ; y < 12 ; y++)
			{
				int R = Random.Range(0,10);

				//2레벨 월드 플레이트 생성식 		  	
				//주택가 90%  ,  대도시 10%     

				//대도시 생성 10%
				if(R == 0)
				{	
					WORLDPLATE_coords[x,y] = new WorldPlate_Class(3,2,x,y);
					BattlePlateCoord_City_GeneratorActual(2,x,y);
				}
				//주택가 생성 90%
				else
				{
					WORLDPLATE_coords[x,y] = new WorldPlate_Class(2,2,x,y);
					BattlePlateCoord_DownTown_GeneratorActual(2,x,y);
				}
			}
		}

		//1레벨 월드플레이트 생성기
		for(int x = 5 ; x < 10 ; x++ )
		{
			for(int y = 5 ; y < 10 ; y++)
			{
				int R = Random.Range(0,10);
				
				//1레벨 월드 플레이트 생성식 /생성확률
				//숲속마을 90%   ,   주택가 10%

				//주택가 생성 10%
				if(R == 0)
				{
					WORLDPLATE_coords[x,y] = new WorldPlate_Class(2,1,x,y);
					BattlePlateCoord_DownTown_GeneratorActual(1,x,y);
				}
				//숲속마을 생성 90%
				else
				{
					WORLDPLATE_coords[x,y] = new WorldPlate_Class(1,1,x,y);
					BattlePlateCoord_ForestTown_GeneratorActual(1,x,y);
				}
			}
		}

		//기지 플레이트 생성 
		//x좌표 : 7 
		//y좌표 : 7
		WORLDPLATE_coords[7,7] = new WorldPlate_Class(0,0,7,7);

		//제이슨 세이브파일 생성
		JsonData data = JsonMapper.ToJson(WORLDPLATE_coords);
		File.WriteAllText(Application.dataPath + "/Data/MapCoords_save/WPC_list.json" , data.ToString());
	}
	
	//배틀플레이트(숲속마을) 좌표 제이슨파일 생성기    입력값 : 월드 플레이트 위험도 레벨 , 월드플레이트 X,Y좌표
	public /*BattlePlate_Class[,]*/void BattlePlateCoord_ForestTown_GeneratorActual(int WorldLevel,int BP_x,int BP_y)
	{
		//배틀플레이트 사이즈 제이슨 파일로 부터 불러오기
		string worldsize = File.ReadAllText(Application.dataPath +"/Data/MapSize.json");
		JsonData WS = JsonMapper.ToObject(worldsize);

		//배틀플레이트 좌표 변수 생성,월드 크기 초기화,월드 레벨 할당
		BattlePlateStruct BATTLEPLATE_info = new BattlePlateStruct();
		BATTLEPLATE_info.BattlePlateCoords = new BattlePlate_Class[(int)WS[WorldLevel-1]["size_x"],(int)WS[WorldLevel-1]["size_y"]];
		BATTLEPLATE_info.BattlePlateLevel = WorldLevel; 

		//배틀플레이트 좌표 변수 생성,월드 크기 초기화
		//BattlePlate_Class[,] BATTLEPLATE_coords = new BattlePlate_Class
		//[(int)WS[WorldLevel-1]["size_x"],(int)WS[WorldLevel-1]["size_y"]];
		
		for(int x = 0; x < (int)WS[WorldLevel-1]["size_x"]; x++)
		{
			for(int y = 0; y < (int)WS[WorldLevel-1]["size_y"]; y++)
			{
				if(y == 0||y==1)//첫번쨰와 두번째 줄까지 일반 플레이트 생성
				{
					int BR = Random.Range(26,29);
					BATTLEPLATE_info.BattlePlateCoords[x,y] = new BattlePlate_Class(3,BR);
				}

				else
				{
					int R = Random.Range(1,100);
					
					//전략 구조물 - 4% 
					if(R < 4)
					{
						int BR = Random.Range(9,12);
						BATTLEPLATE_info.BattlePlateCoords[x,y] = new BattlePlate_Class(1,BR);
					}
					//은폐 구조물 - 6%
					else if(R >= 4&&R < 10)
					{
						int BR = Random.Range(0,3);
						BATTLEPLATE_info.BattlePlateCoords[x,y] = new BattlePlate_Class(0,BR);
					}
					//일반 구조물 - 15%
					else if(R >= 10&&R < 25)
					{
						int BR = Random.Range(18,26);
						BATTLEPLATE_info.BattlePlateCoords[x,y] = new BattlePlate_Class(2,BR);
					}
					//장애물 - 15%
					else if(R >= 25&&R < 50)
					{
						int BR = Random.Range(29,32);
						BATTLEPLATE_info.BattlePlateCoords[x,y] = new BattlePlate_Class(4,BR);
					}
					//일반 플레이트 - 50%
					else if(R >= 50&&R < 100)
					{
						int BR = Random.Range(26,29);
						BATTLEPLATE_info.BattlePlateCoords[x,y] = new BattlePlate_Class(3,BR);
					}
				}
			}
		}
		
		//제이슨 세이브파일 생성
		JsonData data = JsonMapper.ToJson(BATTLEPLATE_info);
		File.WriteAllText(Application.dataPath + "/Data/MapCoords_save" + "/wpc" 
		+ "_" + BP_x.ToString()
		+ "_" + BP_y.ToString() + ".json" , data.ToString());
	}

	//배틀플레이트(주택가)) 좌표 제이슨파일 생성기    입력값 : 월드 플레이트 위험도 레벨 , 월드플레이트 X,Y좌표
	public /*BattlePlate_Class[,]*/void BattlePlateCoord_DownTown_GeneratorActual(int WorldLevel,int BP_x,int BP_y)
	{
		//배틀플레이트 사이즈 제이슨 파일로 부터 불러오기
		string worldsize = File.ReadAllText(Application.dataPath +"/Data/MapSize.json");
		JsonData WS = JsonMapper.ToObject(worldsize);

		//배틀플레이트 좌표 변수 생성,월드 크기 초기화,월드 레벨 할당
		BattlePlateStruct BATTLEPLATE_info = new BattlePlateStruct();
		BATTLEPLATE_info.BattlePlateCoords = new BattlePlate_Class[(int)WS[WorldLevel-1]["size_x"],(int)WS[WorldLevel-1]["size_y"]];
		BATTLEPLATE_info.BattlePlateLevel = WorldLevel; 

		//배틀플레이트 좌표 변수 생성,월드 크기 초기화
		//BattlePlate_Class[,] BATTLEPLATE_coords = new BattlePlate_Class
		//[(int)WS[WorldLevel-1]["size_x"],(int)WS[WorldLevel-1]["size_y"]];

		for(int x = 0; x < (int)WS[WorldLevel-1]["size_x"]; x++)
		{
			for(int y = 0; y < (int)WS[WorldLevel-1]["size_y"]; y++)
			{
				if(y == 0||y==1)//첫번쨰와 두번째 줄까지 일반 플레이트 생성
				{
					int BR = Random.Range(26,29);
					BATTLEPLATE_info.BattlePlateCoords[x,y] = new BattlePlate_Class(3,BR);
				}

				else
				{
					int R = Random.Range(1,100);
					
					//전략 구조물 - 4% 
					if(R < 4)
					{
						int BR = Random.Range(12,15);
						BATTLEPLATE_info.BattlePlateCoords[x,y] = new BattlePlate_Class(1,BR);
					}
					//은폐 구조물 - 6%
					else if(R >= 4&&R < 10)
					{
						int BR = Random.Range(3,9);
						BATTLEPLATE_info.BattlePlateCoords[x,y] = new BattlePlate_Class(0,BR);
					}
					//일반 구조물 - 15%
					else if(R >= 10&&R < 25)
					{
						int BR = Random.Range(21,26);
						BATTLEPLATE_info.BattlePlateCoords[x,y] = new BattlePlate_Class(2,BR);
					}
					//장애물 - 15%
					else if(R >= 25&&R < 50)
					{
						int BR = Random.Range(29,32);
						BATTLEPLATE_info.BattlePlateCoords[x,y] = new BattlePlate_Class(4,BR);
					}
					//일반 플레이트 - 50%
					else if(R >= 50&&R < 100)
					{
						int BR = Random.Range(26,29);
						BATTLEPLATE_info.BattlePlateCoords[x,y] = new BattlePlate_Class(3,BR);
					}
				}
			}
		}
		
		//제이슨 세이브파일 생성
		JsonData data = JsonMapper.ToJson(BATTLEPLATE_info);
		File.WriteAllText(Application.dataPath + "/Data/MapCoords_save" + "/wpc" 
		+ "_" + BP_x.ToString()
		+ "_" + BP_y.ToString() + ".json" , data.ToString());
	}

	//배틀플레이트(도시) 좌표 제이슨파일 생성기     입력값 : 월드 플레이트 위험도 레벨 , 월드플레이트 X,Y좌표
	public /*BattlePlate_Class[,]*/void BattlePlateCoord_City_GeneratorActual(int WorldLevel,int BP_x,int BP_y)
	{
		//배틀플레이트 사이즈 제이슨 파일로 부터 불러오기
		string worldsize = File.ReadAllText(Application.dataPath +"/Data/MapSize.json");
		JsonData WS = JsonMapper.ToObject(worldsize);

		//배틀플레이트 좌표 변수 생성,월드 크기 초기화,월드 레벨 할당
		BattlePlateStruct BATTLEPLATE_info = new BattlePlateStruct();
		BATTLEPLATE_info.BattlePlateCoords = new BattlePlate_Class[(int)WS[WorldLevel-1]["size_x"],(int)WS[WorldLevel-1]["size_y"]];
		BATTLEPLATE_info.BattlePlateLevel = WorldLevel;

		//배틀플레이트 좌표 변수 생성,월드 크기 초기화
		//BattlePlate_Class[,] BATTLEPLATE_coords = new BattlePlate_Class
		//[(int)WS[WorldLevel-1]["size_x"],(int)WS[WorldLevel-1]["size_y"]];

		for(int x = 0; x < (int)WS[WorldLevel-1]["size_x"]; x++)
		{
			for(int y = 0; y < (int)WS[WorldLevel-1]["size_y"]; y++)
			{
				if(y == 0||y==1)//첫번쨰와 두번째 줄까지 일반 플레이트 생성
				{
					int BR = Random.Range(26,29);
					BATTLEPLATE_info.BattlePlateCoords[x,y] = new BattlePlate_Class(3,BR);
				}

				else
				{
					int R = Random.Range(1,100);
					
					//전략 구조물 - 4% 
					if(R < 4)
					{
						int BR = Random.Range(12,18);
						BATTLEPLATE_info.BattlePlateCoords[x,y] = new BattlePlate_Class(1,BR);
					}
					//은폐 구조물 - 6%
					else if(R >= 4&&R < 10)
					{
						int BR = Random.Range(6,9);
						BATTLEPLATE_info.BattlePlateCoords[x,y] = new BattlePlate_Class(0,BR);
					}
					//일반 구조물 - 15%
					else if(R >= 10&&R < 25)
					{
						int BR = Random.Range(23,26);
						BATTLEPLATE_info.BattlePlateCoords[x,y] = new BattlePlate_Class(2,BR);
					}
					//장애물 - 15%
					else if(R >= 25&&R < 50)
					{
						int BR = Random.Range(29,32);
						BATTLEPLATE_info.BattlePlateCoords[x,y] = new BattlePlate_Class(4,BR);
					}
					//일반 플레이트 - 50%
					else if(R >= 50&&R < 100)
					{
						int BR = Random.Range(26,29);
						BATTLEPLATE_info.BattlePlateCoords[x,y] = new BattlePlate_Class(3,BR);
					}
				}
			}
		}
		
		//제이슨 세이브파일 생성
		JsonData data = JsonMapper.ToJson(BATTLEPLATE_info);
		File.WriteAllText(Application.dataPath + "/Data/MapCoords_save" + "/wpc" 
		+ "_" + BP_x.ToString()
		+ "_" + BP_y.ToString() + ".json" , data.ToString());
	}

}