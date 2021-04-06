using System.Collections;
using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Structure
{ 

    public GameObject prefab;
    
    //Number of stories, e.g. 1, 2, 3
    public int height;

    public float probabilityMultiplier;

    //X does nothing. Y modifies height from ground, z modifies distance from curb
    public Vector3 spawnPosition;

    [HideInInspector]
    public float extentX;
}
