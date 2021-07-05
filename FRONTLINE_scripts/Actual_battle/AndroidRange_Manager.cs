//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

//콜라이더 범위 타입
public enum RangeType
{
	//범위 타입 : 탐지범위
	IdentificationDistance,
	//범위 타입 : 공격범위
	AttackRange,
	//범위 타입 : 가시거리
	VisibleRange,
	//범위타입 : 선택된 영역
	SelectedRange
};

public class AndroidRange_Manager : MonoBehaviour 
{
	public RangeType THISRangeType = RangeType.AttackRange;

	public Phase_Manager PM;

	//입력인지 출력인지를 정합니다.
	public bool Input = true;

	void Start() 
	{
		PM = GameObject.FindObjectOfType<Phase_Manager>();
	}

	void OnTriggerEnter(Collider other) 
	{
		//범위 표시 일경우,
		if(Input)
		{
			//범위 타입이 탐지범위 일경우,
			if(THISRangeType == RangeType.IdentificationDistance)
			{
				//해당 오브젝트가 배틀플레이트 일경우,
				if(other.gameObject.tag == "Enemy")
				{
					int EnemyIndex;

					//적 오브젝트 활성화
					other.gameObject.transform.GetChild(2).gameObject.SetActive(true);
					other.gameObject.transform.GetChild(3).gameObject.SetActive(true);
					//인덱스 임시 할당
					EnemyIndex = other.transform.GetChild(2).gameObject.GetComponent<Unit_tag>().Index;
					//적 발각상태로 전환
					PM.UnitListInfo.UNITINFO[EnemyIndex].detected = true;
				}
			}
			
			//범위 타입이 공격범위 일경우,
			else if(THISRangeType == RangeType.AttackRange)
			{
				//해당 오브젝트가 배틀플레이트 일경우,
				if(other.gameObject.tag == "BattlePlate")
				{
					//해당 오브젝트 범위화 활성화
					other.gameObject.GetComponent<PlateIndividual_manager>().StateToAttackRange();
				}
			}

			//범위 타입이 가시거리 일경우,
			else if(THISRangeType == RangeType.VisibleRange)
			{
				//해당 오브젝트가 배틀플레이트 일경우,
				if(other.gameObject.tag == "BattlePlate")
				{
					//해당 오브젝트 범위화 활성화
					other.gameObject.GetComponent<PlateIndividual_manager>().StateToRange();
				}
			}

			//범위 타입이 선택된 구역 일경우,
			else if(THISRangeType == RangeType.SelectedRange)
			{
				//해당 오브젝트가 배틀플레이트 일경우,
				if(other.gameObject.tag == "BattlePlate")
				{
					//해당 오브젝트 범위화 활성화
					other.gameObject.GetComponent<PlateIndividual_manager>().StateToSelectedRange();
				}
			}
		}
		
		//범위 초기화 일경우,
		else
		{
			//범위 타입이 공격범위 일경우 , 범위 타입이 가시거리 일경우 , 범위 타입이 선택된 구역 일경우
			if(THISRangeType == RangeType.AttackRange||THISRangeType == RangeType.VisibleRange)
			{
				//해당 오브젝트가 배틀플레이트 일경우,
				if(other.gameObject.tag == "BattlePlate")
				{
					//오브젝트 할당 초기화
					other.gameObject.GetComponent<PlateIndividual_manager>().StateToNomal();
				}
			}

			else if(THISRangeType == RangeType.SelectedRange)
			{
				//해당 오브젝트가 배틀플레이트 일경우,
				if(other.gameObject.tag == "BattlePlate")
				{
					//오브젝트 할당 초기화
					other.gameObject.GetComponent<PlateIndividual_manager>().IsRangeNow();
				}
			}

			//범위 타입이 가시거리 일경우,
			/*else if(THISRangeType == RangeType.VisibleRange)
			{
				//해당 오브젝트가 배틀플레이트 일경우,
				if(other.gameObject.tag == "BattlePlate")
				{
					//오브젝트 할당 초기화
					other.gameObject.GetComponent<PlateIndividual_manager>().StateToNomal();
				}
			}

			//범위 타입이 선택된 구역 일경우,
			else if(THISRangeType == RangeType.SelectedRange)
			{
				//해당 오브젝트가 배틀플레이트 일경우,
				if(other.gameObject.tag == "BattlePlate")
				{
					//해당 오브젝트 범위화 활성화
					other.gameObject.GetComponent<PlateIndividual_manager>().StateToNomal();
				}
			}*/
		}
	}
}
