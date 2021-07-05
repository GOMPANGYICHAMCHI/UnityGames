using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class passer_controller : MonoBehaviour 
{
	[SerializeField]
	private sound_manager soundmanager;
	public GameObject up_img;
	public GameObject down_img;

	public System_manager SM;
	public Animator passing_bar;

	public Button passer_btn;

	public KeyCode numcode;

	public bool isbarup;

	void Start()
	{
		passer_btn.onClick.AddListener(passer_btnon);
		passing_bar = gameObject.GetComponent<Animator>();
		isbarup = false;
	}

	public void Update()
	{
		if(SM.gs == System_manager.game_state.gameover)
		{
			passing_bar.SetBool("OnClicked",false);
			down_img.SetActive(true);
			up_img.SetActive(false);
			isbarup = false;
		}
		/*else
		{
			if(Input.GetKeyDown(numcode))//&&SM.paused == false)
			{
				if(isbarup == true)
				{
					//soundmanager.passerup_sound();
					soundmanager.btnpress_sound();
					down_img.SetActive(true);
					up_img.SetActive(false);
					passing_bar.SetBool("OnClicked",false);
					isbarup = false;
				}
				else
				{
					//soundmanager.passerup_sound();
					soundmanager.btnpress_sound2();
					down_img.SetActive(false);
					up_img.SetActive(true);
					passing_bar.SetBool("OnClicked",true);
					isbarup = true;
				}
			}
		}*/
	}
	void passer_btnon()
	{
		if(isbarup == true)//&&SM.paused==false)
		{
			passer_up();
		}
		else//SM.paused==false)
		{
			passer_down();
		}
	}
	void passer_up()
	{
		soundmanager.btnpress_sound();
		//soundmanager.passerup_sound();
		down_img.SetActive(true);
		up_img.SetActive(false);
		passing_bar.SetBool("OnClicked",false);
		isbarup = false;
	}
	void passer_down()
	{
		soundmanager.btnpress_sound2();
		//soundmanager.passerup_sound();
		down_img.SetActive(false);
		up_img.SetActive(true);
		passing_bar.SetBool("OnClicked",true);
		isbarup = true;
	}
}