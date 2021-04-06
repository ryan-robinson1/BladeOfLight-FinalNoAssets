using System;
using UnityEngine.Analytics;
using UnityEngine;

public class GiftAdManager : MonoBehaviour
{

    int retryAttemptRewarded;
    //iOS
    private const string RewardedAdUnitId = "fd63e009fb1ce01e";
    //Android
    //private const string RewardedAdUnitId = "53abea9155368a50";
    public Shop shop;
    public GiftInterfaceNav giftInterface;


    private void Start()
    {
        if (!IsFirstStarting.instance.startUp)
            InitializeRewardedAds();
    }

    public void PlayGiftRewardedAd()
    {
        if (MaxSdk.IsRewardedAdReady(RewardedAdUnitId))
        {
            MaxSdk.ShowRewardedAd(RewardedAdUnitId);
            Analytics.CustomEvent("Gift Ad Watched");
            if (PlayerPrefs.GetString("muteMusic", "false") == "false")
            {
                AudioManager.instance.MuteSoundsUnsaved();
            }
        }
        
    }
    #region Rewarded Ad Methods


    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.OnRewardedAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.OnRewardedAdLoadFailedEvent += OnRewardedAdFailedEvent;
        MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.OnRewardedAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.OnRewardedAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.OnRewardedAdHiddenEvent += OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }
    private void OnDisable()
    {
        MaxSdkCallbacks.OnRewardedAdLoadedEvent -= OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.OnRewardedAdLoadFailedEvent -= OnRewardedAdFailedEvent;
        MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent -= OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.OnRewardedAdDisplayedEvent -= OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.OnRewardedAdClickedEvent -= OnRewardedAdClickedEvent;
        MaxSdkCallbacks.OnRewardedAdHiddenEvent -= OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent -= OnRewardedAdReceivedRewardEvent;
    }


    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(RewardedAdUnitId);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId)
    {
        // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(adUnitId) will now return 'true'

        // Reset retry attempt
        retryAttemptRewarded = 0;
    }

    private void OnRewardedAdFailedEvent(string adUnitId, int errorCode)
    {
        // Rewarded ad failed to load 
        // We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds)

        retryAttemptRewarded++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttemptRewarded));

        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, int errorCode)
    {
        // Rewarded ad failed to display. We recommend loading the next ad
        LoadRewardedAd();
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId) { }

    private void OnRewardedAdClickedEvent(string adUnitId) { }

    private void OnRewardedAdDismissedEvent(string adUnitId)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward)
    {

        if (adUnitId == RewardedAdUnitId)
        {
            try
            {
                shop.depositMoney(30);
                giftInterface.closeGiftMenu();
                giftInterface.disableGiftForTime();
                if (PlayerPrefs.GetString("muteMusic", "false") == "false")
                {
                    AudioManager.instance.UnMuteSoundsUnsaved();
                }
            }
            catch
            {
                Debug.LogWarning("Currency Not Added");
            }
        }
    }



    #endregion
}
