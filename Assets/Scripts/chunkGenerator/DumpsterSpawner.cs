using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumpsterSpawner : MonoBehaviour
{
    public GameObject[] dumpsters;

    private void Start()
    {
        if (Random.Range(0, 2) == 0)
        {
            dumpsters[Random.Range(0, dumpsters.Length)].GetComponentInChildren<MeshRenderer>().enabled = true;

        }
    }
}
