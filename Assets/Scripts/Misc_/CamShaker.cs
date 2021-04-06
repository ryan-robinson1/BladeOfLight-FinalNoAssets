using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * CameraShaker is in charge of setting the triggers that cause the camera
 * to shake.
 * 
 * @author Maxfield Barden
 */
public class CamShaker : MonoBehaviour
{
    private Animator _canim;

    /**
     * Start is called before the first frame update.
     */
    void Start()
    {
        _canim = this.GetComponentInChildren<Animator>();
    }

    /**
     * Shakes the camera when the player is hit by a turret.
     */
    public void TurretShake()
    {
        _canim.SetTrigger("turretShake");
    }

    /**
     * Shakes the camera when the player attacks an enemy.
     */
    public void AttackShake()
    {
        _canim.SetTrigger("attackShake");
    }

    /**
     * Shakes the camera when the player gets hit by a bullet.
     */
    public void HitShake()
    {
        _canim.SetTrigger("hitShake");
    }
}
