using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
public class PlayGameServiceController : MonoBehaviour
{
    private string leaderboardID = "CgkIuMX74_wZEAIQBA";
    private static Hashtable achievementID;
    private static PlayGamesPlatform platform;
    private static PlayGamesClientConfiguration configuration;
    public ADSController ads;
    void Start()
    {
        if(configuration == null)
        {
            Configure();
        }
        
        ads = GameObject.FindGameObjectWithTag("ADS").GetComponent<ADSController>();
        if (platform == null)
        {
            if (achievementID == null) AddList();
            Login(SignInInteractivity.CanPromptOnce);   
        }
        else ads.LoadBanner();
    }
    public void AddList()
    {
        achievementID = new Hashtable();
        achievementID.Add("Newbie", "CgkIuMX74_wZEAIQAA");
        achievementID.Add("Pro Player", "CgkIuMX74_wZEAIQAQ");
        achievementID.Add("God", "CgkIuMX74_wZEAIQAg");
        achievementID.Add("Remove ADS", "CgkIuMX74_wZEAIQAw");
    }
    public void Configure()
    {
        configuration = new PlayGamesClientConfiguration.Builder()
      // enables saving game progress.
      //.EnableSavedGames()
      // requests the email address of the player be available.
      // Will bring up a prompt for consent.
      //.RequestEmail()
      // requests a server auth code be generated so it can be passed to an
      //  associated back end server application and exchanged for an OAuth token.
      //.RequestServerAuthCode(false)
      // requests an ID token be generated.  This OAuth token can be used to
      //  identify the player to other services such as Firebase.
      //.RequestIdToken()
      .Build();
    }
    public void Login(SignInInteractivity inInteractivity)
    {
        
        PlayGamesPlatform.InitializeInstance(configuration);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        platform = PlayGamesPlatform.Activate();
        // authenticate user:
        PlayGamesPlatform.Instance.Authenticate(inInteractivity, (result) => 
        {
            // handle results
            ads.LoadBanner();
            Debug.Log(result);
        });
    }
    public void AddScoreToLeaderboard(string score)
    {
        long playerScore=(long)(float.Parse(score));
        if (Social.Active.localUser.authenticated)
        {
            Social.ReportScore(playerScore, leaderboardID, success => { });
        }
    }

    public void ShowLeaderboard()
    {
        ads.Destroy();
        if (Social.Active.localUser.authenticated)
        {
            Social.ShowLeaderboardUI();
        }
    }

    public void ShowAchievements()
    {
        ads.Destroy();
        if (Social.Active.localUser.authenticated)
        {           
            Social.ShowAchievementsUI();
        }
    }

    public void UnlockAchievement(string Key)
    {
        if (Social.Active.localUser.authenticated)
        {
            if(achievementID.ContainsKey(Key))
                Social.ReportProgress((string)achievementID[Key], 100f, success => {});
        }
    }
    // set local play game service
    public void SetLocal(Gravity local)
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                ((GooglePlayGames.PlayGamesPlatform)Social.Active).SetGravityForPopups(local);
            }
        });
    }
}