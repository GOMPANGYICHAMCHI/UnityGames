using UnityEngine;
using UnityEngine.UI;
public class BaseMent_TextOut : MonoBehaviour 
{

	public string Key;

	void Start () 
	{
		Text text = GetComponent<Text>();
		text.text = BaseMentUI_Manager.instance.GetLocalizedValue(Key);
	}
}
