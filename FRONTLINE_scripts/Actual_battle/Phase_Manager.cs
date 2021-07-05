using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseInsertedUnit
{
    //해당 유닛의 리스트 인덱스
    public int UnitIndex;

    //해당유닛의 스피드
    public int Speed;
    
    //유닛 판별 코드
    public int IdentifyCode; 

    public PhaseInsertedUnit(int unitindex,int speed,int identifycode)
    {
        UnitIndex = unitindex;
        Speed = speed;
        IdentifyCode = identifycode;
    }
}

public class Phase_Manager : Unit_List 
{   
    //실제 호출될 순서를 담은 페이즈 리스트 //순서정렬된 유닛의 정보를 보관합니다
    public PhaseInsertedUnit[] PhaseOrderList;

    //현재 호출된 유닛
    public PhaseInsertedUnit CurrentUnit;

    //페이즈 코스트------------------------------------
    public int ActingCost; //행동 코스트
	public int JudgingCost; //판단 코스트
	public int NomalCost; //일반 코스트

    //페이즈 아군행동 매니저----------------------------
    public PhaseFriendlyAct_Manager PFAM;
    //페이즈 아군행동 매니저 오브젝트
    public GameObject PFAM_obj;
    //------------------------------------------------

    //다음턴으로 넘길수있는지 확인하는 변수
    public bool TurntoNextable = false;

    void Awake()
    {
       CurrentBattlePlate = BattlePlateListLoader(BattlePLateCoord_To_MapFile());
       PlayerAndroidStat_setActual(CurrentBattlePlate.BattlePlateLevel);
       UnitListInfo = UNITLIST_GeneratorActual(BattlePLateCoord_To_MapFile());
       //UnitListInfo = UNITLIST_Loader();
       //유닛 오브젝트 리스트 초기화
       UnitObjList = EnemySpawner_Actual(UnitListInfo,EnemyObject,CurrentBattlePlate.BattlePlateLevel);
    }

    //페이즈 매니저 //끝난후 페이즈 메이커를 호출
	IEnumerator PhaseManager_Actual()
    {
        //현제 턴인 리스트 인덱스
        int TurnIndex = 0;
        
        for(TurnIndex = 0 ; TurnIndex < PhaseOrderList.Length ; TurnIndex ++)
        {
            //현제 턴 인덱스가 아군 일 경우,
            if(PhaseOrderList[TurnIndex].IdentifyCode == 0)
            {
                //해당 유닛이 사망상태가 아닐경우,
                if(UnitListInfo.UNITINFO[PhaseOrderList[TurnIndex].UnitIndex].CurrentHealth != 0)
                {
                    //다음턴으로 넘기기(불가능)
                    TurntoNextable = false;

                    //현재 유닛 할당
                    CurrentUnit = PhaseOrderList[TurnIndex];

                    //아군 페이즈 진행
                    yield return StartCoroutine(FriendlyPhase_Actual(TurnIndex));
                }

                //사망상태 일경우,
                else
                {
                    
                }
            }

            //현재 턴인덱스가 적 일 경우,
            else
            {
                //해당 유닛이 사망상태가 아닐경우,
                if(UnitListInfo.UNITINFO[PhaseOrderList[TurnIndex].UnitIndex].CurrentHealth != 0)
                {
                    //다음턴으로 넘기기(불가능)
                    TurntoNextable = false;

                    //현재 유닛 할당
                    CurrentUnit = PhaseOrderList[TurnIndex];

                    //적 페이즈 진행
                    yield return StartCoroutine(EnemyPhase_Actual(TurnIndex));
                }

                //사망상태 일경우,
                else
                {

                }
            }
        }
        PhaseMaker_Actual();
    }

    //-------------------------------------------------------------------------
    //페이즈 메이커 //끝난후 페이즈 매니저를 호출
    public void PhaseMaker_Actual()
    {
        //유닛 페이즈 리스트 생성 및 할당
        UnitInsertToPhase();

        //페이즈매니저 호출
        StartCoroutine(PhaseManager_Actual());
    }

    //페이즈크기를 재는 메서드 (사망하지 않은 아군 + 발각된 적의 수)
    int PhaseScaleCounter()
    {
        int PhaseScale = 0;

        //사망하지 않은 아군의 수 카운팅
        for(int i = 0 ; i < 4 ; i++)
        {
            if(UnitListInfo.UNITINFO[i].CurrentHealth != 0)
            {
                PhaseScale++;
            }
        }

        //발각된 적의 수 카운팅
        for(int i = 4 ; i < UnitListInfo.UNITCOUNT ; i++)
        {
            if(UnitListInfo.UNITINFO[i].detected == true)
            {
               PhaseScale++;
            }
        }

        return PhaseScale;
    }

    //비순서정렬 페이즈에 페이즈 편입될 리스트 생성
    //페이즈 편입 유닛 속도 계산기
    void UnitInsertToPhase()
    {
        //유닛 페이즈 크기 
        int PhaseScale = PhaseScaleCounter();

        //페이즈 크기 할당
        PhaseOrderList = new PhaseInsertedUnit[PhaseScale];

        int phasenum = 0;

        //사망하지 않은 아군을 페이즈에 할당
        for(int i = 0 ; i < 4 ; i++)
        {
            if(UnitListInfo.UNITINFO[i].CurrentHealth != 0)
            {
                //유닛 리스트 인덱스 및 계산된 속도를 할당
                int RI = Random.Range(1,7);
                PhaseOrderList[phasenum] = new PhaseInsertedUnit(i,UnitListInfo.UNITINFO[i].Speed + RI,0);
                phasenum++;
            }
        }

        //발각된 적을 페이즈에 할당
        for(int i = 4 ; i < UnitListInfo.UNITCOUNT ; i++)
        {
            if(UnitListInfo.UNITINFO[i].detected == true)
            {
                //유닛 리스트 인덱스 및 계산된 속도를 할당
                int RI = Random.Range(1,7);
                PhaseOrderList[phasenum] = 
                new PhaseInsertedUnit(UnitListInfo.UNITINFO[i].Index,UnitListInfo.UNITINFO[i].Speed + RI,1);
                phasenum++;
            }
        }

        OrderPhaseList(PhaseScale);
    }

    //페이즈 순서 정렬
    void OrderPhaseList(int PhaseScale)
    {
        PhaseInsertedUnit temp = new PhaseInsertedUnit(0,0,0);

        //버블 정렬
        for (int i = 0; i < PhaseScale - 1; i++)
        {
            for (int j = 0; j < PhaseScale - 1 - i; j++)
            {
                if (PhaseOrderList[j].Speed < PhaseOrderList[j + 1].Speed)
                {
                    temp = PhaseOrderList[j];
                    PhaseOrderList[j] = PhaseOrderList[j + 1];
                    PhaseOrderList[j + 1] = temp;
                }
            }
        }
    }
    //-------------------------------------------------------------------------
    
    //아군 페이즈 메서드
    IEnumerator FriendlyPhase_Actual(int TurnIndex)
    {
        //중앙 상황전달 텍스트 이미지 출력
        StartCoroutine(PUM.SituationImg_AppearActual_IE(2));
        yield return new WaitForSeconds(3);
        
        //아군턴 행동 패널 활성화
        PUM.FriendlyTurnPanel_Actual
        (PhaseOrderList[TurnIndex].UnitIndex,(int)PlayerAndroid_status[PhaseOrderList[TurnIndex].UnitIndex]["code"]);
        
        //코스트 할당
        ActingCost = (int)PlayerAndroid_status[PhaseOrderList[TurnIndex].UnitIndex]["BehaviorCost"];
	    JudgingCost = (int)PlayerAndroid_status[PhaseOrderList[TurnIndex].UnitIndex]["JudgementCost"];
	    NomalCost = (int)PlayerAndroid_status[PhaseOrderList[TurnIndex].UnitIndex]["NomalCost"];

        //유닛 트래커 이미지 on
        FriendlyUnitTracker_on_off(true,true,UnitObjList[CurrentUnit.UnitIndex]);

        while(true)
        {
            yield return null;
            //다음턴으로 넘길수 있을 경우, 종료
            if(TurntoNextable == true)
            {
                break;
            }
        }
        //아군 행동 패널 비활성화
        PUM.FriendlyTurnPanel.SetActive(false);

        //유닛 트래커 이미지 off
        FriendlyUnitTracker_on_off(false,true,UnitObjList[CurrentUnit.UnitIndex]);
    }

    //적군 페이즈 메서드
    IEnumerator EnemyPhase_Actual(int TurnIndex)
    {
        //중앙 상황전달 텍스트 이미지 출력
        StartCoroutine(PUM.SituationImg_AppearActual_IE(2));
        yield return new WaitForSeconds(1);
        
    }

    //코스트 감소처리 메서드 / 코드 0 : 행동코스트 / 코드 1 : 판단코스트
    //코스트 UI 갱신포함 
    public void CostDecrease_Actual(int TargetCost)
    {
        //행동 코스트 대상
        if(TargetCost == 0)
        {
            if(ActingCost == 0)
            {
                NomalCost -=1;
            }
            else
            {
                ActingCost -=1;
            }
        }
        //판단 코스트 대상
        else
        {
            if(JudgingCost == 0)
            {
                NomalCost -=1;
            }
            else
            {
                JudgingCost -=1;
            }
        }

        //코스트 UI 텍스트 갱신
        PUM.CostText_refresher();
    }

    //유닛 트래커 이미지 변경
    void FriendlyUnitTracker_on_off(bool on,bool isfriendly,Transform CurrentUnit_obj)
    {
        //아군 일때
        if(isfriendly)
        {
            if(on)
            {
                CurrentUnit_obj.GetChild(4).transform.GetChild(1).gameObject.SetActive(false);
                CurrentUnit_obj.GetChild(4).transform.GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                CurrentUnit_obj.GetChild(4).transform.GetChild(1).gameObject.SetActive(true);
                CurrentUnit_obj.GetChild(4).transform.GetChild(2).gameObject.SetActive(false);
            }
        }
        //적 일때
        else
        {
            if(on)
            {
                CurrentUnit_obj.GetChild(3).transform.GetChild(1).gameObject.SetActive(false);
                CurrentUnit_obj.GetChild(3).transform.GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                CurrentUnit_obj.GetChild(3).transform.GetChild(1).gameObject.SetActive(true);
                CurrentUnit_obj.GetChild(3).transform.GetChild(2).gameObject.SetActive(false);
            }
        }
    }
}
