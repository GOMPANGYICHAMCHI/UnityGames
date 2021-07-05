using UnityEngine;

public class PlateIndividual_manager : MonoBehaviour 
{
	//플레이트 좌표 코드
	public int X , Y;

	//해당 오브젝트 매쉬 렌더러
	public MeshRenderer MR;

	//기본 메테리얼
	public Material OwnMaterial_mat;

	//선택시 메테리얼
	public Material SelectableMaterial_mat;
	//범위 메테리얼
	public Material RangeMaterial_mat;
	//선택성 범위 메테리얼
	public Material SelectableRangeMaterial_mat;

	public bool Selectable = false;		//선택가능 타일 여부
	public bool Range = false;			//범위 타일 여부
	public bool SelectableRange = false;//선택가능 범위 타일 여부
	public bool UnTouchable = true;     //해당타일 클릭 가능 여부

	//시작시 렌더러 불러오기
	void Start () 
	{
		//매쉬 렌더러 할당
		MR = this.gameObject.GetComponent<MeshRenderer>();
	}
	
	//상태 전부 초기화
	public void Reset_bool()
	{
		Selectable = false;
		Range = false;
		SelectableRange = false;
		UnTouchable = true;
	}

	//마우스가 가리켰을때,
	void OnMouseEnter() 
	{
		if(!UnTouchable)
		{
			//선택 가능 할경우
			if(Selectable)
			{
				//선택 가능 메테리얼 할당
				MR.material = SelectableMaterial_mat;
				//선택 가능 범위 일경우
				if(SelectableRange)
					//선택 가능 범위 메테리얼 할당
					MR.material = SelectableRangeMaterial_mat;
			}
			//선택 가능 범위 일경우
			else if(SelectableRange)
				//선택 가능 범위 메테리얼 할당
				MR.material = SelectableRangeMaterial_mat;
		}
	}

	//마우스가 나갈때,
	void OnMouseExit() 
	{
		if(!UnTouchable)
		{
			//범위 상태 일경우
			if(Range&&!SelectableRange)
			{
				//범위 메테리얼 할당
				MR.material = RangeMaterial_mat;
			}
			else if(SelectableRange)
			{
				//범위 메테리얼 할당
				MR.material = SelectableRangeMaterial_mat;
			}
			//아닐경우
			else
			//메테리얼 기본으로 초기화
				MR.material = OwnMaterial_mat;
		}
	}

	//범위로 상태 및 메테리얼 초기화
	public void StateToRange()
	{
		Selectable = true;
		SelectableRange = false;
		Range = true;
		UnTouchable = false;
		MR.material = RangeMaterial_mat;
	}

	//선택된 범위로 상태 및 메테리얼 초기화
	public void StateToSelectedRange()
	{
		Selectable = true;
		SelectableRange = true;
		UnTouchable = false;
		MR.material = SelectableRangeMaterial_mat;
	}

	//공격범위로 상태 및 메테리얼 초기화
	public void StateToAttackRange()
	{
		MR.material = RangeMaterial_mat;
	}

	//일반으로 상태 및 메테리얼 초기화
	public void StateToNomal()
	{
		Reset_bool();
		MR.material = OwnMaterial_mat;
	}

	//범위 인지 확인한뒤 메테리얼 범위로 변경
	public void IsRangeNow()
	{
		if(Range)
		{
			StateToRange();
		}
		else
		{
			Reset_bool();
			MR.material = OwnMaterial_mat;
		}
	}
}