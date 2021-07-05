using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.IO;

public class LanguageOption_manager : MonoBehaviour 
{
	public MainMenuUI_manager MMUM;
	//언어팩 호출용 언어 기반 데이터
	JsonData LBDJ;
	//현재 언어코드
	[SerializeField]
	int CurrentLangIndex;
	//언어 코드 최대 인덱스
	[SerializeField]
	int MaxIndex;
	//언어 선택기 현재 언어
	public Text LangSelecterCurrentLang_txt;

	void Start()
	{
		//언어코드 불러오기
		string lbd = File.ReadAllText(Application.dataPath +"/Data/Language/LanguageCode.json");
		LBDJ = JsonMapper.ToObject(lbd);
		//현재 언어코드 할당
		CurrentLangIndex = (int)LBDJ["CurrentLanguage"];
		//최대 인덱스 할당
		MaxIndex = (int)LBDJ["LanguageCount"] - 1;
		//셀렉터 언어 할당
		LangSelecterCurrentLang_txt.text = (string)LBDJ["LanguageCode"][CurrentLangIndex]["Languagename"];
	}

	//언어 셀렉터 버튼 클릭시 메서드 
	public void LanguageSelecterBtn_pressed(bool IsRight)
	{
		//오른쪽 버튼
		if(IsRight)
		{
			//현재 인덱스 ++1
			CurrentLangIndex += 1;
			//최대값을 넘을 경우,
			if(MaxIndex < CurrentLangIndex)
			{
				//0으로 초기화
				CurrentLangIndex = 0;
			}
			//선택기 언어 갱신
			LangSelecterCurrentLang_txt.text = (string)LBDJ["LanguageCode"][CurrentLangIndex]["Languagename"];
		}
		//왼쪽 버튼
		else
		{
			//현재 인덱스 --1
			CurrentLangIndex -= 1;
			//0보다 작을 경우,
			if(CurrentLangIndex < 0)
			{
				//최대값으로 초기화
				CurrentLangIndex = MaxIndex;
			}
			//선택기 언어 갱신
			LangSelecterCurrentLang_txt.text = (string)LBDJ["LanguageCode"][CurrentLangIndex]["Languagename"];
		}
	}
	//적용 버튼 클릭시
	public void ApplyBtn_pressed()
	{
		LBDJ["CurrentLanguage"] = CurrentLangIndex;
		JsonData lbdj = JsonMapper.ToJson(LBDJ);
		File.WriteAllText(Application.dataPath +"/Data/Language/LanguageCode.json",lbdj.ToString());

		MMUM.LanguagePackRefresher();
		//GameObject.FindObjectOfType<MainMenu_TextOut>().LanguageRefresher();
	}
}