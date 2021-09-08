using GoogleMobileAds.Api;
using UnityEngine;
using System;
using System.Collections;

public class AdMob : MonoBehaviour
{
    public static AdMob active;
    public enum AdsTypes {Banner, RewardedLife, RewardedPresent, Video} 

    public string AppId = "ca-app-pub-8698419787299114~9025445104";
    public string VideoId = "ca-app-pub-8698419787299114/4405850266";
    public string RewardedId = "ca-app-pub-8698419787299114/7389793178";
    public string BannerId = "ca-app-pub-8698419787299114/2290822811";

    public static bool GotReward;
    public static BannerView banner;
    public static InterstitialAd Video;
    public static RewardedAd Rewarded;
    public delegate void OnRewardedEnd();
    public event OnRewardedEnd Watched;
    public event OnRewardedEnd Skiped;

    private void Awake()
    {
        active = this;
    }
    public void Init()
    {
        MobileAds.Initialize(initStatus => { });
    }

    public void SetCallback(AdsTypes type)
    {
        Watched = null;
        Skiped = null;
        switch (type)
        {
            case AdsTypes.RewardedLife:
                Watched += Level.active.SaveLife;
                Skiped += Level.active.SkipSaveLife;
                break;
            case AdsTypes.RewardedPresent:
                Watched += Level.active.OnPresentTaken;
                break;
        }
    }

    public void ShowVideo()
    {
        StartCoroutine(ShowVideoForce());
    }
    private IEnumerator ShowVideoForce()
    {
        if(Video == null || !Video.IsLoaded())
        {
            LoadVideo();
        }
        while (!Video.IsLoaded())
        {
            yield return new WaitForFixedUpdate();
        }
        Video.Show();
        yield break;
    }

    public void ShowRewarded()
    {
        StartCoroutine(ShowRewardedForce());
    }
    private IEnumerator ShowRewardedForce()
    {
        if(Rewarded == null || !Rewarded.IsLoaded())
        {
            LoadRewarded();
        }
        while (!Rewarded.IsLoaded())
        {
            yield return new WaitForFixedUpdate();
        }
        Rewarded.Show();
        yield break;
    }

    public void LoadAds()
    {
        LoadVideo();
        LoadRewarded();
    }
    public void LoadVideo()
    {
        AdRequest request = new AdRequest.Builder().Build();

        Video = new InterstitialAd(VideoId);
        Video.OnAdOpening += Pause;
        Video.OnAdClosed += Resume;

        Video.OnAdClosed += PreLoadInterstitial;
        Video.LoadAd(request);
    }
    public void LoadRewarded()
    {
        AdRequest request = new AdRequest.Builder().Build();

        Rewarded = new RewardedAd(RewardedId);
        Rewarded.OnAdOpening += Pause;
        Rewarded.OnUserEarnedReward += Reward;
        Rewarded.OnAdClosed += ChecForReward;
        Rewarded.OnAdClosed += PreLoadRewarded;
        Rewarded.LoadAd(request);
    }


    public void Pause(object sender, EventArgs args)
    {

    }
    public void Resume(object sender, EventArgs args)
    {

    }
    public void ChecForReward(object sender, EventArgs args)
    {
        GotReward = true;
    }
    public void Reward(object sender, EventArgs args)
    {
        if(GotReward)
        {
            Watched?.Invoke();
        }
        else
        {
            Skiped?.Invoke();
        }
    }

    public void PreLoadInterstitial(object sender, EventArgs args)
    {
        LoadVideo();
    }
    public void PreLoadRewarded(object sender, EventArgs args)
    {
        LoadRewarded();
    }

    public void CreateBanner()
    {
        AdSize Size = AdSize.Banner;
        AdPosition Position = AdPosition.Bottom;
        banner = new BannerView(BannerId, Size, Position);
        AdRequest request = new AdRequest.Builder().Build();
        banner.LoadAd(request);
        banner.OnAdLoaded += ShowBanner;
    }
    private void ShowBanner(object sender, EventArgs args)
    {
        banner.Show();
    }
    public void ShowBanner()
    {
        if (banner != null)
        {
            banner.Show();
        }
        else
        {
            CreateBanner();
        }
    }
    public void HideBanner()
    {
        if (banner != null)
        {
            banner.Hide();
        }
    }
    public void DestroyBanner()
    {
        if (banner != null)
        {
            banner.Destroy();
            banner = null;
        }
    }
}

