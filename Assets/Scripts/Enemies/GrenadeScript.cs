using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    public Material bulletMaterial;
    public Material bulletTrailMaterial;

    private float lifeLength = 6f;
    private float lifeLengthTimer = float.PositiveInfinity;
    private bool rangeFlag= false;
    [HideInInspector]
    public float bulletSpeed;

    GameObject player;
    PlayerController pc;
    Rigidbody rb;
    private void Start()
    {
      //  bulletMaterial.SetColor("bulletColor", ColorDataBase.GetBulletColor());
     //   bulletTrailMaterial.SetColor("_EmissionColor", ColorDataBase.GetBulletTrail());

        rb = this.GetComponent<Rigidbody>();
        lifeLengthTimer = Time.time;
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();

    }
    private void FixedUpdate()
    {
        rb.velocity = new Vector3(transform.forward.x * bulletSpeed, transform.forward.y*bulletSpeed, transform.forward.z * bulletSpeed);


        if (Time.time - lifeLengthTimer > lifeLength)
            Destroy(this.gameObject); 

    }
    /**
    *  Triggers every frame. If the bullet is within "bulletRange" it triggers "BulletWithinRange" method of the characterController
    */
    private void Update()
    {
        RaycastHit hit;
        float bulletRange = 5f;
        if(Physics.Raycast(this.transform.position,this.transform.forward,
            out hit, bulletRange))
        {
            if (!rangeFlag && hit.transform.CompareTag("Player") 
                || hit.transform.CompareTag("Sword"))
            {
                pc.BulletWithinRange(); 
                rangeFlag = true;
            }
        }
    }
    /**
     *  Destroys the bullet if it hits the player and deals damage if not deflecting
     *  
     *  @param other The Collider
     */
    private void OnTriggerEnter(Collider other)
    {
        explode();
    }
    public void explode()
    {

    }
    public float getBulletSpeed()
    {
        return bulletSpeed; 
    }
}
