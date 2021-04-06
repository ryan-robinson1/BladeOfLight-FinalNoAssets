using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderMover : MonoBehaviour
{
    public int renderDistance;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(player.transform.position.x+renderDistance, this.transform.position.y, this.transform.position.z);
    }
}
