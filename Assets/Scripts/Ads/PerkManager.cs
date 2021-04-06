using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkManager : MonoBehaviour
{
    public DamageManager damageManager;
    public GameOverState gameOverState;
    public PlayerController playerController;
    public ChunkGenerator chunkGenerator;
    /*
     * Multiplies the stamina for the next run based on the amount specified
     * 
     * @param multiplier The number to multiply the stamina by
     */
    public void staminaPerk(float multiplier)
    {
        playerController.deflectLength *= multiplier;
    }
    /*
    * Multiplies the money earned from the next run based on the amount specified
    * 
    * @param multiplier The number to multiply the money by
    */
    public void moneyPerk(float multiplier)
    {
        //gameOverState.moneyMultiplier = multiplier;
    }
    /*
    * Multiplies the damage for the bullets based on the amount specifed
    * 
    * @param multiplier The number to multiply the health by
    */
    public void strengthPerk(float multiplier)
    {
        damageManager.bulletDamage *= multiplier;
    }
    /*
  * Multiplies the chance for powerups based on the amount specified
  * 
  * @param multiplier The number to multiply the health by
  */
    public void luckPerk(float multiplier)
    {
       // chunkGenerator.powerUpLuckMultiplier = multiplier;
    }

 
}
