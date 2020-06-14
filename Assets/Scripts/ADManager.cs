using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class ADManager : MonoBehaviour {

    public GameMech gameMechScript;
    private RewardBasedVideoAd RBV_ad;
    private string adunitId = "ca-app-pub-9765432280435090/6914067763";

    void Start () {
        RBV_ad = RewardBasedVideoAd.Instance;
        RBV_ad.OnAdClosed += RBV_ad_OnAdClosed;
        RBV_ad.OnAdFailedToLoad += RBV_ad_OnAdFailedToLoad;
        RBV_ad.OnAdLeavingApplication += RBV_ad_OnAdLeavingApplication;
        RBV_ad.OnAdLoaded += RBV_ad_OnAdLoaded;
        RBV_ad.OnAdOpening += RBV_ad_OnAdOpening;
        RBV_ad.OnAdRewarded += RBV_ad_OnAdRewarded;
        RBV_ad.OnAdStarted += RBV_ad_OnAdStarted;
    }

    private void RBV_ad_OnAdStarted(object sender, System.EventArgs e)
    {

    }

    private void RBV_ad_OnAdRewarded(object sender, Reward e)
    {
        //Give Reward
        //ExtraLife();
        //gameMechScript.triggerExtraLife = true;
    }

    private void RBV_ad_OnAdOpening(object sender, System.EventArgs e)
    {

    }

    private void RBV_ad_OnAdLoaded(object sender, System.EventArgs e)
    {

    }

    private void RBV_ad_OnAdLeavingApplication(object sender, System.EventArgs e)
    {

    }

    private void RBV_ad_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        //Reset Game
        //OnNegativeShowAd();
        //gameMechScript.triggerOnNegativeShowAd = true;
    }

    private void RBV_ad_OnAdClosed(object sender, System.EventArgs e)
    {
        //Reset Game
        //OnNegativeShowAd();
        //gameMechScript.triggerOnNegativeShowAd = true;
    }

    void LoadVideoAd()
    {
        if (!RBV_ad.IsLoaded())
        {
            RBV_ad.LoadAd(new AdRequest.Builder().Build(), adunitId);
            UnityEngine.Debug.Log("Loading Ad...");
        }
    }

    void ShowVideoAd()
    {
        if (RBV_ad.IsLoaded())
        {
            RBV_ad.Show();
        }
    }

}
