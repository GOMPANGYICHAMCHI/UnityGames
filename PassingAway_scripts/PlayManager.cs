using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class PlayManager : MonoBehaviour 
{
	void Start () 
	{
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
		PlayGamesPlatform.InitializeInstance(config);
		PlayGamesPlatform.DebugLogEnabled = true;
		PlayGamesPlatform.Activate();

		SignIn();
	}

	public static void SignIn()
	{
#if UNITY_ANDROID

		PlayGamesPlatform.Instance.Authenticate((bool success) =>
		{
			if (success)
            {
				Debug.Log("Login success");
				string userInfo = "Username: " + Social.localUser.userName +
                        "\nUser ID: " + Social.localUser.id +
                        "\nIsUnderage: " + Social.localUser.underage;
                // to do ...
                // 구글 로그인 성공 처리
            }
            else
            {
				Debug.Log("Login failed");
                // to do ...
                // 구글 로그인 실패 처리
            }	
		});
#else

		Social.localUser.Authenticate((bool success) => 
		{
			if (success)
            {
				Debug.Log("Login success");
				string userInfo = "Username: " + Social.localUser.userName +
                        "\nUser ID: " + Social.localUser.id +
                        "\nIsUnderage: " + Social.localUser.underage;
                // to do ...
                // 로그인 성공 처리
            }
            else
            {
				Debug.Log("Login failed");
                // to do ...
                // 로그인 실패 처리
            }
		});


#endif
	}

	#region Leaderboard
	public static void AddScoreToLeaderboard(long score,string leaderboardId)
	{
		Social.ReportScore(score,leaderboardId,(bool success) => {});
	}

	public static void ShowLeaderboardUI()
	{
		Social.ShowLeaderboardUI();
		long highscore = PlayerPrefs.GetInt("HighScore",0);
		AddScoreToLeaderboard(highscore,GPGSIds.leaderboard_score);
		//Debug.Log("HI google");
	}
	#endregion /Leaderboard
}
