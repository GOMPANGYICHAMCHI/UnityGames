//using UnityEngine;

//배틀플레이트 클래스
public class BattlePlate_Class
{
	//배틀 플레이트 지정형식 /0 : 은폐구조물/  1 : 전략구조물/  2 : 일반구조물/  3 : 일반플레이트/  4 : 장애물/
	public int BattlePlate_Type;

	//건물 인덱스 
	// 농장 0 / 1 / 2           대형 마트 3 / 4 / 5
	// 은행 6 / 7 / 8           급수탑 9 / 10/ 11
	// 전망대 12 / 13 / 14      고층빌딩 15 / 16 / 17 
	// 나무 18 / 19 / 20        주유소  21 / 22
	// 저층주택 23 / 24 / 25     일반 플레이트 26 / 27 / 28
	//	장애물 29 / 30 / 31
	public int Building_Index;

	//public int Obj_Index;

	public BattlePlate_Class(int type,int building_index)
	{
		BattlePlate_Type = type;
		Building_Index = building_index;
	}
}

//배틀플레이트 저장 내용
//전체유닛 수 //배틀플레이트의 월드상 좌표 //월드레벨
public class LastBattlePlateInfo
{
	//전체 유닛수
	int UnitCount;

	//배틀플레이트의 월드상 좌표
	int[] BattlePlateCoord;

	//월드레벨
	int WorldLevel;
}

//배틀플레이트 구조체 
//배틀플레이트 클래스 //배틀플레이트 레벨
public struct BattlePlateStruct
{	
	//배틀플레이트 클래스
	public BattlePlate_Class[,] BattlePlateCoords;

	//배틀플레이트 레벨
	public int BattlePlateLevel;
};

//월드플레이트 에서 배틀플레이트로 좌표를 전달할때 사용하는 구조체
public struct Selected_BattlePlateCoord
{
	//월드플레이트상의 배틀플레이트 x좌표
	public int X;

	//월드플레이트상의 배틀플레이트 y좌표
	public int Y;
}
