using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destruct();
        }
    }
    void Destruct()
    {
        Destroy(this.gameObject); 
    }
}
