using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BladeCoreSwitcher : MonoBehaviour
{
 
    public GameObject coreSelect;
    public GameObject swordSelect;
    public Shop shop;
    public DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap scrollSnapSwordCore;
    public DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap scrollSnapSword;
    bool switchFlag = false;
    public bool setEquippedIndex = false;
    bool initialSetupFlag = false;
    [HideInInspector]
    public List<int> purchasesToMakeIndex = new List<int>();
    [HideInInspector]
    public int heroSyncEquipIndex = -1;

    public Image swordIcon;
    public Image coreIcon;

     private void Start()
    {
        restorePurchaseEquipIndex();
    }
    public void switchSelects()
    {
        if (!initialSetupFlag)
        {
            swordSelect.transform.GetChild(0).GetChild(0).transform.gameObject.SetActive(true);
            shop.updateScrollSnap(scrollSnapSwordCore);
            UpdatePreviousPurchases();
            int equippedIndex = shop.updateScrollSnap(scrollSnapSwordCore);
            try
            {
                shop.setItemValues(2);
            }
            catch
            {
                Debug.Log("Broke");
            }
            coreSelect.SetActive(false);
            switchFlag = true;
            swordIcon.enabled = true;
            coreIcon.enabled = false;
            initialSetupFlag = true;
            if (setEquippedIndex && equippedIndex > -1)
            {
                scrollSnapSwordCore.GoToPanel(equippedIndex);
                setEquippedIndex = false;
            }
            else if(equippedIndex < 0)
            {
                Debug.LogError("No item equipped");
            }
           
        }
        else
        {
            if (switchFlag)
            {
                shop.SetColors();
                shop.updateScrollSnap(scrollSnapSword);
                try
                {
                    shop.setItemValues();
                }
                catch
                {
                    Debug.Log("Broke");
                }
                coreSelect.SetActive(true);
                swordSelect.SetActive(false);
                switchFlag = false;
                swordIcon.enabled = false;
                coreIcon.enabled = true;

            }
            else
            {
                shop.SetColors();
                shop.updateScrollSnap(scrollSnapSwordCore);
                UpdatePreviousPurchases();
                int equippedIndex = shop.updateScrollSnap(scrollSnapSwordCore);
                try
                {
                    shop.setItemValues();
                }
                catch
                {
                    Debug.Log("Broke");
                }
                coreSelect.SetActive(false);
                swordSelect.SetActive(true);
                switchFlag = true;
                swordIcon.enabled = true;
                coreIcon.enabled = false;
                if (setEquippedIndex && equippedIndex > -1)
                {
                    scrollSnapSwordCore.GoToPanel(equippedIndex);
                    setEquippedIndex = false;
                }
                else if (equippedIndex < 0)
                {
                    Debug.LogError("No item equipped");
                }

            }
        }
        
    }
    public void resetScrollSnap()
    {
        if (switchFlag)
        {
            shop.updateScrollSnap(scrollSnapSword);
            shop.setItemValues();
            coreSelect.SetActive(true);
            swordSelect.SetActive(false);
            switchFlag = false;
            coreSelect.SetActive(true);
            swordSelect.SetActive(false);
            swordIcon.enabled = false;
            coreIcon.enabled = true;

        }
    }
    private void UpdatePreviousPurchases()
    {
        shop.purchaseItemsNoCost(ref purchasesToMakeIndex);
        if(heroSyncEquipIndex > -1)
        {
        
            shop.equipItem(heroSyncEquipIndex);
            heroSyncEquipIndex = -1;
        }
    }
    public void savePurchaseEquipIndex()
    {
        PlayerPrefs.SetString("purchasesToMakeIndex", listToString(purchasesToMakeIndex));
        PlayerPrefs.SetInt("equipIndex", heroSyncEquipIndex);
    }
    private void restorePurchaseEquipIndex()
    {
        purchasesToMakeIndex = stringToList(PlayerPrefs.GetString("purchasesToMakeIndex", ""));
        heroSyncEquipIndex = PlayerPrefs.GetInt("equipIndex", -1);
    }
    private string listToString(List<int> array)
    {
        string s = "";
        foreach (int i in array) 
        {
            s += i;
        }
        return s;

    }
    private List<int> stringToList(string s)
    {
        List<int> list = new List<int>();
        foreach(char c in s)
        {
            list.Add((int)Char.GetNumericValue(c));
        }
        return list;
    }
    public void switchToCoreIcon()
    {
       
    }
}
