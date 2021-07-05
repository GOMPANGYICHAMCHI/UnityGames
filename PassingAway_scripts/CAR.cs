using UnityEngine;
using System.Collections;

public class CAR : MonoBehaviour 
{
	/*enum CAR_COLOR
	{
		blue , red
	}*/

	//private bool death = false;

	//[SerializeField]
	//private CAR_COLOR car_color;

	//[SerializeField]
	//private GameObject Destroy_particle;

	[SerializeField]
	private System_manager SM;

	//private done dn;

	void Start()
	{
		//dn = FindObjectOfType<done>();
		SM = FindObjectOfType<System_manager>();
	}

	void Update()
	{
		//if(SM.paused == false)
		//{
		//if(death == false)
		transform.Translate(Vector3.back * SM.car_speed * Time.deltaTime);
		//}
	}

	

	/*void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "pass_bar")//&&dn.done_ == false)
		{
			if(car_color == CAR_COLOR.red)
			{
				SM.score +=1;
				Instantiate(Destroy_particle,transform.position,Destroy_particle.transform.rotation);
				Destroy(gameObject);
			}
			else
			{
				StartCoroutine(bluebreak_gameover());
			}
		}
		else if(other.gameObject.tag == "end")
		{
			if(car_color == CAR_COLOR.blue)
			{
				SM.score +=1;
				//Instantiate(Destroy_particle,transform.position,Destroy_particle.transform.rotation);
				Destroy(gameObject);
			}
			else
			{
				SM.game_over();
				//Instantiate(Destroy_particle,transform.position,Destroy_particle.transform.rotation);
				Destroy(gameObject);
			}
		}*/

	/*IEnumerator bluebreak_gameover()
	{
		death = true;
		SM.blueover();
		SM.all_delete.SetActive(true);
		yield return new WaitForSeconds(1);
		Instantiate(Destroy_particle,transform.position,Destroy_particle.transform.rotation);
		SM.game_over();
		Destroy(gameObject);
	}*/
}