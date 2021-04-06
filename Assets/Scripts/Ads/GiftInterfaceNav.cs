
using UnityEngine;
using UnityEngine.UI;
public class GiftInterfaceNav : MonoBehaviour
{
    public GameObject giftMenu;
    public float timeBetweenNewGifts = 100f;

    public void openGitftMenu()
    {
        giftMenu.SetActive(true);
        
    }
    public void closeGiftMenu()
    {
        giftMenu.SetActive(false);
    }
    public void disableGiftForTime()
    {
        this.GetComponent<Button>().enabled = false;
        this.GetComponent<Image>().enabled = false;
        Invoke("enableGiftButton", timeBetweenNewGifts);
    }
    public void enableGiftButton()
    {
        this.GetComponent<Button>().enabled = true;
        this.GetComponent<Image>().enabled = true;
    }
}
