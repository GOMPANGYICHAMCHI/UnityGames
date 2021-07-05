using UnityEngine;
using UnityEngine.UI;

public class TextOut : MonoBehaviour 
{
	public string Key;

	void Start () 
	{
		Text text = GetComponent<Text>();
		text.text = PhaseUI_Manager.instance.GetLocalizedValue(Key);
	}
	
}
