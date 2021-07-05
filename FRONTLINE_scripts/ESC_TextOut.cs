using UnityEngine;
using UnityEngine.UI;

public class ESC_TextOut : MonoBehaviour 
{
	public string Key;

	void Start () 
	{
		Text text = GetComponent<Text>();
		text.text = ESC_manager.instance.GetLocalizedValue(Key);
	}
}
