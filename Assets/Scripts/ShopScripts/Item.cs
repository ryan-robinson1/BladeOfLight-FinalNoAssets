using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item : MonoBehaviour
{
    public GameObject prefab;
    //Unique Identifying name
    public string name;
    //Displayed Name
    public string displayName;
    public int price;
    public bool defaultItem;
    public ColorDataBase.enemyColorOptions enemyColor;
    public ColorDataBase.heroColorOptions heroColor;
    public int swordID;
    //Change to class later
    public string achievement;
    public ButtonState purchaseState;
    public Type type;

    public enum ButtonState
    {
        purchased,
        equipped,
        unlocked,
        locked,
        noPurchase
    }
    public enum Type
    {
        heroColor,
        enemyColor,
        swordColor,
        swordModel
    }
 
}

