//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.IO;
using UnityEngine.SceneManagement;

public class MainMenuUI_manager : MonoBehaviour 
{
	public static MainMenuUI_manager instance;

	public bool TextReady = false;

	//-----------------------------------------------
	//새게임 시작 경고 패널
	public GameObject NewGameWarning_panel;

	//-----------------------------------------------
	//게임 나가기 경고 패널
	public GameObject ExitGameWarning_panel;

	//-----------------------------------------------
	//옵션 패널
	public GameObject Option_panel;

	//-----------------------------------------------
	//이어하기 버튼 
	public GameObject ContinueBtn_panel;

	//-----------------------------------------------

	//선택 버튼음
	public AudioSource SelectTick_sound;
	//취소 버튼음
	public AudioSource CancelTick_sound;

	//텍스트 키와 텍스트값을 저장하는 딕셔너리
	public Dictionary<string,string> LoadedText;

	void Awake()
	{
		//언어 코드 불러오기
		string langcode = File.ReadAllText(Application.dataPath + "/Data/Language/LanguageCode.json");
		JsonData LANGCODE = JsonMapper.ToObject(langcode);
		//언어 코드 할당
		LanguageCode.LCODE = (string)LANGCODE["LanguageCode"][(int)LANGCODE["CurrentLanguage"]]["Code"];

		if(instance == null)
		{
			instance = this;
		}
		else if(instance != this)
		{
			Destroy(gameObject);
		}
		LocalizedTextLoad();

		string savecheck = File.ReadAllText(Application.dataPath + "/Data/Status_save/SaveCheck.json");
		JsonData SaveCheck = JsonMapper.ToObject(savecheck);

		//기존 데이터가 없다면
		if((int)SaveCheck["savecheck"] == 0)
		{
			//이어하기 버튼 비활성화
			ContinueBtn_panel.SetActive(false);
		}
	}
	//언어팩 갱신 메서드
	public void LanguagePackRefresher()
	{
		//언어 코드 불러오기
		string langcode = File.ReadAllText(Application.dataPath + "/Data/Language/LanguageCode.json");
		JsonData LANGCODE = JsonMapper.ToObject(langcode);
		//언어 코드 할당
		LanguageCode.LCODE = (string)LANGCODE["LanguageCode"][(int)LANGCODE["CurrentLanguage"]]["Code"];

		LoadedText = new Dictionary<string,string>();

		//LanguageCode에서 로드한 언어코드를 기반으로 제이슨 텍스트 파일 접근
		string filepath = File.ReadAllText
		(Application.dataPath + "/Data/Language/MainMenu_Lang/MMlang_"+LanguageCode.LCODE+".json");
		LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(filepath);

		for(int i = 0; i < loadedData.UIINFO.Length ; i++)
		{
			LoadedText.Add(loadedData.UIINFO[i].key,loadedData.UIINFO[i].value);
		}
	}

	//LoadedText딕셔너리 초기화
	public void LocalizedTextLoad()
	{
		LoadedText = new Dictionary<string,string>();

		//LanguageCode에서 로드한 언어코드를 기반으로 제이슨 텍스트 파일 접근
		string filepath = File.ReadAllText
		(Application.dataPath + "/Data/Language/MainMenu_Lang/MMlang_"+LanguageCode.LCODE+".json");
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

	//새게임 버튼클릭시 메서드
	public void NewGameBtn_pressed()
	{
		//기존 데이터가 존재 한다면
		if(ContinueBtn_panel.activeSelf == true)
		{
			//선택버튼음 재생
			//SelectTick_sound.Play();
			//새게임 경고 패널 활성화
			NewGameWarning_panel.SetActive(true);
		}
		//기존 데이터가 존재 하지 않는다면
		else
		{
			//선택버튼음 재생
			//SelectTick_sound.Play();
			//로딩 인덱스 변경
			LoadingIndex.LoadSceneIndex = 0;
			//로딩씬 로드
			SceneManager.LoadScene("LoadingScene");
		}
	}
	//이어하기 버튼클릭시 메서드
	public void ContinueBtn_pressed()
	{
		//선택버튼음 재생
		//SelectTick_sound.Play();

		//로딩 인덱스 변경
		LoadingIndex.LoadSceneIndex = 1;
		//로딩씬 로드
		SceneManager.LoadScene("LoadingScene");
	}
	//옵션 버튼클릭시 메서드
	public void OptionBtn_pressed()
	{
		//선택버튼음 재생
		//SelectTick_sound.Play();
		//패널 활성화
		Option_panel.SetActive(true);
	}
	//옵션 나가기 버튼클릭시 메서드
	public void OptionExitBtn_pressed()
	{
		//취소버튼음 재생
		//CancelTick_sound.Play();
		//패널 비활성화
		Option_panel.SetActive(false);
	}
	//나가기 버튼클릭시 메서드
	public void ExitBtn_pressed()
	{
		//선택버튼음 재생
		//SelectTick_sound.Play();
		//나가기 경고 패널 활성화
		ExitGameWarning_panel.SetActive(true);
	}
	//새게임 버튼 클릭시 경고 패널 선택버튼 메서드
	public void NewGameChoicePanelBtn_pressd(bool isYES)
	{
		//예 버튼 일경우, (새게임 시작)
		if(isYES)
		{
			//선택버튼음 재생
			//SelectTick_sound.Play();
			//로딩 인덱스 변경
			LoadingIndex.LoadSceneIndex = 0;
			//로딩씬 로드
			SceneManager.LoadScene("LoadingScene");
		}
		//아니요 버튼 일경우, (패널 닫기)
		else
		{
			//취소버튼음 재생
			//CancelTick_sound.Play();
			//패널 비활성화
			NewGameWarning_panel.SetActive(false);
		}
	}
	//나가기 버튼 클릭시 경고 패널 선택버튼 메서드
	public void ExitChoicePanelBtn_pressed(bool isYES)
	{
		//예 버튼 일경우, (게임 나가기)
		if(isYES)
		{
			//선택버튼음 재생
			//SelectTick_sound.Play();
			//게임 종료
			Application.Quit();
		}
		//아니요 버튼 일경우, (패널 닫기)
		else
		{
			//취소버튼음 재생
			//CancelTick_sound.Play();
			//페널 비활성화
			ExitGameWarning_panel.SetActive(false);
		}
	}
}