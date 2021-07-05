using UnityEngine;

public class sound_manager : MonoBehaviour
{
	[SerializeField]
	private AudioSource btn_press;
	
	[SerializeField]
	private AudioSource btn_press2;

	[SerializeField]
	private AudioSource passer_up;

	[SerializeField]
	private AudioSource passer_down;

	[SerializeField]
	private AudioSource highscore_;

	[SerializeField]
	private AudioSource cardoppler;

	[SerializeField]
	private AudioSource carsound;

	[SerializeField]
	private AudioSource beepsound;

	public void beep_sound()
	{
		beepsound.Play();
	}

	public void car_sound()
	{
		carsound.Play();
	}

	public void car_doppler()
	{
		cardoppler.Play();
	}

	public void btnpress_sound()
	{
		btn_press.Play();
	}
	public void btnpress_sound2()
	{
		btn_press2.Play();
	}

	public void passerup_sound()
	{
		passer_up.Play();
	}
	public void passerdown_sound()
	{
		passer_down.Play();
	}

	public void highscore_sound()
	{
		highscore_.Play();
	}
	
}