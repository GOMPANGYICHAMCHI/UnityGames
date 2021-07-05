using UnityEngine;
using UnityEngine.Advertisements;
 
public class advertiser_manager : MonoBehaviour
{
    private const string android_game_id = "2583905";
   // private const string ios_game_id = "2583907";
 
    private const string rewarded_video_id = "rewardedVideo";

    private const string video = "video";
 
    void Start()
    {
        Initialize();
    }
 
    private void Initialize()
    {
		#if UNITY_ANDROID
      	  	Advertisement.Initialize(android_game_id);
		/*#elif UNITY_IOS
       		Advertisement.Initialize(ios_game_id);*/
		#endif
    }

    public void ShowAd()
    {
        if ( Advertisement.IsReady(video) )
        {
            Advertisement.Show(video);
        }
    }
 
    public void ShowRewardedAd()
    {
        if (Advertisement.IsReady(rewarded_video_id))
        {
            Advertisement.Show(rewarded_video_id);
        }
    }
}
