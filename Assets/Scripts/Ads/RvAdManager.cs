using System;
using UnityEngine;

public class ReviveAdManager : MonoBehaviour
{

    int retryAttemptRewarded;
    //iOS
    private const string ReviveRewardedAdUnitId = "34fead58c10e695d";
    //Android
    //private const string ReviveRewardedAdUnitId = "e17e3eb22c74eaad";
    public GameOverState _gameOverState;
    public Shop shop;
    private void Start()
    {
        if (!IsFirstStarting.instance.startUp)
            InitializeRewardedAds();
    }
    public void PlayReviveRewardedAd()
    {
        if (MaxSdk.IsRewardedAdReady(ReviveRewardedAdUnitId))
        {
            AudioManager.instance.MuteSoundsUnsaved();
            MaxSdk.ShowRewardedAd(ReviveRewardedAdUnitId);
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
        MaxSdk.LoadRewardedAd(ReviveRewardedAdUnitId);
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
        if(adUnitId == ReviveRewardedAdUnitId)
        {
            try
            {
                _gameOverState.enabled = true;
                _gameOverState.revivePlayer();
                AudioManager.instance.UnMuteSoundsUnsaved();
            }
            catch
            {
                Debug.LogWarning("GameOverState might be null");
            }
        }
      

       
    }



    #endregion
}
