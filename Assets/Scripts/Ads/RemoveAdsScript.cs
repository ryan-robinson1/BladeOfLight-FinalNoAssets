
using System.Collections;
using UnityEngine;
using UnityEngine.Purchasing;

public class RemoveAdsScript : MonoBehaviour
{
    public GameObject backGround;
    public GameObject noAds;
    public GameObject restorePurchases;
    public GameObject restoreBackground;
    private Vector3 noAdsPos;
    private Vector3 restorePos;

    private void Start()
    {
        noAdsPos = backGround.transform.position;
        restorePos = restoreBackground.transform.position;
        if (PlayerPrefs.GetString("noads", "false") == "true")
        {
            backGround.SetActive(false);
            noAds.SetActive(false);
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            restorePurchases.SetActive(false);
            restoreBackground.SetActive(false);
            backGround.SetActive(false);
            noAds.SetActive(false);
        }
    }
    /**
     * Called when the player purchases the remove ads button
     */
    public void OnPurchaseComplete(Product product)
    {
        PlayerPrefs.SetString("noads", "true");
#if UNITY_EDITOR
        StartCoroutine(DisableAdsButton());
#else
        noAds.SetActive(false);
#endif

    }

    /**
     * Called when the purchase fails for the remove ads button
     */
    public void OnPurchaseFailure(Product product, PurchaseFailureReason reason)
    {
        Debug.Log($"The {product} purchase failed because {reason}.");
    }

    /**
     * Called for the editor to allow processing to complete before disabling
     * the no ads button.
     */
    private IEnumerator DisableAdsButton()
    {
        yield return new WaitForEndOfFrame();
        noAds.SetActive(false);
        backGround.SetActive(false);
    }
}
