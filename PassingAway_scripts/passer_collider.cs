using System.Collections;
using UnityEngine;

public class passer_collider : MonoBehaviour 
{
	[SerializeField]
	private System_manager SM;
	
	[SerializeField]
	private GameObject DestroyParticle_blue;

	[SerializeField]
	private GameObject DestroyParticle_red;

	void Start()
	{
		SM = FindObjectOfType<System_manager>();
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "car_blue")
		{
			other.gameObject.tag = "car";
			CAR car = other.gameObject.GetComponent<CAR>();
			Destroy(car);
			StartCoroutine(bluebreak_gameover(other.gameObject));
		}
		else if(other.gameObject.tag == "car_red")
		{
			SM.score +=1;
			Instantiate(DestroyParticle_red,other.transform.position,DestroyParticle_red.transform.rotation);
			Destroy(other.gameObject);
		}
	}

	IEnumerator bluebreak_gameover(GameObject car_obj)
	{
		SM.blueover();
		SM.all_delete.SetActive(true);
		yield return new WaitForSeconds(1);
		Instantiate(DestroyParticle_blue,transform.position,DestroyParticle_blue.transform.rotation);
		SM.game_over();
		Destroy(car_obj);
	}
}