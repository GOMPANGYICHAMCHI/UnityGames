using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.IO;
using UnityEngine.SceneManagement;
public class WorldPlateUI_Manager : MonoBehaviour 
{
	public static WorldPlateUI_Manager instance;

	public bool TextReady = false;

	//텍스트 키와 텍스트값을 저장하는 딕셔너리
	public Dictionary<string,string> LoadedText;

	//-----------------------------------------------
	//플레이어가 선택한 플레이트 표시 오브젝트
	public GameObject PlayerPicker;
	//플레이어핔커 좌표
	public int[] PlayerPickerCoord = new int[2];

	//-----------------------------------------------
	//플레이어핔커 스프라이트
	// 0 번 : 마을외곽 // 1 번 : 마을 // 2 번 : 도시 
	public Sprite[] PlayerPickerKind_sprite = new Sprite[3];

	//-----------------------------------------------
	//플레이트 정보 패널
	public GameObject PlateInfo_panel;
	//현재 플레이트 좌표
	public Text[] CurrentPlateCoord_txt = new Text[2];
	//현재 플레이트 타입 텍스트
	public Text CurrentPlateType_text;
	//플레이트 타입 이미지
	public Image CurrentPlateType_img;
	//플레이트 타입 스프라이트
	public Sprite[] PlateType_sprite = new Sprite[3];

	//-----------------------------------------------

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else if(instance != this)
		{
			Destroy(gameObject);
		}
		LocalizedTextLoad();
	}
	
	void Update()
	{
		//마우스 버튼(0) 입력시
		if(Input.GetButtonDown("Fire1"))
		{
			//현재 마우스 위치로 레이캐스트 출력
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			//충돌 물체가 월드 플레이트 일경우,
			if(Physics.Raycast(ray ,out hit)&&hit.transform.gameObject.tag == "WorldPlate")
			{
				//플레이어핔커 위치 할당
				Vector3 picker_pos = hit.transform.position;
				PlayerPicker.transform.position = picker_pos;
				//플레이어 핔커 스프라이트 할당
				PlayerPicker.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Image>().sprite = 
				PlayerPickerKind_sprite[hit.transform.gameObject.GetComponent<WorldPlate_individual>().Type-1];
				//현재좌표 할당
				PlayerPickerCoord[0] = hit.transform.gameObject.GetComponent<WorldPlate_individual>().Coord_x;
				PlayerPickerCoord[1] = hit.transform.gameObject.GetComponent<WorldPlate_individual>().Coord_y;
				//플레이트 UI 갱신
				PlatePickerUI_refresher(hit.transform.gameObject.GetComponent<WorldPlate_individual>().Type-1);
			}
		}
	}

	//LoadedText딕셔너리 초기화
	public void LocalizedTextLoad()
	{
		LoadedText = new Dictionary<string,string>();

		//LanguageCode에서 로드한 언어코드를 기반으로 제이슨 텍스트 파일 접근
		string filepath = File.ReadAllText
		(Application.dataPath + "/Data/Language/WorldPlate_Lang/WPlang_"+LanguageCode.LCODE+".json");
		LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(filepath);

		for(int i = 0; i < loadedData.UIINFO.Length ; i++)
		{
			LoadedText.Add(loadedData.UIINFO[i].key,loadedData.UIINFO[i].value);
		}

		TextReady = true;
	}

	//키를 입력받아 텍스트를 전달하는 메서드
	//입력 : 키 // 출력 : 키에 해당 하는 텍스트 (기본값 : MissingText)
	public string GetLocalizedValue(string key)
	{
		//텍스트 기본값 지정
		string OutText = "MissingText";
		//만약 대응하는 키가 존재할경우,
		if(LoadedText.ContainsKey(key))
		{
			OutText = LoadedText[key];
		}
		//텍스트 반환
		return OutText;
	}

	//배틀플레이트로 넘어가는 버튼 클릭시 메서드
	public void ToBattlePlateBtn_prssed()
	{
		string ctbp = File.ReadAllText(Application.dataPath + "/Data/GameProgress_save/LastBattlePlate/BattlePlateCoord_ForLoad.json");
		JsonData CTBP = JsonMapper.ToObject(ctbp);

		CTBP["X"] = PlayerPickerCoord[0];
		CTBP["Y"] = PlayerPickerCoord[1];

		JsonData CoordToBattlePlate = JsonMapper.ToJson(CTBP);
		File.WriteAllText(Application.dataPath +"/Data/GameProgress_save/LastBattlePlate/BattlePlateCoord_ForLoad.json",CoordToBattlePlate.ToString());

		//로딩 인덱스 변경
		LoadingIndex.LoadSceneIndex = 2;
		//로딩씬 로드
		SceneManager.LoadScene("LoadingScene");
	}

	//배틀플레이트 UI 갱신 메서드
	void PlatePickerUI_refresher(int Platetype)
	{
		//플레이트 정보 패널 활성화
		PlateInfo_panel.SetActive(true);
		//플레이트 좌표 텍스트 할당
		CurrentPlateCoord_txt[0].text = "X:" + PlayerPickerCoord[0].ToString();
		CurrentPlateCoord_txt[1].text = "Y:" + PlayerPickerCoord[1].ToString();
		//플레이트 이미지 할당
		CurrentPlateType_img.sprite = PlateType_sprite[Platetype];
		//플레이트 형식 텍스트 할당
		CurrentPlateType_text.text = LoadedText["platekind" + Platetype +"_txt"];
	}
}