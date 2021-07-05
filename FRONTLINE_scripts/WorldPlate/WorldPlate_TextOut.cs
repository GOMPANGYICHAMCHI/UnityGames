using UnityEngine;
using UnityEngine.UI;

public class WorldPlate_TextOut : MonoBehaviour 
{
	public string Key;

	void Start () 
	{
		Text text = GetComponent<Text>();
		text.text = WorldPlateUI_Manager.instance.GetLocalizedValue(Key);
	}
}
