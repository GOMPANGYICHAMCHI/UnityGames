using UnityEngine;

public class Unit_List : UnitList_Manager 
{	
	//기존 세이브 파일이 있을경우, UNITLIST_loader()
	//기존 세이브 파일이 없을경우, UNITLIST_GeneratorActual()

	//적오브젝트
	public Transform EnemyObject;
	
	//현재 배틀플레이트 정보
	public BattlePlateStruct CurrentBattlePlate;

	//유닛 오브젝트 리스트
	public Transform[] UnitObjList;

	//유닛리스트 구조체
	//유닛리스트 , 유닛카운트 포함
	public UnitListStruct UnitListInfo;

	//적중 한기의 유닛이라도 발각 되었는지 확인하는 메서드
	public bool CheckEnemyDetected()
	{
		//반환할 bool 값 / 기본값 : false
		bool ReturnBool = false;

		//전체 적 유닛중 단하나라도 발각 되었을 경우, true 값 반환
		for(int i = 4 ; i < UnitListInfo.UNITCOUNT ; i++)
		{
			if(UnitListInfo.UNITINFO[i].detected == true)
			{
				ReturnBool = true;
			}
		}

		return ReturnBool;
	}

	//입력받은 유닛인덱스에 해당하는 아군이 현재 최대 장전 탄약인지 확인하는 메서드
	public bool CheckFriendlyFullLoaded(int UnitIndex)
	{
		//반환할 bool 값 / 기본값 : false
		bool ReturnBool = false;

		//최대장약 인지 확인
		if((int)PlayerAndroid_status[UnitIndex]["ActualLoadedBullet"] == (int)PlayerAndroid_status[UnitIndex]["ReloadedBullets"])
		{
			ReturnBool = true;
		}

		return ReturnBool;
	}

	//현재 선택한 안드로이드 재장전 메서드
	public void FriendlyReloadActual(int UnitIndex)
	{
		//장전 해야할 탄약수  (장전가능한 탄약수) - (현재 장전된 탄약수)
		int ActualLoadMounts = 
		(int)PlayerAndroid_status[UnitIndex]["ReloadedBullets"] - (int)PlayerAndroid_status[UnitIndex]["ActualLoadedBullet"];

		//만약 현재 전체 탄약수가 장전해야할 탄약보다 적을경우,
		if((int)PlayerAndroid_status[UnitIndex]["CurrentBullet"] < ActualLoadMounts)
		{
			//현재 장전된 탄약수 = 장전된 탄약수 + 장전해야할 탄약수
			int ActualLoadedBullet = (int)PlayerAndroid_status[UnitIndex]["ActualLoadedBullet"];
			ActualLoadedBullet += ActualLoadMounts;
			PlayerAndroid_status[UnitIndex]["ActualLoadedBullet"] = ActualLoadedBullet;

			//전체 탄약수 = 0
			PlayerAndroid_status[UnitIndex]["CurrentBullet"] = 0;
		}

		//아닐경우 (현재 전체 탄약수가 장전가능한 탄약보다 많을경우,)
		else
		{
			//장전된 탄약수 = 장전가능한 탄약수
			PlayerAndroid_status[UnitIndex]["ActualLoadedBullet"] = (int)PlayerAndroid_status[UnitIndex]["ReloadedBullets"];

			//전체 탄약수 - 장전해야할 탄약수
			int MaxBullet = (int)PlayerAndroid_status[UnitIndex]["CurrentBullet"];
			MaxBullet -= ActualLoadMounts;
			PlayerAndroid_status[UnitIndex]["CurrentBullet"] = MaxBullet;
		}

		//안드로이드 스탯 세이브
		PlayerAndroidStat_save();
	}

	//아군 공격시 데미지 계산기
	public int[] FriendlyAttackDamageCalculator(int EnemyIndex,int FriendlyIndex,bool isEnemyinBuilding)
	{
		//발사할 탄환수
		int RoundMount;
		//기습인지 확인하는 변수
		bool isSurpriseShot;

		//발각 상태할당
		isSurpriseShot = !UnitListInfo.UNITINFO[EnemyIndex].detected;
		
		//장전된 탄약이 턴당발사수 보다 적을 경우,
		if((int)PlayerAndroid_status[FriendlyIndex]["ActualLoadedBullet"] < (int)PlayerAndroid_status[FriendlyIndex]["RPT"])
		{
			//장전된 탄약수를 발사할 탄약수에 할당
			RoundMount = (int)PlayerAndroid_status[FriendlyIndex]["ActualLoadedBullet"];
			//Debug.Log("발사할 탄약 부족 잔탄을 전부 발사합니다");
		}
		
		//장전된 탄약이 턴당발사수 보다 많거나 같을때,
		else
		{
			//턴당 발사수를 발사할 탄약수에 할당
			RoundMount = (int)PlayerAndroid_status[FriendlyIndex]["RPT"];
			//Debug.Log("턴당 발사수 만큼 발사합니다");
		}

		//턴당 발사수 만큼 데미지 배열 크기 할당
		int[] Damage = new int[RoundMount];

		//무기 종류에따른 데미지판별 분리
		switch ((string)PlayerAndroid_status[FriendlyIndex]["Type"])
		{
			//돌격 소총 / 구조물 효과 (받음) / 장갑 (비관통)
			case "AR":
				//계산 완료된 할당용 데미지
				int FinalDamage_1 = (int)PlayerAndroid_status[FriendlyIndex]["DPB"] 			//안드로이드 데미지
				- (int)Enemy_status[UnitListInfo.UNITINFO[EnemyIndex].Index]["Armor"];		//장갑

				//적이 건물에 있다면
				if(isEnemyinBuilding)
				{
					FinalDamage_1 -= (int)PlayerAndroid_status[FriendlyIndex]["DPB"]/2;		//건물 감소도
				}	
				//만약 기습 일경우
				if(isSurpriseShot)
				{
					FinalDamage_1 = FinalDamage_1 * ((int)PlayerAndroid_status[FriendlyIndex]["SurpriseShotDamage"]/100); //기습율 추가
				}

				for(int i = 0 ; i < Damage.Length ; i++)
				{
					Damage[i] = FinalDamage_1;			
				}
				//갱신된 탄약수 할당
				PlayerAndroid_status[FriendlyIndex]["ActualLoadedBullet"] =
				//현재 장전된 탄약수 - 턴당 소모량
				(int)PlayerAndroid_status[FriendlyIndex]["ActualLoadedBullet"] - RoundMount;
					
				//안드로이드 스탯 세이브
				PlayerAndroidStat_save();

				break;
			//저격 소총 / 구조물 효과 (무시) / 장갑 (관통)
			// 특수효과 : 관통
			case "SR":
				//계산 완료된 할당용 데미지
				int FinalDamage_2 = (int)PlayerAndroid_status[FriendlyIndex]["DPB"];

				//만약 기습 일경우
				if(isSurpriseShot)
				{
					FinalDamage_2 = FinalDamage_2 * ((int)PlayerAndroid_status[FriendlyIndex]["SurpriseShotDamage"]/100); //기습율 추가
				}

				for(int i = 0 ; i < Damage.Length ; i++)
				{
					Damage[i] = FinalDamage_2;
				}
				//갱신된 탄약수 할당
				PlayerAndroid_status[FriendlyIndex]["ActualLoadedBullet"] =
				//현재 장전된 탄약수 - 턴당 소모량
				(int)PlayerAndroid_status[FriendlyIndex]["ActualLoadedBullet"] - RoundMount;
					
				//안드로이드 스탯 세이브
				PlayerAndroidStat_save();

				break;
			//경기관총 / 구조물 효과 (받음) / 장갑 (비관통)
			//특수효과 : 감나빗
			case "LMG":
				//코드확률
				int HR = Random.Range(0,100);
				int FinalDamage_3 = (int)PlayerAndroid_status[FriendlyIndex]["DPB"] 			//안드로이드 데미지
				- (int)Enemy_status[UnitListInfo.UNITINFO[EnemyIndex].Index]["Armor"];			//장갑
				//적이 건물에 있다면
				if(isEnemyinBuilding)
				{
					FinalDamage_3 -= (int)PlayerAndroid_status[FriendlyIndex]["DPB"]/2;		//건물 감소도
				}

				//만약 기습 일경우
				if(isSurpriseShot)
				{
					FinalDamage_3 = FinalDamage_3 * ((int)PlayerAndroid_status[FriendlyIndex]["SurpriseShotDamage"]/100); //기습율 추가
				}

				//명중률
				int Accuracy = (int)PlayerAndroid_status[FriendlyIndex]["Accuracy"];

				//코드 확률보다 명줄률이 높으면 , 안정적 코드
				if(HR < (int)PlayerAndroid_status[FriendlyIndex]["Accuracy"])
				{
					for(int i = 0 ; i < Damage.Length ; i++)
					{
						int R = Random.Range(0,100);
						if(R <= Accuracy)
						{
							Damage[i] = FinalDamage_3;		
						}
						else
						{
							//빗나감
							Damage[i] = 0;
						}
					}
				}

				//코드 확률보다 명중률이 낮으면 , 극단적 코드
				else
				{
					//최대 탄약수 할당
					int LMG_Maxbullet = Random.Range(1,RoundMount);
					for(int i = 0 ; i < Damage.Length ; i++)
					{
						if(Damage.Length - i == LMG_Maxbullet)
						{
							Damage[i] = FinalDamage_3;
							LMG_Maxbullet -=1;
						}
						else
						{
							int R = Random.Range(0,100);
							if(R <= Accuracy)
							{
								Damage[i] = FinalDamage_3;
								LMG_Maxbullet -=1;		
							}
							else
							{
								//빗나감
								Damage[i] = 0;
							}
						}
					}
				}
				
				break;
			// 샷건 / 구조물 효과 (받음) / 장갑 (비관통)
			// 특수효과 : 벅샷
			case "SG":
				//계산 완료된 할당용 데미지
				int FinalDamage_4 = ((int)PlayerAndroid_status[FriendlyIndex]["DPB"]/8)		//안드로이드 데미지
				- (int)Enemy_status[UnitListInfo.UNITINFO[EnemyIndex].Index]["Armor"];		//장갑

				//적이 건물에 있다면
				if(isEnemyinBuilding)
				{
					FinalDamage_4 -= (int)PlayerAndroid_status[FriendlyIndex]["DPB"]/8/2;	//건물 감소도
				}

				//만약 기습 일경우
				if(isSurpriseShot)
				{
					FinalDamage_4 = FinalDamage_4 * ((int)PlayerAndroid_status[FriendlyIndex]["SurpriseShotDamage"]/100); //기습율 추가
				}

				for(int i = 0 ; i < Damage.Length ; i++)
				{
					//데미지 0으로 초기화
					Damage[i] = 0;
					//펠릿 수만큼 for문
					for(int a = 0 ; a < 8 ; a++ )
					{
						Damage[i] += FinalDamage_4;
					}		
				}
				//갱신된 탄약수 할당
				PlayerAndroid_status[FriendlyIndex]["ActualLoadedBullet"] =
				//현재 장전된 탄약수 - 턴당 소모량
				(int)PlayerAndroid_status[FriendlyIndex]["ActualLoadedBullet"] - RoundMount;
					
				//안드로이드 스탯 세이브
				PlayerAndroidStat_save();

				break;
			//지정 사수 소총/ 구조물 효과 (받음) / 장갑 (비관통)
			// 특수효과 비장전
			case "DMR":
				//계산 완료된 할당용 데미지
				int FinalDamage_5 = (int)PlayerAndroid_status[FriendlyIndex]["DPB"] 			//안드로이드 데미지
				- (int)Enemy_status[UnitListInfo.UNITINFO[EnemyIndex].Index]["Armor"];			//장갑

				//적이 건물에 있다면
				if(isEnemyinBuilding)
				{
					FinalDamage_5 -= (int)PlayerAndroid_status[FriendlyIndex]["DPB"]/2;		//건물 감소도
				}

				//만약 기습 일경우
				if(isSurpriseShot)
				{
					FinalDamage_5 = FinalDamage_5 * ((int)PlayerAndroid_status[FriendlyIndex]["SurpriseShotDamage"]/100); //기습율 추가
				}

				for(int i = 0 ; i < Damage.Length ; i++)
				{
					Damage[i] = FinalDamage_5;
				}
				//갱신된 탄약수 할당 (장전된 탄약)
				PlayerAndroid_status[FriendlyIndex]["ActualLoadedBullet"] =
				//현재 장전된 탄약수 - 턴당 소모량
				(int)PlayerAndroid_status[FriendlyIndex]["ActualLoadedBullet"] - RoundMount;
				
				//안드로이드 스탯 세이브
				PlayerAndroidStat_save();

				break;
			//기관단총/ 구조물 효과 (받음) / 장갑 (비관통)
			case "SMG":
				//계산 완료된 할당용 데미지
				int FinalDamage_6 = (int)PlayerAndroid_status[FriendlyIndex]["DPB"] 			//안드로이드 데미지
				- (int)Enemy_status[UnitListInfo.UNITINFO[EnemyIndex].Index]["Armor"];			//장갑

				//적이 건물에 있다면
				if(isEnemyinBuilding)
				{
					FinalDamage_6 -= (int)PlayerAndroid_status[FriendlyIndex]["DPB"]/2;		//건물 감소도
				}

				//만약 기습 일경우
				if(isSurpriseShot)
				{
					FinalDamage_6 = FinalDamage_6 * ((int)PlayerAndroid_status[FriendlyIndex]["SurpriseShotDamage"]/100); //기습율 추가
				}

				for(int i = 0 ; i < Damage.Length ; i++)
				{
					Damage[i] = FinalDamage_6;
				}
				//갱신된 탄약수 할당
				PlayerAndroid_status[FriendlyIndex]["ActualLoadedBullet"] =
				//현재 장전된 탄약수 - 턴당 소모량
				(int)PlayerAndroid_status[FriendlyIndex]["ActualLoadedBullet"] - RoundMount;
					
				//안드로이드 스탯 세이브
				PlayerAndroidStat_save();

				break;
		}

		return Damage;
	}

	//적이 건물안에 있는지 확인하는 메서드
	public bool CheckEnemyInBuilding(int TargetEnemyIndex)
	{
		//반환할 bool값 건물안에 있다면 참
		bool isInBuilding;
		//적이위치한 플레이트의 건물인덱스
		int TargetPlateBuildingIndex =
		CurrentBattlePlate.BattlePlateCoords
		[UnitListInfo.UNITINFO[TargetEnemyIndex].Coord_X,UnitListInfo.UNITINFO[TargetEnemyIndex].Coord_Y].Building_Index;

		//만약 일반플레이트에 있을경우,
		if(TargetPlateBuildingIndex != 26||TargetPlateBuildingIndex != 27||TargetPlateBuildingIndex != 28)
		{
			isInBuilding = false;
		}
		//만약 건물안에 있을경우,
		else
		{
			isInBuilding = true;
		}

		return isInBuilding;
	}
}