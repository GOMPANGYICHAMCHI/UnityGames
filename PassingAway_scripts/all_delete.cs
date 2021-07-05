using UnityEngine;

public class all_delete : MonoBehaviour 
{
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "car_blue")
		{
			Destroy(other.gameObject);
		}
		if(other.gameObject.tag == "car_red")
		{
			Destroy(other.gameObject);
		}
	}
}