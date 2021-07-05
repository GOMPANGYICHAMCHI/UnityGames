[System.Serializable]
public class LocalizationData
{
	public LocalizationText[] UIINFO;
}

[System.Serializable]
public class LocalizationText
{
	public string key;
	public string value;
}