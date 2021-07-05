using System.Collections;
//using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading_Manager : MonoBehaviour
{
	//로딩 게이지 슬라이더
	public Slider LoadingSlider;
	//로딩이 끝났는지 확인하는 변수
    bool IsDone = false;
	//진행 시간
    float fTime = 0f;
	//불러올 씬 이름
	string SceneName;
    AsyncOperation async_operation;
 
    void Start()
    {
		//현재 씬 인덱스를 씬 이름으로 바꾸어주는 메서드
		switch (LoadingIndex.LoadSceneIndex)
		{
			//새게임(캐릭터 선택창 불러오기)
			case 0 :	
				SceneName = "CharacterSelect";
				break;
			//기지씬 불러오기
			case 1 :
				SceneName = "BaseMent";
				break;
			//전투진입(배틀플레이트 불러오기)
			case 2 :
				SceneName = "BattlePlate";
				break;
			//월드플레이트 불러오기
			case 3 : 
				SceneName = "WorldPlate";
				break;
            //메인화면 불러오기
			case 4 : 
				SceneName = "MainMenu";
				break;
		}

        StartCoroutine(StartLoad(SceneName));
    }
 
    void Update()
    {
        fTime += Time.deltaTime;
        LoadingSlider.value = fTime;
 
        if (fTime >= 5)
        {
            async_operation.allowSceneActivation = true;
        }
    }
	//씬로드
    public IEnumerator StartLoad(string strSceneName)
    {
        async_operation = SceneManager.LoadSceneAsync(strSceneName);
        async_operation.allowSceneActivation = false;
 
        if (IsDone == false)
        {
            IsDone = true;
 
            while (async_operation.progress < 0.9f)
            {
                LoadingSlider.value = async_operation.progress;
 
                yield return true;
            }
        }
    }
}