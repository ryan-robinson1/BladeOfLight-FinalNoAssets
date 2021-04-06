using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InterAdManager : MonoBehaviour
{

    public RvAdManager rvAds;
    public GiftAdManager giftAds;
    //iOS
    private const string InterstitialAdUnitId = "52ebc2d24e124327";
    //Android
    //private const string InterstitialAdUnitId = "264b4fc9e9791988";
    int retryAttempt;
    bool init = false;
 
   
    private void Start()
    {
        InitializeInterstitialAds();
    }

    //Plays an interstitial ad
    public void PlayInterstitialAd()
    {
        if (MaxSdk.IsInterstitialReady(InterstitialAdUnitId))
        {
            MaxSdk.ShowInterstitial(InterstitialAdUnitId);
            if (PlayerPrefs.GetString("muteMusic", "false") == "false")
            {
                AudioManager.instance.MuteSoundsUnsaved();
            }
        }
      //  LoadInterstitial();
    }


    #region Interstitial Ad Methods
    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.OnInterstitialLoadFailedEvent += OnInterstitialFailedEvent;
        MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent += InterstitialFailedToDisplayEvent;
        MaxSdkCallbacks.OnInterstitialHiddenEvent += OnInterstitialDismissedEvent;

        // Load the first interstitial
        LoadInterstitial();
    }
    private void OnDisable()
    {
        MaxSdkCallbacks.OnInterstitialLoadedEvent -= OnInterstitialLoadedEvent;
        MaxSdkCallbacks.OnInterstitialLoadFailedEvent -= OnInterstitialFailedEvent;
        MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent -= InterstitialFailedToDisplayEvent;
        MaxSdkCallbacks.OnInterstitialHiddenEvent -= OnInterstitialDismissedEvent;
    }
    private void Disable()
    {
        MaxSdkCallbacks.OnInterstitialLoadedEvent -= OnInterstitialLoadedEvent;
        MaxSdkCallbacks.OnInterstitialLoadFailedEvent -= OnInterstitialFailedEvent;
        MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent -= InterstitialFailedToDisplayEvent;
        MaxSdkCallbacks.OnInterstitialHiddenEvent -= OnInterstitialDismissedEvent;
    }
    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(InterstitialAdUnitId);
        Debug.Log("Loading Inter");
    }

    private void OnInterstitialLoadedEvent(string adUnitId)
    {
        // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(adUnitId) will now return 'true'

        // Reset retry attempt
        retryAttempt = 0;
    }

    private void OnInterstitialFailedEvent(string adUnitId, int errorCode)
    {
        // Interstitial ad failed to load 
        // We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds)

        retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

        Invoke("LoadInterstitial", (float)retryDelay);
    }
    private void InterstitialFailedToDisplayEvent(string adUnitId, int errorCode)
    {
        // Interstitial ad failed to display. We recommend loading the next ad
        LoadInterstitial();
    }

    private void OnInterstitialDismissedEvent(string adUnitId)
    {
        // Interstitial ad is hidden. Pre-load the next ad
        rvAds.enabled = false;
        giftAds.enabled = false;
        Disable();
        if (PlayerPrefs.GetString("muteMusic", "false") == "false")
        {
            AudioManager.instance.UnMuteSoundsUnsaved();
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
       
    }
    #endregion

    
}
