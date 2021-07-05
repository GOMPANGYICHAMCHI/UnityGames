using UnityEngine;

public class AndroidStatus
{	
	//총알당 데미지//턴당 발사수//속도
	public int DPB , RPT , Speed 
	//공격 사거리//스캔범위//식별 사거리
	, AttackRange , ScanRadius , IdentificationDistance
	//피격대응사격 데미지//기습사격 데미지//기습대응사격 데미지
	, ResponseShotDamage , SurpriseShotDamage , CounterShotDamage 
	//방어력//체력//호출코드
	, Armor , Health , Code
	//판단코스트//행동코스트//일반코스트
	, JudgementCost , BehaviorCost , NomalCost;
}