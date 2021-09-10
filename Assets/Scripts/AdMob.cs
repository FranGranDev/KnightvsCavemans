using GoogleMobileAds.Api;
using UnityEngine;
using System;
using System.Collections;

public class AdMob : MonoBehaviour
{
    public static AdMob active;
    public enum AdsTypes {Banner, RewardedLife, RewardedPresent, Video} 

    const string VideoId = "ca-app-pub-3940256099942544/1033173712";
    const string RewardedId = "ca-app-pub-3940256099942544/5224354917";
    const string BannerId = "ca-app-pub-3940256099942544/6300978111";

    public static bool ShowingAds;
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
        LoadAds();
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
        ShowingAds = true;
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
        ShowingAds = true;
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
        Rewarded.OnAdClosed += Resume;
        Rewarded.OnAdClosed += ChecForReward;
        Rewarded.OnAdClosed += PreLoadRewarded;
        Rewarded.LoadAd(request);
    }


    public void Pause(object sender, EventArgs args)
    {
        ShowingAds = true;
    }
    public void Resume(object sender, EventArgs args)
    {
        ShowingAds = false;
        Level.active.SpecialResume();
    }

    public void ChecForReward(object sender, EventArgs args)
    {
        if (GotReward)
        {
            Watched?.Invoke();
        }
        else
        {
            Skiped?.Invoke();
        }
    }
    public void Reward(object sender, EventArgs args)
    {
        GotReward = true;

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
        if (banner != null)
            return;

        AdSize Size = AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        banner = new BannerView(BannerId, Size, AdPosition.Bottom);
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
            Debug.Log("Show");
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
            Debug.Log("Hide");
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

