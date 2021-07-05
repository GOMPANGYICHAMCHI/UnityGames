using UnityEngine;
using UnityEngine.UI;

public class MainMenu_TextOut : MonoBehaviour 
{
	public string Key;

	void Start () 
	{
		Text text = GetComponent<Text>();
		text.text = MainMenuUI_manager.instance.GetLocalizedValue(Key);
	}

	void Update()
	{
		Text text = GetComponent<Text>();
		text.text = MainMenuUI_manager.instance.GetLocalizedValue(Key);
	}
}
