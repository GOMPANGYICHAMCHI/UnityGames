using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class System_manager : MonoBehaviour 
{
	[SerializeField]
	private sound_manager soundmanager;

	//public bool paused = false;
	public UI_manager ui_manager;
	public GameObject[] spawners = new GameObject[4];

	public enum game_state
	{
		started , gameover
	}
	public GameObject score_panel;
	public GameObject highscore_panel;

	public GameObject all_delete;

	//public done dn;
	private bool can_spawn;

	[SerializeField]
	private float wait_time;

	public GameObject blue_car;
	public GameObject red_car;

	[SerializeField]
	private GameObject highscore_;

	public game_state gs;

	public float car_speed = 0;

	public int score = 0;

	public int highscore = 0;

	public Text score_txt;

	public Text highscore_txt;

	//public Text speed_txt;
	//public Text dillay_txt;

	public Text offhighscore_txt;
	public Text offscore_txt;

	public Button Exit_btn;

	public Button start_btn;

	public Button retry_btn;

	public Button mm_btn;

	void Start()
	{
		highscore = PlayerPrefs.GetInt("HighScore",0);
		PlayerPrefs.Save();
		//paused = false;
		ui_manager = FindObjectOfType<UI_manager>();
		car_speed = 5;
		//dn = FindObjectOfType<done>();
		can_spawn = true;
		wait_time = 0.8f;
		gs = game_state.gameover;
		start_btn.onClick.AddListener(start_btn_onclicked);
		retry_btn.onClick.AddListener(start_btn_onclicked);
		Exit_btn.onClick.AddListener(Exit_game);
		mm_btn.onClick.AddListener(mainmenubtn_onclicked);
	}

	void Update()
	{
		if(gs == game_state.started)//&&paused == false)
		{
			level_increase();
			displaying();
			//spawn_cars();
			StartCoroutine(spawning());
		}
	}

	void mainmenubtn_onclicked()
	{
		ui_manager.mainmenu();
	}

	void displaying()
	{
		if(score > highscore)
		{
			highscore = score;
			PlayerPrefs.SetInt("HighScore",highscore);
		}
		score_txt.text = ("SCORE : "+score);
		highscore_txt.text = ("HIGHSCORE : "+highscore);
		//dillay_txt.text = ("dillay : "+wait_time);
		//speed_txt.text = ("speed : "+car_speed);
	}

	void start_btn_onclicked()
	{
		highscore_.SetActive(false);
		soundmanager.btnpress_sound();
		ui_manager.yellow_panel.gameObject.SetActive(false);
		ui_manager.pause_btn.gameObject.SetActive(true);
		ui_manager.num_btn.SetActive(true);
		ui_manager.main_hide();
		ui_manager.gameover_hide();
		score_panel.SetActive(true);
		highscore_panel.SetActive(true);
		car_speed =5;
		all_delete.SetActive(false);
		score = 0;
		//dn.done_ = false;
		start_btn.gameObject.SetActive(false);
		gs = game_state.started;
	}

	public void game_over()
	{
		ui_manager.gameover();
		offscore_txt.text = ("SCORE : "+score);
		score_panel.SetActive(false);
		highscore_panel.SetActive(false);

		car_speed =5;
		all_delete.SetActive(true);
		//dn.done_ = true;
		wait_time = 1;
		gs=game_state.gameover;
		if(score == highscore&&score != 0)
		{
			PlayerPrefs.SetInt("HighScore",highscore);
			int score = PlayerPrefs.GetInt("HighScore",0);
			PlayManager.AddScoreToLeaderboard(score,GPGSIds.leaderboard_score);
			highscore_.SetActive(true);
			soundmanager.highscore_sound();
			highscore = score;
			highscore_txt.text = ("HIGHSCORE : "+highscore);
		}
		offhighscore_txt.text = ("HIGHSCORE : "+highscore);
	}

	public void pause_over()
	{
		if(score == highscore)
		{
			PlayerPrefs.SetInt("HighScore",highscore);
			int score = PlayerPrefs.GetInt("HighScore",0);
			PlayManager.AddScoreToLeaderboard(score,GPGSIds.leaderboard_score);
		}
		score_panel.SetActive(false);
		highscore_panel.SetActive(false);
		car_speed =5;
		all_delete.SetActive(true);
		//dn.done_ = true;
		wait_time = 1;
		gs=game_state.gameover;
	}

	public void blueover()
	{
		soundmanager.beep_sound();
		ui_manager.pause_btn.gameObject.SetActive(false);
		score_panel.SetActive(false);
		highscore_panel.SetActive(false);
	}

	void spawn_cars()
	{
		int spawner_index = Random.Range(0,5);
		int car_index = Random.Range(0,2);

		Vector3 spawn_pos = new Vector3(spawners[spawner_index].transform.position.x,0.5f,spawners[spawner_index].transform.position.z);
		if(car_index == 0)
		{
			Instantiate(blue_car,spawn_pos,blue_car.transform.rotation);
		}
		else
		{
			Instantiate(red_car,spawn_pos,red_car.transform.rotation);
		}
	}

	void level_increase()
	{
		if(car_speed < 25)
		{
			car_speed += 0.08f * Time.deltaTime;
		}
		if(wait_time > 0.25)
		{
			wait_time -= 0.006f * Time.deltaTime;
		}
	}

	IEnumerator spawning()
	{
		if(can_spawn == true)
		{
			can_spawn = false;
			spawn_cars();
			yield return new WaitForSeconds(wait_time);
			can_spawn = true;
		}
	}
	void Exit_game()
	{
		soundmanager.btnpress_sound2();
		Application.Quit();
	}	
}