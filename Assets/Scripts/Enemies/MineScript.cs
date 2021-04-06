using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineScript : MonoBehaviour
{

    private float _removalDistance = 10;
    private float positionDifference;
    private Transform objectToFollow;
    private GameObject hero;
    private ParticleSystem mineExplosion;
    private ParticleSystem.MainModule expModule;
    private DamageManager dm;


    private void Start()
    {
        hero = GameObject.FindGameObjectWithTag("Player");
        objectToFollow = hero.transform.GetChild(1).transform;
        mineExplosion = this.GetComponentInChildren<ParticleSystem>();
        expModule = mineExplosion.main;
        expModule.startColor = ColorDataBase.GetEnemyStripAlbedo();
        dm = hero.GetComponent<DamageManager>();
    }

    /**
     *  Damages the player upon colliding with the player model.
     *  Destroys this mine object in the process and prompts it
     *  to explode.
     *  
     *  @param other The other collider
     */
    private void OnTriggerEnter(Collider other)
    {
        if (!dm.isDead() && other.gameObject.transform.CompareTag("Player"))
        {
            other.gameObject.GetComponent<DamageManager>().TakeDamageFromMine();

            mineExplosion.Play();
            this.transform.GetChild(0).gameObject.SetActive(false);
            AudioManager.instance.Play("TurretExplosion", Random.Range(0.9f, 1.15f));
        }

    }

    /**
    *  Called every fixed frame rate (used bc of physics calculations)
    */
    private void FixedUpdate()
    {
        positionDifference = transform.position.x - (objectToFollow.position.x);

        destroyModel();

    }

    /**
     * Destroys the enemy model after the player has passed it. 
     */
    private void destroyModel()
    {
        if (-positionDifference > _removalDistance)
        {
            Destroy(this.gameObject);
        }
    }
}
