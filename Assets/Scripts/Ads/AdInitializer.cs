using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdInitializer : MonoBehaviour
{
    private const string MaxSdkKey = "31yfPvKGT3LgIuCPEKHzWRmSV5xPWbGnB8PWGTbfRnOXHktU2QIar3kGgxMrR-xS84tMM_aiVDF2MHaiwMSV__";

    public static AdInitializer instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    
    
            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
            {
                // AppLovin SDK is initialized, configure and start loading ads.
                Debug.Log("MAX SDK Initialized");

              

                /*;W
                InitializeRewardedInterstitialAds();
                InitializeBannerAds();
                InitializeMRecAds();*/
                // MaxSdk.ShowMediationDebugger();
            };


            Debug.Log("SetSDKKey");
            MaxSdk.SetSdkKey(MaxSdkKey);
            MaxSdk.InitializeSdk();

       

    }
}
