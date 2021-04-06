using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestructScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("destroyObject", 1);
    }
    void destroyObject()
    {
        Destroy(this.gameObject);
    }
}
