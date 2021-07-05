using UnityEngine;
using System.Collections;
//using UnityEngine.UI;

// 아군 오브젝트 0번 자식 : IdentificationDistance / 1번 자식 : VisibleRange / 2번 자식 : AttackRange

public class PhaseFriendlyAct_Manager : MonoBehaviour 
{
	public Phase_Manager PM;
	public PhaseUI_Manager PUM;

	//메인 카메라
	public Camera MainCam_camera;  

	//데미지 팝업 텍스트
	public GameObject PopupText;

	//공중지원기 A10 오브젝트
	public GameObject AirSupportObj_a10;

	//폭발 이펙트
	public GameObject ExplosionParticle_obj;

	//공격 메서드
	public IEnumerator AttackActual(GameObject TargetEnemy_obj,GameObject Friendly_obj)
	{
		//타겟 적 유닛 오브젝트 인덱스 할당
		int TargetEnemyIndex = TargetEnemy_obj.GetComponent<Unit_tag>().Index;
		//아군 유닛 오브젝트 인덱스 할당
		int FriendlyIndex = Friendly_obj.transform.GetChild(3).GetComponent<Unit_tag>().Index;
		//데미지 계산
		int[] ActualDamage = PM.FriendlyAttackDamageCalculator(TargetEnemyIndex,FriendlyIndex,PM.CheckEnemyInBuilding(TargetEnemyIndex));

		//적의 체력바가 켜저 있지 않다면, 체력바 활성화
		if(TargetEnemy_obj.transform.parent.GetChild(3).transform.GetChild(4).gameObject.activeSelf == false)
		{
			TargetEnemy_obj.transform.parent.GetChild(3).transform.GetChild(4).gameObject.SetActive(true);
		}

		//계산된 데미지 수만큼 진행되는 for문
		for(int i = 0 ; i < ActualDamage.Length ; i++)
		{
			yield return new WaitForSeconds(0.2f);
			//데미지 팝업 텍스트 메서드 작동
			PUM.DamageTextPopupActual(TargetEnemy_obj.transform,ActualDamage[i].ToString());
			//적 체력 감소
			PM.UnitListInfo.UNITINFO[TargetEnemyIndex].CurrentHealth -= ActualDamage[i];
			//적 체력바 동기화
			PUM.EnemyHealthBar_refresher(TargetEnemy_obj.transform.parent.GetChild(3).GetChild(4).gameObject,TargetEnemyIndex);
			//아군 탄약 텍스트 갱신 메서드 작동
			PUM.BulletText_refresher(FriendlyIndex,true,ActualDamage.Length-(i+1));

			//만약 공격 대상 적이 사망할 경우,
			if(PM.UnitListInfo.UNITINFO[TargetEnemyIndex].CurrentHealth <= 0)
			{
				//해당 적 체력 0 으로 초기화
				PM.UnitListInfo.UNITINFO[TargetEnemyIndex].CurrentHealth = 0;
				//for문 나가기
				i = ActualDamage.Length;
				//적 사망 로그 출력
				StartCoroutine(PUM.LogOutputActual(14));
				
				Destroy(TargetEnemy_obj.transform.parent.GetChild(0).gameObject);
				Destroy(TargetEnemy_obj.transform.parent.GetChild(1).gameObject);
				Destroy(TargetEnemy_obj.transform.parent.GetChild(2).gameObject);
				Destroy(TargetEnemy_obj.transform.parent.GetChild(3).transform.GetChild(0).gameObject);
				Destroy(TargetEnemy_obj.transform.parent.GetChild(3).transform.GetChild(1).gameObject);
				Destroy(TargetEnemy_obj.transform.parent.GetChild(3).transform.GetChild(2).gameObject);
				TargetEnemy_obj.transform.parent.GetChild(3).transform.GetChild(3).gameObject.SetActive(true);
				Destroy(TargetEnemy_obj.transform.parent.GetChild(3).transform.GetChild(4).gameObject);
				Destroy(TargetEnemy_obj.transform.parent.GetChild(3).transform.GetChild(5).gameObject);
			}
		}
	}

	//경계 메서드
	public void BoundaryActual(GameObject TargetPlate_obj,GameObject Boundary_obj)
	{
		
	}

	//공중지원 메서드
	public void AirsupportActual(GameObject TargetPlate_obj)
	{
		//공중지원기 생성
		Instantiate(AirSupportObj_a10,TargetPlate_obj.transform.position,Quaternion.identity);
		//파티클 메서드 
		StartCoroutine(Explosion_on(TargetPlate_obj.transform));
	}

	//이동 메서드
	public void MoveActual(GameObject TargetPlate_obj,GameObject Friendly_obj)
	{
		//선택된 플레이트로 아군 이동
		Friendly_obj.transform.position = TargetPlate_obj.transform.position;
	}

	//스캔 메서드
	public void ScanActual(GameObject TargetEnemy_obj)
	{
		
	}

	//폭발 코루틴 메서드
	IEnumerator Explosion_on(Transform TargetTransform)
	{
		yield return new WaitForSeconds(1.5f);
		//폭발 파티클 작동
		ExplosionParticle_obj.transform.position = TargetTransform.position;
		ExplosionParticle_obj.SetActive(true);

		//폭발 이펙트 off
		yield return new WaitForSeconds(1.5f);
		ExplosionParticle_obj.SetActive(false);
	}
}
