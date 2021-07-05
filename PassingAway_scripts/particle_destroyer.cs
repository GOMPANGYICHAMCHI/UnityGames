using UnityEngine;
using System.Collections;

public class particle_destroyer : MonoBehaviour 
{
	void Start()
	{
		StartCoroutine(particle_destroy());
	}

	IEnumerator particle_destroy()
	{
		yield return new WaitForSeconds(0.6f);
		Destroy(gameObject);
	}
}