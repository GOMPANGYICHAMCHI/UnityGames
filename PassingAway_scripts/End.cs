using UnityEngine;

public class End : MonoBehaviour 
{
	[SerializeField]
	private System_manager SM;

	[SerializeField]
	private sound_manager soundmanager;

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "car_red")
		{
			soundmanager.car_sound();
			SM.game_over();
			Destroy(other.gameObject);
		}
		else if(other.gameObject.tag == "car_blue")
		{
			soundmanager.car_sound();
			SM.score +=1;
			Destroy(other.gameObject);
		}
	}
}