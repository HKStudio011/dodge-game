using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;

public class ADSController : MonoBehaviour
{
    public string openID;
    public string bannerID;
    public string interstitialID;
    public string rewardedID;
    public string rewardedinterstitialID;
    private AppOpenAdManager openAD;
    private GameController controller;
    private SaveSystem saveSystem;
    private Banner banner;
    private Interstitial interstitial;
    private Rewarded reward;
    private RewardedInterstitial rewardinterstitial;
    private bool isLoadRewardedInterstitial;
    // Start is called before the first frame update
    void Start()
    {
        
        Inits();
        saveSystem=new SaveSystem();
        isLoadRewardedInterstitial = false;
        if (SceneManager.GetActiveScene().name.Equals("MainGame"))
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        openAD = new AppOpenAdManager(openID);
        banner = new Banner(bannerID);
        interstitial = new Interstitial(interstitialID);
        reward = new Rewarded(rewardedID);
        rewardinterstitial = new RewardedInterstitial(rewardedinterstitialID);
        RequestConfiguration requestConfiguration =
            new RequestConfiguration.Builder()
            .SetSameAppKeyEnabled(true).build();
        MobileAds.SetRequestConfiguration(requestConfiguration);

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name.Equals("MainGame") &&
            controller.IsGameOver &&!isLoadRewardedInterstitial)
        {
            LoadRewardedInterstitial();
            isLoadRewardedInterstitial = true;
        }
    }
    private void Inits()
    {
#if UNITY_EDITOR
        openID = "ca-app-pub-3940256099942544/3419835294";
        bannerID = "ca-app-pub-3940256099942544/6300978111";
        interstitialID = "ca-app-pub-3940256099942544/1033173712";
        rewardedID = "ca-app-pub-3940256099942544/5224354917";
        rewardedinterstitialID = "ca-app-pub-3940256099942544/5354046379";
#endif
        if (openID.Equals("")) openID = "ca-app-pub-3940256099942544/3419835294";
        if (bannerID.Equals("")) bannerID = "ca-app-pub-3940256099942544/6300978111";
        if (interstitialID.Equals("")) interstitialID = "ca-app-pub-3940256099942544/1033173712";
        if (rewardedID.Equals("")) rewardedID = "ca-app-pub-3940256099942544/5224354917";
        if (rewardedinterstitialID.Equals("")) rewardedinterstitialID = "ca-app-pub-3940256099942544/5354046379";
    }
    public void Destroy()
    {
        openAD.Destroy();
        banner.Destroy();
        interstitial.Destroy(); 
        reward.Destroy();
        rewardinterstitial.Destroy();
    }
    public void LoadOpenAD()
    {
        openAD.Rotatio = ScreenOrientation.Landscape;
        openAD.LoadAd();
    }
    public void LoadBanner()
    {
        
            if (saveSystem.LoadPlayer().Item2) return;
            banner.AdPosition = AdPosition.Bottom;
            banner.AdSize = AdSize.Banner;
            banner.RequestBanner();

    }
    public void LoadInterstitial()
    {
        interstitial.RequestInterstitial();
    }
    public void LoadRewarded()
    {
        controller.LoadAd = true;
        reward.RequestRewarded();
        reward.RewardAd.OnUserEarnedReward += RewardAd_OnUserEarnedReward;
        reward.RewardAd.OnAdFailedToLoad += RewardAd_OnAdFailedToLoad;
        reward.RewardAd.OnAdClosed += RewardAd_OnAdClosed;
    }

    private void RewardAd_OnAdClosed(object sender, EventArgs e)
    {
        controller.LoadAd = false;
    }

    private void RewardAd_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        controller.LoadAd = false;
    }

    private void RewardAd_OnUserEarnedReward(object sender, Reward e)
    {
        controller.LoadAd = false;
        controller.AdRequest = true;
    }

    public void LoadRewardedInterstitial()
    {
        if (saveSystem.LoadPlayer().Item2) return;
        controller.LoadAd = true;
        rewardinterstitial.RequestRewardedInterstitial();
        rewardinterstitial.UserEarnedReward += Rewardinterstitial_UserEarnedReward;
        rewardinterstitial.AdFailedToLoad += Rewardinterstitial_AdFailedToLoad;
        rewardinterstitial.AdClose += Rewardinterstitial_AdClose;
    }

    private void Rewardinterstitial_AdClose(object sender, EventArgs e)
    {
        controller.LoadAd = false;
    }

    private void Rewardedinterstitial_OnAdDidDismissFullScreenContent(object sender, EventArgs e)
    {
        controller.LoadAd = false;
    }

    private void Rewardinterstitial_AdFailedToLoad(object sender, EventArgs e)
    {
        controller.LoadAd = false;
    }

    private void Rewardinterstitial_UserEarnedReward(object sender, EventArgs e)
    {
        controller.LoadAd = false;
        controller.AdRequest = true;
    }
}

public class AppOpenAdManager
{

    private string appOpenID;

    private static AppOpenAdManager instance;

    private static AppOpenAd ad;

    private bool isShowingAd = false;
    private ScreenOrientation rotatio;
    public string AppOpenID { get => appOpenID; }
    public ScreenOrientation Rotatio { get => rotatio; set => rotatio = value; }
    public AppOpenAd Ad { get => ad; set => ad = value; }
    public AppOpenAdManager(string appOpenID)
    {
        this.appOpenID = appOpenID;
    }

    

    private bool IsAdAvailable
    {
        get
        {
            return ad != null;
        }
    }

    

    public void LoadAd()
    {
        AdRequest request = new AdRequest.Builder().Build();

        // Load an app open ad for portrait orientation
        AppOpenAd.LoadAd(appOpenID, Rotatio, request, ((appOpenAd, error) =>
        {
            if (error != null)
            {
                // Handle the error.
                Debug.LogFormat("Failed to load the ad. (reason: {0})", error.LoadAdError.GetMessage());
                return;
            }

            // App open ad is loaded.
            ad = appOpenAd;
            ShowAdIfAvailable();
        }));
    }
    public void ShowAdIfAvailable()
    {
        if (!IsAdAvailable || isShowingAd)
        {
            return;
        }

        ad.OnAdDidDismissFullScreenContent += HandleAdDidDismissFullScreenContent;
        ad.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresentFullScreenContent;
        ad.OnAdDidPresentFullScreenContent += HandleAdDidPresentFullScreenContent;
        //ad.OnAdDidRecordImpression += HandleAdDidRecordImpression;
        //ad.OnPaidEvent += HandlePaidEvent;
        ad.Show();
        
    }
    public void Destroy()
    {
        if(instance!=null)
        {
            instance.Destroy();
            instance = null;
        }
            
        if (ad != null)
        {
            ad.Destroy();
            ad = null;
        }

    }
    private void HandleAdDidDismissFullScreenContent(object sender, EventArgs args)
    {
        Debug.Log("Closed app open ad");
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        ad = null;
        isShowingAd = false;
        LoadAd();
    }

    private void HandleAdFailedToPresentFullScreenContent(object sender, AdErrorEventArgs args)
    {
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        ad = null;
        LoadAd();
    }

    private void HandleAdDidPresentFullScreenContent(object sender, EventArgs args)
    {
        isShowingAd = true;
    }

    private void HandleAdDidRecordImpression(object sender, EventArgs args)
    {

    }

    private void HandlePaidEvent(object sender, AdValueEventArgs args)
    {

    }
}

public class Banner
{
    private static BannerView banner;
    private string bannerID;
    private AdPosition adPosition;
    private AdSize adSize;
    public string BannerID { get => bannerID; set => bannerID = value; }
    public AdPosition AdPosition { get => adPosition; set => adPosition = value; }
    public AdSize AdSize { get => adSize; set => adSize = value; }
    public BannerView BannerAd { get => banner; }

    public Banner(string bannerID)
    {
        this.bannerID = bannerID;
    }

    public void RequestBanner()
    {   
        banner = new BannerView(bannerID, adSize, AdPosition);

        ////call events
        //// Called when an ad request has successfully loaded.
        //banner.OnAdLoaded += this.HandleOnAdLoaded;
        //// Called when an ad request failed to load.
        //banner.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;
        //// Called when an ad is clicked.
        //banner.OnAdOpening += this.HandleOnAdOpened;
        //// Called when the user returned from the app after an ad click.
        //banner.OnAdClosed += this.HandleOnAdClosed;
        //// Called when the ad click caused the user to leave the application.


        //create and ad request
        AdRequest request = new AdRequest.Builder().Build();
        // Load the banner with the request.
        banner.LoadAd(request);
        
    }
    public void Destroy()
    {
        if (banner != null) 
        {
            banner.Destroy();
            banner = null;
        }
        
    }

    //events below
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
       
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        
    }
}
public class Interstitial
{
    private static InterstitialAd interstitial;
    private string interstitialID;

    public string InterstitialID { get => interstitialID; set => interstitialID = value; }
    public InterstitialAd InterstitialAd { get => interstitial;}

    public Interstitial(string interstitialID)
    {
        this.interstitialID = interstitialID;
    }

    public void RequestInterstitial()
    {
        interstitial = new InterstitialAd(interstitialID);

        // Called when an ad request has successfully loaded.
        interstitial.OnAdLoaded += HandleOnAdLoaded;
        //// Called when an ad request failed to load.
        //interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        //// Called when an ad is shown.
        //interstitial.OnAdOpening += HandleOnAdOpened;
        //// Called when the ad is closed.
        //interstitial.OnAdClosed += HandleOnAdClosed;
        

        //create and ad request
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        interstitial.LoadAd(request);
       
    }
    public void Destroy()
    {
        if(interstitial != null)
        {
            interstitial.Destroy();
            interstitial = null;
        }
        
    }
    //show the ad
    private void ShowInterstitial()
    {
        if (interstitial.IsLoaded())
        {
            interstitial.Show();
        }
        
    }

    //events below
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        ShowInterstitial();
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        
    }
}

public class Rewarded
{
    private static RewardedAd rewardAd;
    private string rewardID;
    public string RewardID { get => rewardID; set => rewardID = value; }
    public RewardedAd RewardAd { get => rewardAd;}

    public Rewarded(string rewardID)
    {
        this.rewardID = rewardID;
    }

    public void RequestRewarded()
    {
        rewardAd = new RewardedAd(rewardID);

        // Called when an ad request has successfully loaded.
        rewardAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        //rewardAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        //// Called when an ad is shown.
        //rewardAd.OnAdOpening += HandleRewardedAdOpening;
        //// Called when an ad request failed to show.
        //rewardAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        //// Called when the user should be rewarded for interacting with the ad.
        //rewardAd.OnUserEarnedReward += HandleUserEarnedReward;
        //// Called when the ad is closed.
        //rewardAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        rewardAd.LoadAd(request); //load & show the banner ad
        
    }
    public void Destroy()
    {
        if(rewardAd != null)
        {
            rewardAd.Destroy();
            rewardAd = null;
        }
        
    }
    //attach to a button that plays ad if ready
    private void ShowRewarded()
    {
        if (rewardAd.IsLoaded())
        {
            rewardAd.Show();
        }
    }

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        ShowRewarded();
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
       
    }
}

public class RewardedInterstitial
{
    private static RewardedInterstitialAd rewardedinterstitial;
    private string rewardedinterstitialID;
    private event EventHandler eUserEarnedReward;
    
    public event EventHandler UserEarnedReward
    {
        add
        {
            eUserEarnedReward += value;
        }
        remove
        {
            eUserEarnedReward -= value;
        }
    }
    private event EventHandler eAdFailedToLoad;
    public event EventHandler AdFailedToLoad
    {
        add
        {
            eAdFailedToLoad += value;
        }
        remove
        {
            eAdFailedToLoad -= value;
        }
    }
    private event EventHandler eAdClose;
    public event EventHandler AdClose
    {
        add
        {
            eAdClose += value;
        }
        remove
        {
            eAdClose -= value;
        }
    }
    private void OnUserEarnedReward()
    {
        if(eUserEarnedReward != null) eUserEarnedReward(this,new EventArgs());
    }
    private void OnAdFailedToLoad()
    {
        if (eAdFailedToLoad != null) eAdFailedToLoad(this, new EventArgs());
    }
    private void OnAdClose()
    {
        if (eAdClose != null) eAdClose(this, new EventArgs());
    }


    public string RewardedinterstitialID { get => rewardedinterstitialID; set => rewardedinterstitialID = value; }
    public RewardedInterstitialAd Rewardedinterstitial { get => rewardedinterstitial;}

    public RewardedInterstitial(string rewardedinterstitialID)
    {
        this.rewardedinterstitialID = rewardedinterstitialID;
    }
    public void Destroy()
    {
        if(rewardedinterstitial != null)
        {
            rewardedinterstitial.Destroy();
            rewardedinterstitial = null;
        }
        
    }
    public void RequestRewardedInterstitial()
    {

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        RewardedInterstitialAd.LoadAd(rewardedinterstitialID,request, adLoadCallback);
    }

    private void adLoadCallback(RewardedInterstitialAd ad, AdFailedToLoadEventArgs error)
    {
        if (error == null)
        {
            rewardedinterstitial = ad;
            rewardedinterstitial.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresent;
            //rewardedinterstitial.OnAdDidPresentFullScreenContent += HandleAdDidPresent;
            rewardedinterstitial.OnAdDidDismissFullScreenContent += HandleAdDidDismiss;
            //rewardedinterstitial.OnPaidEvent += HandlePaidEvent;
            ShowRewardedInterstitialAd();
        }
        else OnAdFailedToLoad();
    }

    //show the ad
    private void ShowRewardedInterstitialAd()
    {
        if (rewardedinterstitial != null)
        {
            rewardedinterstitial.Show(userEarnedRewardCallback);
        }
        else OnAdFailedToLoad();
    }

    private void userEarnedRewardCallback(Reward reward)
    {
        // TODO: Reward the user.
        OnUserEarnedReward();
    }

    private void HandleAdFailedToPresent(object sender, AdErrorEventArgs args)
    {
        
    }

    private void HandleAdDidPresent(object sender, EventArgs args)
    {
        
    }

    private void HandleAdDidDismiss(object sender, EventArgs args)
    {
        OnAdClose();
    }

    private void HandlePaidEvent(object sender, AdValueEventArgs args)
    {
        
    }

}
