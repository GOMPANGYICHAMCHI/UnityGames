using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

// 아군 오브젝트 0번 자식 : IdentificationDistance / 1번 자식 : VisibleRange / 2번 자식 : AttackRange
public enum SystemState
{
	System_Idle,  			//일반
	System_Attack, 			//공격
	System_Boundary, 		//경계
	System_AirSupport, 		//공중지원
	System_Reload, 			//재장전
	System_Move, 			//이동
	System_Scan 			//스캔
}

public class ActingSystem_Manager : PhaseFriendlyAct_Manager 
{
	//행동취소/행동확정 버튼 패널
	public GameObject ActingSure_panel;
	
	//행동확정 버튼
	public GameObject ActingSure_btn;

	//행동취소 버튼
	public GameObject ActingCancel_btn;

	//이동종료 버튼
	public GameObject ActMoveExit_btn;

	//이동사거리 오브젝트
	public GameObject Move_Range;

	//경계 사거리 오브젝트
	public GameObject Boundary_Range;

	//선택된 플레이트
	public GameObject SelectedPlate;
	
	//현재 유저가 밟고 있는 플레이트
	public GameObject CurrentPlate;

	//선택된 적 오브젝트
	public GameObject SelectedEnemy;

	//시스템 상태 (기본값 : 일반으로 설정)
	public SystemState SYSTEMSTATE_Actual = SystemState.System_Idle;

	//메인 카메라
	//public Camera MainCam_camera;

	//행동이 끝났는지 확인하는 변수
	public bool ActFinish = false;

	//코스트를 사용했는지 확인하는 변수
	public bool UsedCost = false;

	//-------------
	//이동 포인트 (속도/2)
	int movepoint;

	//-------------

	void Update() 
	{
		//시스템이 일반이 아닐때
		if(SYSTEMSTATE_Actual != SystemState.System_Idle)
		{
			//마우스 버튼(0) 입력시
			if(Input.GetButtonDown("Fire1"))
			{
				//현재 마우스 위치로 레이캐스트 출력
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;

				//해당 물체가 배틀플레이트 일경우,
				if(Physics.Raycast(ray ,out hit)&&hit.transform.gameObject.tag == "BattlePlate")
				{
					//해당 배틀플레이트의 상태가 선택불가가 아닐때,
					if(hit.transform.gameObject.GetComponent<PlateIndividual_manager>().UnTouchable != true)
					{
						switch (SYSTEMSTATE_Actual)
						{
							//공중 지원 일때,
							case SystemState.System_AirSupport:
								if(SelectedPlate != null)
								{
									//선택된 타일 상태 초기화
									SelectedPlate.GetComponent<PlateIndividual_manager>().StateToRange();
								}
								//레이에 맞은 타일 현재 선택 타일로 할당
								SelectedPlate = hit.transform.gameObject;
								//선택된 타일 선택 상태로 변경
								hit.transform.gameObject.GetComponent<PlateIndividual_manager>().StateToSelectedRange();
								//행동 확정 버튼 텍스트 할당
								PUM.SituationSureBtnText_loader(4);
								//행동 확정 버튼 활성화
								ActingSure_btn.SetActive(true);
								break;

							//경계 일때,
							case SystemState.System_Boundary:
								StartCoroutine(BoundaryAllMethod(hit.transform));
								break;

							//이동 일때,
							case SystemState.System_Move:
								if(SelectedPlate != null)
								{
									//선택된 타일 상태 초기화
									SelectedPlate.GetComponent<PlateIndividual_manager>().StateToRange();
								}
								//레이에 맞은 타일 현재 선택 타일로 할당
								SelectedPlate = hit.transform.gameObject;
								//선택된 타일 선택 상태로 변경
								hit.transform.gameObject.GetComponent<PlateIndividual_manager>().StateToSelectedRange();
								//행동 확정 버튼 텍스트 할당
								PUM.SituationSureBtnText_loader(3);
								//행동 확정 버튼 활성화
								ActingSure_btn.SetActive(true);
								break;
						}
					}
				}

				//해당 물체가 적 일경우,
				else if(Physics.Raycast(ray ,out hit)&&hit.transform.gameObject.tag == "Enemy_A")
				{
					switch (SYSTEMSTATE_Actual)
					{
						//공격 일때,
						case SystemState.System_Attack:
							//선택된 적 오브젝트가 초기 상태가 아니라면
							if(SelectedEnemy != null)
							{
								//유닛 락온 UI off
								SelectedEnemy.transform.parent.GetChild(3).transform.GetChild(5).gameObject.SetActive(false);
							}
							//레이에 맞은 적 현재 선택 적 오브젝트로 할당
							SelectedEnemy = hit.transform.gameObject;
							//유닛 락온 UI on
							SelectedEnemy.transform.parent.GetChild(3).transform.GetChild(5).gameObject.SetActive(true);
							//행동 확정 버튼 텍스트 할당
							PUM.SituationSureBtnText_loader(0);
							//행동 확정 버튼 활성화
							ActingSure_btn.SetActive(true);
							break;

						//스캔 일때,
						case SystemState.System_Scan:
							//레이에 맞은 적 현재 선택 적 오브젝트로 할당
							SelectedEnemy = hit.transform.gameObject;
							//행동 확정 버튼 텍스트 할당
							PUM.SituationSureBtnText_loader(2);
							//행동 확정 버튼 활성화
							ActingSure_btn.SetActive(true);
							break;
					}
				}
			}
		}
	}

	//경계 범위 활성화/비활성화 메서드
	public IEnumerator BoundaryRange_on_off(bool on)
	{
		//입력값 할당
		if(on)Boundary_Range.GetComponent<AndroidRange_Manager>().Input = true;
		else Boundary_Range.GetComponent<AndroidRange_Manager>().Input = false;

		//경계 범위 활성화
		Boundary_Range.SetActive(true);
		yield return new WaitForSeconds(0.1f);
		Boundary_Range.SetActive(false);
	}

	//경계 범위 실제 작동 경계로 바꾸는 메서드
	public IEnumerator BoundaryActual()
	{
		Boundary_Range.GetComponent<AndroidRange_Manager>().Input = true;
		Boundary_Range.GetComponent<AndroidRange_Manager>().THISRangeType = RangeType.IdentificationDistance;

		//경계 범위 활성화
		Boundary_Range.SetActive(true);
		yield return new WaitForSeconds(0.1f);
		Boundary_Range.SetActive(false);

		Boundary_Range.GetComponent<AndroidRange_Manager>().THISRangeType = RangeType.SelectedRange;
	}

	//Update 함수용 경계 전체 메서드
	public IEnumerator BoundaryAllMethod(Transform HitGameobject)
	{
		//경계 범위 비활성화
		StartCoroutine(BoundaryRange_on_off(false));
		yield return new WaitForSeconds(0.2f);
		//경계 범위 오브젝트 현재 선택 타일 오브젝트 위치에 할당
		Boundary_Range.transform.position = HitGameobject.position;
		//경계 범위 활성화
		StartCoroutine(BoundaryRange_on_off(true));
		//행동 확정 버튼 텍스트 할당
		PUM.SituationSureBtnText_loader(1);
		//행동 확정 버튼 활성화
		ActingSure_btn.SetActive(true);
	}

	//이동 범위 활성화/비활성화 메서드
	public IEnumerator MoveRange_on_off(bool on)
	{
		//입력값 할당
		if(on)Move_Range.GetComponent<AndroidRange_Manager>().Input = true;
		else Move_Range.GetComponent<AndroidRange_Manager>().Input = false;

		//범위 활성화
		Move_Range.SetActive(true);
		yield return new WaitForSeconds(0.1f);
		Move_Range.SetActive(false);
	}

	//공격 버튼 클릭 호출
	public void AttackBtn_Pressed()
	{
		StartCoroutine(AttackBtn_Act());
	}
	//경계 버튼 클릭 호출
	public void BoundaryBtn_pressed()
	{
		StartCoroutine(BoundaryBtn_Act());
	}
	//공중지원 버튼 클릭 호출
	public void AirSupportBtn_pressed()
	{
		StartCoroutine(AirSupportBtn_Act());
	}
	//재장전 버튼 클릭 호출
	public void ReloadBtn_pressed()
	{
		StartCoroutine(ReloadBtn_Act());
	}
	//이동 버튼 클릭 호출
	public void MoveBtn_pressed()
	{
		StartCoroutine(MoveBtn_Act());
	}
	//스캔 버튼 클릭 호출
	public void ScanBtn_pressed()
	{
		StartCoroutine(ScanBtn_Act());
	}

	//공격 버튼 클릭 메서드 // 판단 코스트
	public IEnumerator AttackBtn_Act()
	{
		//동작 코스트 나 일반 코스트가 있는지 확인
		if(PM.JudgingCost != 0||PM.NomalCost != 0)
		{
			//현재 발각된 적이 있는지 확인
			if(PM.CheckEnemyDetected() == true)
			{
				//현재 선택된 아군이 잔탄이 존재하는지 확인
				if((int)PM.PlayerAndroid_status[PM.CurrentUnit.UnitIndex]["ActualLoadedBullet"] != 0)
				{
					//시스템 상태를 "공격" 으로 전환
					SYSTEMSTATE_Actual = SystemState.System_Attack;
					//현재 페이즈 유닛 오브젝트
					Transform CurrentFriendly_obj = PM.UnitObjList[PM.CurrentUnit.UnitIndex].transform;

					//범위 오브젝트 on // AttackRange
					GameObject range_obj = CurrentFriendly_obj.GetChild(2).gameObject;
					range_obj.GetComponent<AndroidRange_Manager>().Input = true;
					range_obj.gameObject.SetActive(true);
					yield return new WaitForSeconds(0.2f);
					range_obj.gameObject.SetActive(false);

					//아군 행동 패널 비활성화
					PUM.FriendlyTurnPanel.SetActive(false);
					//행동취소 버튼 패널 활성화
					ActingSure_panel.SetActive(true);

					ActFinish = false;
					while(true)
					{
						yield return null;
						//다음턴으로 넘길수 있을 경우, 종료
						if(ActFinish == true)
						{
							break;
						}
					}

					//Debug.Log("공격 종료");

					//아군 행동 패널 활성화
					PUM.FriendlyTurnPanel.SetActive(true);
					//배치확정 버튼 비활성화
					ActingSure_btn.SetActive(false);
					//행동취소 버튼 패널 비활성화
					ActingSure_panel.SetActive(false);

					range_obj.GetComponent<AndroidRange_Manager>().Input = false;
					range_obj.gameObject.SetActive(true);
					yield return new WaitForSeconds(0.2f);
					range_obj.gameObject.SetActive(false);

					//시스템 일반으로 전환
					SYSTEMSTATE_Actual = SystemState.System_Idle;

					//만약 코스트가 소모 되었을 경우,
					if(UsedCost)
					{
						//코스트 감소
						PM.CostDecrease_Actual(1);
					}
				}
				//잔탄이 없을경우,
				else
				{
					StartCoroutine(PUM.LogOutputActual(11));
				}
				
			}
			//현재 발각된 적이 없을 경우,
			else
			{
				StartCoroutine(PUM.LogOutputActual(1));
			}
		}
		//소모할 코스트가 모자랄 경우,
		else
		{
			StartCoroutine(PUM.LogOutputActual(2));
		}
	}

	//경계 버튼 클릭 메서드 // 행동코스트
	public IEnumerator BoundaryBtn_Act()
	{
		//행동 코스트나 일반코스트가 있는지 확인
		if(PM.ActingCost != 0||PM.NomalCost != 0)
		{
			UsedCost = false;
			//시스템 상태를 "경계" 으로 전환
			SYSTEMSTATE_Actual = SystemState.System_Boundary;
			//현재 페이즈 유닛 오브젝트
			Transform CurrentFriendly_obj = PM.UnitObjList[PM.CurrentUnit.UnitIndex].transform;

			//범위 오브젝트 on // VisibleRange
			GameObject range_obj = CurrentFriendly_obj.GetChild(1).gameObject;
			range_obj.GetComponent<AndroidRange_Manager>().Input = true;
			range_obj.gameObject.SetActive(true);
			yield return new WaitForSeconds(0.2f);
			range_obj.gameObject.SetActive(false);

			//경계범위 초기화
			ResetBoundaryRange_obj();

			//아군 행동 패널 비활성화
			PUM.FriendlyTurnPanel.SetActive(false);
			//행동취소 버튼 패널 활성화
			ActingSure_panel.SetActive(true);

			ActFinish = false;
			while(true)
			{
				yield return null;
				//다음턴으로 넘길수 있을 경우, 종료
				if(ActFinish == true)
				{
					break;
				}
			}

			StartCoroutine(BoundaryRange_on_off(false));

			//Debug.Log("경계 종료");

			//아군 행동 패널 활성화
			PUM.FriendlyTurnPanel.SetActive(true);
			//배치확정 버튼 비활성화
			ActingSure_btn.SetActive(false);
			//행동취소 버튼 패널 비활성화
			ActingSure_panel.SetActive(false);

			//시스템 일반으로 전환
			SYSTEMSTATE_Actual = SystemState.System_Idle;

			range_obj.GetComponent<AndroidRange_Manager>().Input = false;
			range_obj.gameObject.SetActive(true);
			yield return new WaitForSeconds(0.2f);
			range_obj.gameObject.SetActive(false);

			//만약 코스트가 소모 되었을 경우,
			if(UsedCost)
			{
				//경계 메서드 호출
				StartCoroutine(BoundaryActual());
				//코스트 감소
				PM.CostDecrease_Actual(0);
			}
		}
		//소모할 코스트가 모자랄 경우,
		else
		{
			StartCoroutine(PUM.LogOutputActual(2));
		}
	}

	//공중지원 버튼 클릭 메서드 // 판단코스트
	public IEnumerator AirSupportBtn_Act()
	{
		//판단 코스트나 일반코스트가 있는지 확인
		if(PM.JudgingCost != 0||PM.NomalCost != 0)
		{
			//시스템 상태를 "공중지원" 으로 전환
			SYSTEMSTATE_Actual = SystemState.System_AirSupport;
			//현재 페이즈 유닛 오브젝트
			Transform CurrentFriendly_obj = PM.UnitObjList[PM.CurrentUnit.UnitIndex].transform;

			//범위 오브젝트 on // VisibleRange
			GameObject range_obj = CurrentFriendly_obj.GetChild(1).gameObject;
			range_obj.GetComponent<AndroidRange_Manager>().Input = true;
			range_obj.gameObject.SetActive(true);
			yield return new WaitForSeconds(0.2f);
			range_obj.gameObject.SetActive(false);

			//아군 행동 패널 비활성화
        	PUM.FriendlyTurnPanel.SetActive(false);
			//행동취소 버튼 패널 활성화
			ActingSure_panel.SetActive(true);
			
			ActFinish = false;
			while(true)
        	{
				yield return null;
				//다음턴으로 넘길수 있을 경우, 종료
				if(ActFinish == true)
				{
					break;
				}
        	}

			//Debug.Log("공중지원 종료");

			//아군 행동 패널 활성화
       		PUM.FriendlyTurnPanel.SetActive(true);	
			//배치확정 버튼 비활성화
			ActingSure_btn.SetActive(false);
			//행동취소 버튼 패널 비활성화
			ActingSure_panel.SetActive(false);

			//시스템 일반으로 전환
			SYSTEMSTATE_Actual = SystemState.System_Idle;

			range_obj.GetComponent<AndroidRange_Manager>().Input = false;
			range_obj.gameObject.SetActive(true);
			yield return new WaitForSeconds(0.2f);
			range_obj.gameObject.SetActive(false);

			//만약 코스트가 소모 되었을 경우,
			if(UsedCost)
			{
				//코스트 감소
				PM.CostDecrease_Actual(1);
			}
		}
		//소모할 코스트가 모자랄 경우,
		else
		{
			StartCoroutine(PUM.LogOutputActual(2));
		}
	}
	
	//재장전 버튼 클릭 메서드 // 판단 코스트
	public IEnumerator ReloadBtn_Act()
	{
		//판단 코스트나 일반코스트가 있는지 확인
		if(PM.JudgingCost != 0||PM.NomalCost != 0)
		{
			//현재 장전된 탄약수가 최대 탄약수가 아닐경우,
			if(!PM.CheckFriendlyFullLoaded(PM.CurrentUnit.UnitIndex))
			{
				//만약 해당 안드로이드의 총기가 DMR 이 아닐경우,
				if((string)PM.PlayerAndroid_status[PM.CurrentUnit.UnitIndex]["Type"] != "DMR")
				{
					//만약 현재 소지 탄약이 없을경우,
					if((int)PM.PlayerAndroid_status[PM.CurrentUnit.UnitIndex]["CurrentBullet"] == 0)
					{
						StartCoroutine(PUM.LogOutputActual(5));
					}
					//재장전할 탄약이 남아 있을경우,
					else
					{
						//시스템 상태를 "재장전" 으로 전환
						SYSTEMSTATE_Actual = SystemState.System_Reload;
						//현재 페이즈 유닛 오브젝트
						Transform CurrentFriendly_obj = PM.UnitObjList[PM.CurrentUnit.UnitIndex].transform;
						//재장전 패널 애니메이션
						StartCoroutine(PUM.ReloadPanelActual());
						yield return new WaitForSeconds(1.5f);

						//재장전 데이터 처리
						PM.FriendlyReloadActual(PM.CurrentUnit.UnitIndex);
						//탄약 텍스트 갱신
						PUM.BulletText_refresher(PM.CurrentUnit.UnitIndex,false,0);
						//시스템 일반으로 전환
						SYSTEMSTATE_Actual = SystemState.System_Idle;
						//로그호출
						StartCoroutine(PUM.LogOutputActual(12));
						//코스트 갱신
						PM.CostDecrease_Actual(1);
					}
				}
				//만약 해당 안드로이드의 총기가 DMR 일경우,
				else
				{
					StartCoroutine(PUM.LogOutputActual(6));
				}
				
			}
			//현재 장전된 탄약수가 최대 탄약수 일경우,
			else
			{
				StartCoroutine(PUM.LogOutputActual(7));
			}
		}
		//소모할 코스트가 모자랄 경우,
		else
		{
			StartCoroutine(PUM.LogOutputActual(2));
		}
	}

	//이동 버튼 클릭 메서드 // 행동 코스트
	public IEnumerator MoveBtn_Act()
	{
		//동작 코스트 나 일반 코스트가 있는지 확인
		if(PM.ActingCost != 0||PM.NomalCost != 0)
		{
			//시스템 상태를 "이동" 으로 전환
			SYSTEMSTATE_Actual = SystemState.System_Move;
			//현재 페이즈 유닛 오브젝트
			Transform CurrentFriendly_obj = PM.UnitObjList[PM.CurrentUnit.UnitIndex].transform;
			//이동 사거리 오브젝트 위치 아군 위치로 초기화
			Move_Range.transform.position = CurrentFriendly_obj.transform.position;

			//범위 활성화
			StartCoroutine(MoveRange_on_off(true));

			//아군 행동 패널 비활성화
        	PUM.FriendlyTurnPanel.SetActive(false);
			//행동취소 버튼 패널 활성화
			ActingSure_panel.SetActive(true);
			
			//이동포인트 할당 , 텍스트 갱신
			movepoint = PM.CurrentUnit.Speed/2;
			PUM.MovePoint_refresher(movepoint);

			ActFinish = false;
			while(true)
			{
				yield return null;
				if(ActFinish == true)
				{
					break;
				}
			}

			//아군 행동 패널 활성화
       		PUM.FriendlyTurnPanel.SetActive(true);	
			//배치확정 버튼 비활성화
			ActingSure_btn.SetActive(false);
			//행동취소 버튼 패널 비활성화
			ActingSure_panel.SetActive(false);

			//이동 포인트 텍스트 비활성화
			PUM.MovingPoint_txt.gameObject.SetActive(false);

			//시스템 일반으로 전환
			SYSTEMSTATE_Actual = SystemState.System_Idle;

			//이동 범위 초기화
			StartCoroutine(MoveRange_on_off(false));
			
			//만약 코스트가 소모 되었을 경우,
			if(UsedCost)
			{
				//코스트 감소
				PM.CostDecrease_Actual(0);
			}
		}
		//소모할 코스트가 모자랄 경우,
		else
		{
			StartCoroutine(PUM.LogOutputActual(2));
		}
	}

	//스캔 버튼 클릭 메서드 // 판단 코스트 
	public IEnumerator ScanBtn_Act()
	{
		//판단 코스트나 일반코스트가 있는지 확인
		if(PM.JudgingCost != 0||PM.NomalCost != 0)
		{
			//현재 발각된 적이 있는지 확인
			if(PM.CheckEnemyDetected() == true)
			{
				//시스템 상태를 "스캔" 으로 전환
				SYSTEMSTATE_Actual = SystemState.System_Scan;
				//현재 페이즈 유닛 오브젝트
				Transform CurrentFriendly_obj = PM.UnitObjList[PM.CurrentUnit.UnitIndex].transform;

				//범위 오브젝트 on // VisibleRange
				GameObject range_obj = CurrentFriendly_obj.GetChild(1).gameObject;
				range_obj.GetComponent<AndroidRange_Manager>().Input = true;
				range_obj.gameObject.SetActive(true);
				yield return new WaitForSeconds(0.2f);
				range_obj.gameObject.SetActive(false);

				//아군 행동 패널 비활성화
				PUM.FriendlyTurnPanel.SetActive(false);
				//행동취소 버튼 패널 활성화
				ActingSure_panel.SetActive(true);

				ActFinish = false;
				while(true)
				{
					yield return null;
					//다음턴으로 넘길수 있을 경우, 종료
					if(ActFinish == true)
					{
						break;
					}
				}

				//Debug.Log("스캔 종료");

				//아군 행동 패널 활성화
				PUM.FriendlyTurnPanel.SetActive(true);
				//배치확정 버튼 비활성화
				ActingSure_btn.SetActive(false);
				//행동취소 버튼 패널 비활성화
				ActingSure_panel.SetActive(false);

				//시스템 일반으로 전환
				SYSTEMSTATE_Actual = SystemState.System_Idle;
				
				//범위 초기화
				range_obj.GetComponent<AndroidRange_Manager>().Input = false;
				range_obj.gameObject.SetActive(true);
				yield return new WaitForSeconds(0.2f);
				range_obj.gameObject.SetActive(false);

				//만약 코스트가 소모 되었을 경우,
				if(UsedCost)
				{
					//코스트 감소
					PM.CostDecrease_Actual(1);
				}
			}
			//현재 발각된 적이 없을 경우,
			else
			{
				StartCoroutine(PUM.LogOutputActual(1));
			}
		}
			
		//소모할 코스트가 모자랄 경우,
		else
		{
			StartCoroutine(PUM.LogOutputActual(2));
		}
	}

	//행동취소 버튼 클릭시
	public void ActingCancelBtn_pressed()
	{
		//로그호출
		StartCoroutine(PUM.LogOutputActual(10));
		if(SelectedEnemy != null)
		{
			//유닛 락온 UI off
			SelectedEnemy.transform.parent.GetChild(3).transform.GetChild(5).gameObject.SetActive(false);
		}
		UsedCost = false;
		ActFinish = true;
		SelectedPlate = null;
		SelectedEnemy = null;
	}

	//행동확정 버튼 클릭시
	public void ActingSureBtn_pressed()
	{
		switch(SYSTEMSTATE_Actual)
		{
			//상황 공중지원 일경우,
			case SystemState.System_AirSupport:
				//공중지원 활성화
				AirsupportActual(SelectedPlate);
				//로그호출
				StartCoroutine(PUM.LogOutputActual(4));
				//반복문 종료 , 코스트 사용 값 반환
				UsedCost = true;
				ActFinish = true;
				SelectedPlate = null;
				SelectedEnemy = null;
				break;
			//상황 공격 일경우,
			case SystemState.System_Attack:
				//로그호출
				StartCoroutine(PUM.LogOutputActual(0));
				//공격 코루틴 메서드 호출
				StartCoroutine(AttackActual(SelectedEnemy,PM.UnitObjList[PM.CurrentUnit.UnitIndex].gameObject));
				//반복문 종료 , 코스트 사용 값 반환
				UsedCost = true;
				ActFinish = true;
				//유닛 락온 UI off
				SelectedEnemy.transform.parent.GetChild(3).transform.GetChild(5).gameObject.SetActive(false);
				SelectedPlate = null;
				SelectedEnemy = null;
				break;
			//상황 경계 일경우,
			case SystemState.System_Boundary:
				//로그호출
				StartCoroutine(PUM.LogOutputActual(3));
				//반복문 종료 , 코스트 사용 값 반환
				UsedCost = true;
				ActFinish = true;
				SelectedPlate = null;
				SelectedEnemy = null;
				break;
			//상황 이동 일경우,
			case SystemState.System_Move:
				StartCoroutine(ActMove_allmethod());
				break;
			//상황 스캔 일경우,
			case SystemState.System_Scan:
				//로그호출
				StartCoroutine(PUM.LogOutputActual(9));
				//반복문 종료 , 코스트 사용 값 반환
				UsedCost = true;
				ActFinish = true;
				SelectedPlate = null;
				SelectedEnemy = null;
				break;
		}
	}

	//이동 시 전체 메서드
	public IEnumerator ActMove_allmethod()
	{
		if(movepoint != 0)
		{
			SYSTEMSTATE_Actual = SystemState.System_Idle;
			//선택된 타일 상태 초기화
			SelectedPlate.GetComponent<PlateIndividual_manager>().StateToRange();

			//범위 비활성화
			StartCoroutine(MoveRange_on_off(false));
			yield return new WaitForSeconds(0.1f);

			//코스트 사용 값 반환
			UsedCost = true;
			//행동취소버튼 비활성화
			ActingCancel_btn.SetActive(false);
			//이동종료 버튼 활성화
			ActMoveExit_btn.SetActive(true);
			//이동 포인트 감소
			movepoint -=1;
			//이동포인트 텍스트 갱신
			PUM.MovePoint_refresher(movepoint);
			//행동 확정 버튼 비활성화
			ActingSure_btn.SetActive(false);

			//실질적 이동 메서드
			MoveActual(SelectedPlate,PM.UnitObjList[PM.CurrentUnit.UnitIndex].transform.gameObject);
			//이동 사거리 오브젝트 위치 아군 위치로 초기화
			Move_Range.transform.position = PM.UnitObjList[PM.CurrentUnit.UnitIndex].transform.position;

			//현재 플레이트 할당
			CurrentPlate = SelectedPlate;

			//범위 활성화
			StartCoroutine(MoveRange_on_off(true));
			SYSTEMSTATE_Actual = SystemState.System_Move;
		}
		else
		{
			//더이상 이동 불가 로그 출력
			StartCoroutine(PUM.LogOutputActual(13));
		}
	}

	//이동 종료 버튼 클릭 메서드
	public void ActMoveExitBtn_pressed()
	{
		//행동종료 버튼 활성화
		ActingCancel_btn.SetActive(true);
		//이동 종료 버튼 비활성화
		ActMoveExit_btn.SetActive(false);

		//안드로이드 데이터 상 좌표 할당
		PM.UnitListInfo.UNITINFO[PM.CurrentUnit.UnitIndex].Coord_X = CurrentPlate.GetComponent<PlateIndividual_manager>().X;
		PM.UnitListInfo.UNITINFO[PM.CurrentUnit.UnitIndex].Coord_Y = CurrentPlate.GetComponent<PlateIndividual_manager>().Y;

		//로그호출
		StartCoroutine(PUM.LogOutputActual(8));
		SelectedPlate = null;
		//반복문 종료
		ActFinish = true;
	}

	//경계범위 오브젝트 초기화 메서드
	void ResetBoundaryRange_obj()
	{
		//범위 지름값
		int PlayerRange = (int)PM.PlayerAndroid_status[PM.CurrentUnit.UnitIndex]["Scanradius"];
		//범위 지름 벡터화
		Vector3 BoundaryRange_vec = new Vector3 (PlayerRange,4,PlayerRange);
		//범위 크기 조정
		Boundary_Range.transform.localScale = BoundaryRange_vec;
	}
}