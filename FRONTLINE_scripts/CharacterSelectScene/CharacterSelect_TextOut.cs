using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect_TextOut : MonoBehaviour 
{
	public string Key;

	void Start () 
	{
		Text text = GetComponent<Text>();
		text.text = CharacterSelectUI_Manager.instance.GetLocalizedValue(Key);
	}
}
