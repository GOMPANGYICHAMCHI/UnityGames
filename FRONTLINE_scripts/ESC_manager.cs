using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.IO;
using UnityEngine.SceneManagement;

public class ESC_manager : MonoBehaviour
{
    public static ESC_manager instance;

	public bool TextReady = false;

    //일시 정지 창이 열려 있는지 확인하는 변수
    bool isPauseOn = false;

    //일시정지 패널
    public GameObject PausePanel;
    //메인메뉴 경고 패널
    public GameObject MainMenuWarning;
    //종료 경고 패널
    public GameObject ExitWarning;

    //텍스트 키와 텍스트값을 저장하는 딕셔너리
	public Dictionary<string,string> LoadedText;

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

    //LoadedText딕셔너리 초기화
	public void LocalizedTextLoad()
	{
		LoadedText = new Dictionary<string,string>();

		//LanguageCode에서 로드한 언어코드를 기반으로 제이슨 텍스트 파일 접근
		string filepath = File.ReadAllText
		(Application.dataPath + "/Data/Language/Menu_Lang/Mlang_"+LanguageCode.LCODE+".json");
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
    
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            //이미 패널이 켜져 있을경우,
            if(isPauseOn)
            {
                //게임 일시정지 해제
                Time.timeScale = 1;
                //게임 패널 비활성화
                PausePanel.SetActive(false);

                //경고창 비활성화
                MainMenuWarning.SetActive(false);
                ExitWarning.SetActive(false);

                //창비활성화
                isPauseOn = false;
            }
            //패널이 꺼져 있을경우
            else
            {
                //게임 일시정지
                Time.timeScale = 0;
                //게임 패널 활성화
                PausePanel.SetActive(true);
                //창활성화
                isPauseOn = true;
            }
        }
    }
    
    //계속하기 버튼 클릭시
    public void ResumeBtn_pressed()
    {
        //게임 일시정지 해제
        Time.timeScale = 1;
        //게임 패널 비활성화
        PausePanel.SetActive(false);

        //경고창 비활성화
        MainMenuWarning.SetActive(false);
        ExitWarning.SetActive(false);

        //창비활성화
        isPauseOn = false;
    }

    //게임 나가기 버튼 클릭시 메서드
    public void ExitBtn_pressed()
    {
        //경고창 활성화
        ExitWarning.SetActive(true);
    }
    //메인메뉴 버튼 클릭시 메서드
    public void MainMenuBtn_pressed()
    {
        //경고창 활성화
        MainMenuWarning.SetActive(true);
    }
    //게임을 종료할지 물어보는 버튼 메서드
    public void ExitChoiceBtn_pressed(bool isExit)
    {
        //나가기가 맞을경우
        if(isExit)
        {
            //프로그램 종료
            Application.Quit();
        }
        //아닐경우
        else
        {
            //경고창 비활성화
            ExitWarning.SetActive(false);
        }
    }
    //메인메뉴로 나갈건지 물어보는 버튼 메서드
    public void MainMenuChoiceBtn_pressed(bool isMainMenu)
    {
        //메인메뉴로 가기가 맞을경우
        if(isMainMenu)
        {
            //로딩 인덱스 변경
			LoadingIndex.LoadSceneIndex = 4;
			//로딩씬 로드
			SceneManager.LoadScene("LoadingScene");
        }
        //아닐경우
        else
        {
            //경고창 비활성화
            MainMenuWarning.SetActive(false);
        }
    }
}