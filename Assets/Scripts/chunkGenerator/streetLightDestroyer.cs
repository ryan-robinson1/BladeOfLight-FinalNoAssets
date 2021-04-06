using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class streetLightDestroyer : MonoBehaviour
{
    GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.x > this.transform.position.x + 35)
        {
            Destroy(this.gameObject);
        }
    }
}
