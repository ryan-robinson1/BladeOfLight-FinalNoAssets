
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopInterfaceNav : MonoBehaviour
{
    public Camera ShopCamera;
    public Camera MainMenuCamera;
    public Canvas PlayerShopCanvas;
    public Canvas EnemyShopCanvas;
    public Canvas SwordShopCanvas;
    public Canvas mainMenuCanvas;
    public Shop shop;
    public DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap scrollSnapSword;
    public DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap scrollSnapSwordCore;
    public BladeCoreSwitcher bladeCoreSwitcher;
    bool moveToEnemyShop = false;
    bool moveToPlayerShop = false;
    bool moveToSwordShop = false;
    public TextMeshProUGUI EnemyShopAchievementRequirement;
    public TextMeshProUGUI SwordShopAchievementRequirement;
    public TextMeshProUGUI HeroShopAchievementRequirement;

    public TextMeshProUGUI EnemyShopItemName;
    public TextMeshProUGUI SwordShopItemName;
    public TextMeshProUGUI HeroShopItemName;

    public Button EnemyShopPurchaseButton;
    public Button SwordShopPurchaseButton;
    public Button HeroShopPurchaseButton;

    public bool exitingShop = false;

    Vector3 enemyShopCameraPosition = new Vector3(-8.152f, 2.43f, 10.6f);
    Vector3 playerShopCameraPosition = new Vector3(-15f, 2.43f, 10.6f);
    Vector3 swordShopCameraPosition = new Vector3(-21.848f, 2.43f, 10.6f);

    private bool inEnemy = false;
    private bool inSword = false;
    private bool inPlayer = false;

    public Animator[] playerUI;
    public Animator[] enemyUI;
    public Animator[] swordUI;

   
    private DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap scrollSnap;
    public void goToShop()
    {
        int equippedIndex;
        ShopCamera.enabled = true;
        MainMenuCamera.enabled = false;
        EnterPlayerAnim();
        mainMenuCanvas.enabled = false;
        shop.transform.GetChild(0).gameObject.SetActive(true);
        scrollSnap = PlayerShopCanvas.transform.GetComponentInChildren<DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap>();
        exitingShop = false;
        

        equippedIndex = shop.updateScrollSnap(scrollSnap);
        shop.updateShopUIReferences(HeroShopAchievementRequirement, HeroShopPurchaseButton,
                 HeroShopItemName);
        shop.setItemValues();
        if (equippedIndex > -1)
        {
            scrollSnap.GoToPanel(equippedIndex);
        }


    }
    public void goToMainMenu()
    {
        DisablePlayerCanvas();
        shop.SetColors();
        ShopCamera.enabled = false;
        ShopCamera.transform.position = playerShopCameraPosition;
        MainMenuCamera.enabled = true;
        exitingShop = true;
        if (inPlayer)
        {
            ExitPlayerAnim();
        }
        else if (inEnemy)
        {
            ExitEnemyAnim();
        }
        else
        {
            ExitSwordAnim();
        }

        
        mainMenuCanvas.enabled = true;
        inEnemy = false;
        inSword = false;
        shop.transform.GetChild(0).gameObject.SetActive(false);
    }
    public void goToEnemyShop()
    {
        shop.SetColors();
        int equippedIndex;
        moveToEnemyShop = true;
        ExitPlayerAnim();
        Invoke("EnterEnemyAnim", 0.5f);
        scrollSnap = EnemyShopCanvas.transform.GetComponentInChildren<DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap>();
        equippedIndex = shop.updateScrollSnap(scrollSnap);
        shop.updateShopUIReferences(EnemyShopAchievementRequirement, EnemyShopPurchaseButton,
                 EnemyShopItemName);
        shop.setItemValues();

        if (equippedIndex > -1)
        {
            scrollSnap.GoToPanel(equippedIndex);
        }

    }
    public void goToPlayerShop()
    {
        shop.SetColors();
        int equippedIndex;
        if (inEnemy)
        {
            ExitEnemyAnim();
        }
        else if (inSword)
        {
            ExitSwordAnim();
        }
        

        Invoke("EnterPlayerAnim", 0.5f);
        moveToPlayerShop = true;
        scrollSnap = PlayerShopCanvas.transform.GetComponentInChildren<DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap>();
        equippedIndex = shop.updateScrollSnap(scrollSnap);
        shop.updateShopUIReferences(HeroShopAchievementRequirement, HeroShopPurchaseButton,
                 HeroShopItemName);
        shop.setItemValues();
        if (equippedIndex > -1)
        {
            scrollSnap.GoToPanel(equippedIndex);
            
        }



    }
    public void goToSwordShop()
    {
        
        shop.SetColors();
        
        int equippedIndex; 
        moveToSwordShop = true;
        ExitPlayerAnim();
        Invoke("EnterSwordAnim", 0.5f);
        SwordShopCanvas.enabled = true;
        bladeCoreSwitcher.resetScrollSnap();
        bladeCoreSwitcher.setEquippedIndex = true;
         
        equippedIndex = shop.updateScrollSnap(scrollSnapSword);
        shop.setItemValues();
        if (equippedIndex > -1)
        {
            scrollSnapSword.GoToPanel(equippedIndex);
        }


        shop.updateShopUIReferences(SwordShopAchievementRequirement, SwordShopPurchaseButton,
                 SwordShopItemName);

    }

    void Update()
    {
        if (moveToEnemyShop)
        {
            ShopCamera.transform.position = Vector3.MoveTowards(ShopCamera.transform.position, enemyShopCameraPosition, Time.deltaTime*10);
            if(ShopCamera.transform.position == enemyShopCameraPosition)
            {
                moveToEnemyShop = false;
                
                shop.setItemValues();

            }
        }
        else if (moveToPlayerShop)
        {
            ShopCamera.transform.position = Vector3.MoveTowards(ShopCamera.transform.position, playerShopCameraPosition, Time.deltaTime * 10);
            if (ShopCamera.transform.position == playerShopCameraPosition)
            {
                moveToPlayerShop = false;

                shop.setItemValues();
            }
        }
        else if (moveToSwordShop)
        {
            ShopCamera.transform.position = Vector3.MoveTowards(ShopCamera.transform.position, swordShopCameraPosition, Time.deltaTime * 10);
            if (ShopCamera.transform.position == swordShopCameraPosition)
            {
                moveToSwordShop = false;

                shop.setItemValues();
            }
        }
    }

    private void EnterPlayerAnim()
    {
        PlayerShopCanvas.enabled = true;
        inPlayer = true;
        foreach (Animator anim in playerUI)
        {
            anim.SetTrigger("enter");
        }
    }

    private void ExitPlayerAnim()
    {
        inPlayer = false;
        foreach (Animator anim in playerUI)
        {
            anim.SetTrigger("exit");
        }
        if (PlayerShopCanvas.enabled)
        {
            Invoke("DisablePlayerCanvas", 0.5f);
        }
        
    }

    private void DisablePlayerCanvas()
    {
        PlayerShopCanvas.enabled = false;
    }

    private void EnterEnemyAnim()
    {
        EnemyShopCanvas.enabled = true;
        inEnemy = true;
        foreach (Animator anim in enemyUI)
        {
            anim.SetTrigger("enter");
        }
    }

    private void ExitEnemyAnim()
    {
        inEnemy = false;
        foreach (Animator anim in enemyUI)
        {
            anim.SetTrigger("exit");
        }
        Invoke("DisableEnemyCanvas", 0.75f);
    }

    private void DisableEnemyCanvas()
    {
        EnemyShopCanvas.enabled = false;
    }

    private void EnterSwordAnim()
    {
        SwordShopCanvas.enabled = true;
        inSword = true;
        // bc player and sword share same ui elements

        foreach (Animator anim in swordUI)
        {
            anim.SetTrigger("enter");
        }
    }

    private void ExitSwordAnim()
    {
        inSword = false;

        foreach (Animator anim in swordUI)
        {
            anim.SetTrigger("exit");
        }
        Invoke("DisableSwordCanvas", 0.75f);
    }

    private void DisableSwordCanvas()
    {
        SwordShopCanvas.enabled = false;
    }

}
