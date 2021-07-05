using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class UI_manager : MonoBehaviour 
{
	[SerializeField]
	private sound_manager soundmanager;

	[SerializeField]
	private advertiser_manager AM;
	public Text mainhighscore_txt;
	public System_manager SM;
	public GameObject pause_panel;
	public GameObject credit_panel;
	public GameObject num_btn;
	private float wait;

	public Animator mainhighscore;
	public Animator gameover_panel;
	public Animator logo_panel;
	public Animator start_btn;
	public Animator exit_btn;
	public Animator retry_btn;
	public Animator mm_btn;
	public Animator score_anim;
	public Animator highscore_anim;
	public Animator credit_anim;
	public Animator yellow_panel;
	public Animator deletead;
	public Animator leaderboard;
	public Animator tutorialbtn_anim;

	[SerializeField]
	private Button youtube_btn;

	public Button delete_ad;

	[SerializeField]
	private Button leaderboard_btn;

	[SerializeField]
	private Button tutorial_btn;

	//[SerializeField]
	//private Button tutorialback_btn;

	[SerializeField]
	private GameObject tutorial_panel;

	public Button pause_btn;
	public Button credit_btn;
	public Button back_btn;
	public Button resume_btn;
	public Button pausemainmenu_btn;

	public bool ad_off = false;

	//public string LeaderboardId = "CgkIsKvgm5sNEAIQAA";

	void Start()
	{
		int AD_conf = PlayerPrefs.GetInt("AD_deleted",0);
		if(AD_conf == 1)
		{
			ad_off = true;
		}
		//tutorialback_btn.onClick.AddListener(tutorial_backbtn_onclicked);
		leaderboard_btn.onClick.AddListener(leaderboardbtn_onclicked);
		youtube_btn.onClick.AddListener(linkto_youtube);
		resume_btn.onClick.AddListener(resumebtn_onclicked);
		pausemainmenu_btn.onClick.AddListener(pausemainmenu_onclicked);
		credit_btn.onClick.AddListener(credit_btn_onclicked);
		back_btn.onClick.AddListener(backbtn_onclicked);
		pause_btn.onClick.AddListener(pausebtn_onclicked);
		main_animation();
	}

	public void tutorialbtn_onclicked()
	{
		tutorial_panel.SetActive(true);
		soundmanager.btnpress_sound();
	}

	public void tutorial_backbtn_onclicked()
	{
		tutorial_panel.SetActive(false);
		soundmanager.btnpress_sound();
	}

	public void leaderboardbtn_onclicked()
	{
		soundmanager.btnpress_sound();
		PlayManager.SignIn();
		PlayManager.ShowLeaderboardUI();
	}
	void linkto_youtube()
	{
		soundmanager.btnpress_sound();
		Application.OpenURL("https://www.youtube.com/channel/UCAss_Uz1fyUxSd-I94Q8U-g?view_as=subscriber");
	}

	public void pausemainmenu_onclicked()
	{
		soundmanager.btnpress_sound2();
		pause_panel.SetActive(false);
		num_btn.SetActive(false);
		Time.timeScale = 1;
		SM.pause_over();
		mainmenu();
	}

	public void pausebtn_onclicked()
	{
		soundmanager.btnpress_sound();
		//SM.paused = true;
		Time.timeScale = 0;
		pause_panel.SetActive(true);
		pause_btn.gameObject.SetActive(false);
	}

	public void resumebtn_onclicked()
	{
		soundmanager.btnpress_sound();
		//SM.paused = false;
		Time.timeScale = 1;
		pause_panel.SetActive(false);
		pause_btn.gameObject.SetActive(true);
	}

	void credit_btn_onclicked()
	{
		soundmanager.btnpress_sound();
		credit_panel.SetActive(true);
		credit_btn.gameObject.SetActive(false);
		delete_ad.gameObject.SetActive(false);
	}

	void backbtn_onclicked()
	{
		soundmanager.btnpress_sound2();
		credit_panel.SetActive(false);
		credit_btn.gameObject.SetActive(true);
		delete_ad.gameObject.SetActive(true);
	}

	public void main_animation()
	{
		tutorial_btn.gameObject.SetActive(true);
		leaderboard_btn.gameObject.SetActive(true);
		mainhighscore.gameObject.SetActive(true);
		mainhighscore_txt.text = ("HIGHSCORE : " + PlayerPrefs.GetInt("HighScore"));
		logo_panel.gameObject.SetActive(true);
		start_btn.gameObject.SetActive(true);
		exit_btn.gameObject.SetActive(true);
		credit_anim.gameObject.SetActive(true);
		delete_ad.gameObject.SetActive(true);
		tutorialbtn_anim.SetTrigger("tutorialbtn_on");
		leaderboard.SetTrigger("leaderboard_on");
		mainhighscore.SetTrigger("mainhighscore_on");
		credit_anim.SetTrigger("credit_on");
		logo_panel.SetTrigger("logo_on");
		start_btn.SetTrigger("str_on");
		exit_btn.SetTrigger("exit_on");
		deletead.SetTrigger("deletead_on");
	}

	public void main_hide()
	{
		tutorial_btn.gameObject.SetActive(false);
		leaderboard.gameObject.SetActive(false);
		mainhighscore.gameObject.SetActive(false);
		credit_anim.gameObject.SetActive(false);
		logo_panel.gameObject.SetActive(false);
		start_btn.gameObject.SetActive(false);
		exit_btn.gameObject.SetActive(false);
		delete_ad.gameObject.SetActive(false);
	}

	public void gameover()
	{
		pause_btn.gameObject.SetActive(false);
		num_btn.SetActive(false);
		retry_btn.gameObject.SetActive(true);
		mm_btn.gameObject.SetActive(true);
		gameover_panel.gameObject.SetActive(true);

		score_anim.gameObject.SetActive(true);
		highscore_anim.gameObject.SetActive(true);

		score_anim.SetTrigger("score_on");
		highscore_anim.SetTrigger("highscore_on");

		retry_btn.SetTrigger("retry_on");
		mm_btn.SetTrigger("mm_on");
		gameover_panel.SetTrigger("gameover_on");

		if(SM.score == SM.highscore&&ad_off == false&&SM.highscore > 50)
		{
			StartCoroutine(Rewardedadvertiser());
		}
		else if(SM.score > 50&&ad_off == false)
		{
			StartCoroutine(advertiser());
		}
	}

	public void gameover_hide()
	{	
		score_anim.gameObject.SetActive(false);
		highscore_anim.gameObject.SetActive(false);
		retry_btn.gameObject.SetActive(false);
		mm_btn.gameObject.SetActive(false);
		gameover_panel.gameObject.SetActive(false);
	}

	public void mainmenu()
	{
		soundmanager.btnpress_sound2();
		gameover_hide();
		yellow_panel.gameObject.SetActive(true);
		yellow_panel.SetBool("yellow_on",true);
		soundmanager.car_doppler();
		StartCoroutine(sometime());
	}

	IEnumerator sometime()
	{
		yield return new WaitForSeconds(1);
		yellow_panel.SetBool("yellow_on",false);
		yield return new WaitForSeconds(1.7f);
		main_animation();
		yellow_panel.gameObject.SetActive(false);
	}

	IEnumerator Rewardedadvertiser()
	{
		yield return new WaitForSeconds(0.9f);
		if(SM.gs == System_manager.game_state.started)
		{
			pausebtn_onclicked();
		}
		AM.ShowRewardedAd();
	}

	IEnumerator advertiser()
	{
		yield return new WaitForSeconds(0.9f);
		if(SM.gs == System_manager.game_state.started)
		{
			pausebtn_onclicked();
		}
		AM.ShowAd();
	}
	/*IEnumerator credit()
	{
		credit_anim.gameObject.SetActive(false);
		yield return new WaitForSeconds(2);
		yellow_panel.SetBool("yellow_on",false);
		yield return new WaitForSeconds(1);
		credit_panel.SetActive(true);
	}*/


}