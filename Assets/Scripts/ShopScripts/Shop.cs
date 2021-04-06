using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    private int money;
    private List<Item> itemList = new List<Item>();
    private DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap scrollSnap; 
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI moneyText2;
    public TextMeshProUGUI moneyText3;
    public TextMeshProUGUI moneyText4;
    public List<Transform> itemCanvas;
    public DissolveEnemyScript shopEnemyScript;
    public DamageManager damageManagerScript;
    public BladeCoreSwitcher bladeCoreSwitcher;
    public SwordSwapper swordSwapper;
    public DisplaySwapper dSwapper;
    public HeroDisplaySwapper hdSwapper;
    public StartMenuScript startMenu;
    public Light heroLight;
    public Light enemyLight;
    public Light swordLight;
    public bool deletePlayerPrefs = false;
    public ShopInterfaceNav nav;

    private ColorDataBase.enemyColorOptions equipppedEnemy;
    private ColorDataBase.heroColorOptions equippedHero;
    private ColorDataBase.heroColorOptions equippedSwordC;
    private int equippedSwordID;
  
    TextMeshProUGUI achievmentText;
    Button purchaseButton;
    TextMeshProUGUI itemName;
    private bool firstTimePlaying;

    public List<AchievementTracker> shopAchievements;

    private void Awake()
    {
        if (deletePlayerPrefs)
        {
            PlayerPrefs.DeleteAll();
        }
        Enum.TryParse(PlayerPrefs.GetString("EnemyColor", "red"), out ColorDataBase.enemyColorOptions enemyC);
        ColorDataBase.setEnemyColor(enemyC);
        Enum.TryParse(PlayerPrefs.GetString("HeroColor", "blue"), out ColorDataBase.heroColorOptions heroC);
        ColorDataBase.setHeroColor(heroC);
        Enum.TryParse(PlayerPrefs.GetString("SwordColor", "blue"), out ColorDataBase.heroColorOptions swordC);
        ColorDataBase.setSwordColor(swordC);
        equippedHero = heroC;
        equipppedEnemy = enemyC;
        equippedSwordC = swordC;
        equippedSwordID = PlayerPrefs.GetInt("equipped-sword", 0);


    }

    public void SetColors()
    {

        ColorDataBase.setEnemyColor(equipppedEnemy);
        ColorDataBase.setHeroColor(equippedHero);
        ColorDataBase.setSwordColor(equippedSwordC);

        //shopEnemyScript.SetMatColors();

        enemyLight.color = ColorDataBase.GetEnemyStripAlbedo();


        swordSwapper.GetActiveSword().GetComponent<SwordColorScript>().setSwordColor();
        damageManagerScript.SetHeroColor();
        startMenu.updateUI();
        heroLight.color = ColorDataBase.GetHeroAlbedo();

 

        swordSwapper.GetActiveSword().GetComponent<SwordColorScript>().setSwordColor();
        swordLight.color = ColorDataBase.GetSwordAlbedo();

        swordSwapper.SwapSwords(equippedSwordID);
        dSwapper.SetDisplaySword(equippedSwordID);
        hdSwapper.SetDisplaySword(equippedSwordID);
    }
    void Start()
    {   
        hasPlayed();
        setUpItemPlayerPrefs();
        money = PlayerPrefs.GetInt("money", 0);
        moneyText.text = "$"+money + "";
        moneyText2.text = "$" + money + "";
        moneyText3.text = "$" + money + "";
        moneyText4.text = "$" + money + "";
        heroLight.color = ColorDataBase.GetHeroAlbedo();
        enemyLight.color = ColorDataBase.GetEnemyStripAlbedo();
        swordLight.color = ColorDataBase.GetSwordAlbedo();
      
        
    }

    public void hasPlayed()
    {
        int hasPlayed = PlayerPrefs.GetInt("hasPlayed",0);
        if (hasPlayed == 0)
        {
            PlayerPrefs.SetInt("hasPlayed", 1);
            firstTimePlaying = true;
            
        }
        else
        {
            firstTimePlaying = false;
        }
        
    }
    public int selectedPanel()
    {
        return scrollSnap.CurrentPanel;
    }
    public void setUpItemPlayerPrefs()
    {
        bool noMoreEquipEnemyFlag = false;
        bool noMoreEquipSwordCoreFlag = false;
        bool noMoreEquipSword = false;

        foreach (Transform canvas in itemCanvas)
        {
            foreach(Transform t in canvas)
            {
                Item tempItem = t.GetComponent<Item>();
               
                tempItem.purchaseState = (Item.ButtonState)Enum.Parse(typeof(Item.ButtonState), PlayerPrefs.GetString(tempItem.name, tempItem.purchaseState.ToString()), true);
    
                if (tempItem.defaultItem && firstTimePlaying)
                {
                    tempItem.price = 0;
                    
                    tempItem.purchaseState = Item.ButtonState.equipped;
                    PlayerPrefs.SetString(tempItem.name, tempItem.purchaseState.ToString());

                }
            }
        }
    }
  
    
    public int updateScrollSnap(DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap _scrollSnap)
    {
        int equippedIndex = -1;
        itemList.Clear();
        scrollSnap = _scrollSnap;
        GameObject content = _scrollSnap.transform.GetChild(0).GetChild(0).gameObject;
       
        foreach (Transform child in content.transform)
        {
            Item i = child.GetComponent<Item>();
            itemList.Add(i);
            if(i.purchaseState == Item.ButtonState.equipped)
            {
                if(i.type == Item.Type.enemyColor && i.enemyColor.ToString() == ColorDataBase.enemyColorName)
                {
                    equippedIndex = itemList.Count - 1;
                }
                else if(i.type == Item.Type.enemyColor && i.enemyColor.ToString() != ColorDataBase.enemyColorName)
                {
                    i.purchaseState = Item.ButtonState.purchased;
                }

                else if (i.type == Item.Type.swordColor && i.heroColor.ToString() == ColorDataBase.swordColorName)
                {
                    equippedIndex = itemList.Count - 1;

                }
                else if (i.type == Item.Type.swordColor && i.heroColor.ToString() != ColorDataBase.swordColorName)
                {
                    i.purchaseState = Item.ButtonState.purchased;
                }
                else
                {
                    equippedIndex = itemList.Count - 1;
                }
            }
            else if(i.type == Item.Type.swordColor && i.heroColor.ToString() == ColorDataBase.swordColorName)
            {
                equippedIndex = itemList.Count - 1;
             
            }
            else if (i.type == Item.Type.enemyColor && i.enemyColor.ToString() == ColorDataBase.enemyColorName)
            {
                equippedIndex = itemList.Count - 1;
         
            }
        }
        return equippedIndex;
    }
    public void updateShopUIReferences(TextMeshProUGUI _achievementText, Button _purchaseButton, TextMeshProUGUI _itemName)
    {
        itemName = _itemName;
        purchaseButton = _purchaseButton;
        achievmentText = _achievementText;
    }
    public void setItemValues()
    {
        try
        {
            int index = scrollSnap.CurrentPanel;
            achievmentText.text = itemList[index].achievement;
            itemName.text = itemList[index].displayName;

            Text buttonText = purchaseButton.GetComponentInChildren<Text>();
            if (itemList[index].purchaseState == Item.ButtonState.equipped)
            {
                buttonText.text = "Equipped";
            }
            else if (itemList[index].purchaseState == Item.ButtonState.locked)
            {
                buttonText.text = "Locked";
            }
            else if (itemList[index].purchaseState == Item.ButtonState.unlocked)
            {
                buttonText.text = "$" + itemList[index].price;
            }
            else if (itemList[index].purchaseState == Item.ButtonState.purchased)
            {
                buttonText.text = "Equip";
            }
            else if (itemList[index].purchaseState == Item.ButtonState.noPurchase)
            {
                buttonText.text = "Locked";
            }

            Invoke("PreviewItems", 0.25f);
            
        }

        catch
        {

        }
    }
    public void setItemValues(int _index)
    {
        int index = _index;
        
        achievmentText.text = itemList[index].achievement;
        itemName.text = itemList[index].displayName;

        Text buttonText = purchaseButton.GetComponentInChildren<Text>();
        if (itemList[index].purchaseState == Item.ButtonState.equipped)
        {
            buttonText.text = "Equipped";
        }
        else if (itemList[index].purchaseState == Item.ButtonState.locked)
        {
            buttonText.text = "Locked";
        }
        else if (itemList[index].purchaseState == Item.ButtonState.unlocked)
        {
            buttonText.text = "$" + itemList[index].price;
        }
        else if (itemList[index].purchaseState == Item.ButtonState.purchased)
        {
            buttonText.text = "Equip";
        }
        else if (itemList[index].purchaseState == Item.ButtonState.noPurchase)
        {
            buttonText.text = "-";
        }
    }
    public void pressPurchaseEnableButton(Button button)
    {
        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText.text == "Equip")
        {
            equipItem();
        }
        else if (buttonText.text.StartsWith("$"))
        {
            purchaseItem();
        }
    }
    public void purchaseItem()
    {
        
        Item i = itemList[scrollSnap.CurrentPanel];
        if (withdrawlMoney(i.price))
        {
            i.purchaseState = Item.ButtonState.purchased;
            PlayerPrefs.SetString(i.name, i.purchaseState.ToString());
            setItemValues();
            if(i.type == Item.Type.heroColor)
            {
                Analytics.CustomEvent("Hero Color Purchased: ", new Dictionary<string, object>
                {
                    {"Hero Color: ", i.heroColor},
                });
                

                bladeCoreSwitcher.purchasesToMakeIndex.Add(scrollSnap.CurrentPanel);
                bladeCoreSwitcher.savePurchaseEquipIndex();

                if (PlayerPrefs.GetString("shopfirstherocolor", "locked") == "locked")
                {
                    AchievementEvents.aEvents.UnlockAchievementTrigger("shopfirstherocolor");
                    shopAchievements[0].InitVariables();
                }
               
            }

            else if (i.type == Item.Type.enemyColor)
            {
                Analytics.CustomEvent("Enemy Color Purchased: ", new Dictionary<string, object>
                {
                    {"Enemy Color: ", i.enemyColor},
                });
                
                if (PlayerPrefs.GetString("shopfirstenemycolor", "locked") == "locked")
                {
                    AchievementEvents.aEvents.UnlockAchievementTrigger("shopfirstenemycolor");
                    shopAchievements[1].InitVariables();
                }
            }

            else if (i.type == Item.Type.swordModel)
            {
                Analytics.CustomEvent("Sword Model Purchased: ", new Dictionary<string, object>
                {
                    {"Sword Model: ", i.swordID},
                });
                if (PlayerPrefs.GetString("shopfirstsword", "locked") == "locked")
                {
                    AchievementEvents.aEvents.UnlockAchievementTrigger("shopfirstsword");
                    shopAchievements[2].InitVariables();
                }
            }

            Analytics.CustomEvent("Shop Item Purchased: ", new Dictionary<string, object>
            {
                {"Item Type: ", i.type},
            });
            

        }
    }
    public void purchaseItemsNoCost(ref List<int> indexes)
    {
        for(int j = 0;j < indexes.Count; j++)
        {
            Item i = itemList[indexes[j]];
            i.purchaseState = Item.ButtonState.purchased;
            Debug.Log(i.name);
            PlayerPrefs.SetString(i.name, i.purchaseState.ToString());
            setItemValues();
        }
        indexes.Clear();
    }

    public void equipItem()
    {
        Item i = itemList[scrollSnap.CurrentPanel];
        if (i.purchaseState == Item.ButtonState.purchased)
        {
            foreach (var otherItem in itemList)
            {
                if(otherItem.purchaseState == Item.ButtonState.equipped)
                {
                    otherItem.purchaseState = Item.ButtonState.purchased;
                    PlayerPrefs.SetString(otherItem.name, otherItem.purchaseState.ToString());
                }
            }
            i.purchaseState = Item.ButtonState.equipped;
            PlayerPrefs.SetString(i.name, i.purchaseState.ToString());
          
            
            setItemValues();
            if(i.type == Item.Type.enemyColor)
            {
                equipppedEnemy = i.enemyColor;

                AudioManager.instance.Play("PowerCoreStartup", UnityEngine.Random.Range(1f, 1.25f));
            }
            else if (i.type == Item.Type.heroColor)
            {
                
                bladeCoreSwitcher.savePurchaseEquipIndex();
                AudioManager.instance.Play("PowerCoreStartup", UnityEngine.Random.Range(1f, 1.25f));
                startMenu.updateUI();
                equippedHero = i.heroColor;


            }
            else if (i.type == Item.Type.swordColor)
            {
                AudioManager.instance.Play("PowerCoreStartup", UnityEngine.Random.Range(1f, 1.25f));
                equippedSwordC = i.heroColor;
            }
            else if (i.type == Item.Type.swordModel)
            {
                AudioManager.instance.Play("PowerCoreStartup", UnityEngine.Random.Range(1f, 1.25f));
                equippedSwordID = i.swordID;
            }
        }
        
    }

    public void PreviewItems()
    {
        Item i = itemList[scrollSnap.CurrentPanel];
        if (i.type == Item.Type.enemyColor)
        {
            ColorDataBase.setEnemyColor(i.enemyColor);

            shopEnemyScript.SetMatColors();

            enemyLight.color = ColorDataBase.GetEnemyStripAlbedo();

        }
        else if (i.type == Item.Type.heroColor)
        {
            ColorDataBase.setHeroColor(i.heroColor);
            swordSwapper.GetActiveSword().GetComponent<SwordColorScript>().setSwordColor();
            damageManagerScript.SetHeroColor();
            
            heroLight.color = ColorDataBase.GetHeroAlbedo();
            // bladeCoreSwitcher.heroSyncEquipIndex = scrollSnap.CurrentPanel;


        }
        else if (i.type == Item.Type.swordColor)
        {
            
            ColorDataBase.setSwordColor(i.heroColor);

            swordSwapper.GetActiveSword().GetComponent<SwordColorScript>().setSwordColor();
            swordLight.color = ColorDataBase.GetSwordAlbedo();
        }
        else if (i.type == Item.Type.swordModel)
        {
            swordSwapper.SwapSwords(i.swordID);
            dSwapper.SetDisplaySword(i.swordID);
            hdSwapper.SetDisplaySword(i.swordID);
        }

        if (nav.exitingShop)
        {
            this.SetColors();
        }
    }
    //Only used by sword cores
    public void equipItem(int  j)
    {
        Item i = itemList[j];
      
            foreach (var otherItem in itemList)
            {
                if (otherItem.purchaseState == Item.ButtonState.equipped)
                {
                    otherItem.purchaseState = Item.ButtonState.purchased;
                }
            }
            i.purchaseState = Item.ButtonState.equipped;
            PlayerPrefs.SetString(i.name, i.purchaseState.ToString());
            PlayerPrefs.SetString("SwordColor", i.heroColor.ToString());
            setItemValues();
       
    }
    public void unlockItem(Item i)
    {
        i.purchaseState = Item.ButtonState.unlocked;
    }
    public void depositMoney(int deposit)
    {
        money += deposit;
        PlayerPrefs.SetInt("money", money);
        moneyText.text = "$" + money +"";
        moneyText2.text = "$" + money + "";
        moneyText3.text = "$" + money + "";
        moneyText4.text = "$" + money + "";

    }
    public bool withdrawlMoney(int withdrawl)
    {

        if (money - withdrawl >= 0)
        {
            money -= withdrawl;
            PlayerPrefs.SetInt("money", money);
            moneyText.text = "$" + money + "";
            moneyText2.text = "$" + money + "";
            moneyText3.text = "$" + money + "";
            moneyText4.text = "$" + money + "";
            return true;
        }
        else
        {
            return false;
        }
       
       
    }
    private void OnApplicationQuit()
    {
        SetColors();
        PlayerPrefs.SetString("HeroColor", ColorDataBase.heroColorName);
        PlayerPrefs.SetString("EnemyColor", ColorDataBase.enemyColorName);
        PlayerPrefs.SetString("SwordColor", ColorDataBase.swordColorName);

/*
        foreach (Transform canvas in itemCanvas)
        {
            foreach (Transform t in canvas)
            {
                Item tempItem = t.GetComponent<Item>();

               PlayerPrefs.SetString(tempItem.name,tempItem.p)
             
            }
        }*/
    }
   

}
