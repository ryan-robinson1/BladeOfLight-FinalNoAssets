using UnityEngine;

/**
 * Gives the enemy the dissolve effect and destroys the enemy upon getting
 * hit by the player.
 * 
 * Note: There could be a more efficient way to do this, but this works
 * perfectly fine for now. - 2020.10.25
 * 
 * @author Maxfield Barden
 */
public class DissolveEnemyScript : MonoBehaviour
{

    Collider collider;
    Renderer[] renderers;
    public Material offSetDissolveMaterial;
    public Material armorMat;
    public Material armorStrip;
    public Material mineMaterial;


    private float shaderLifetime = 1f;
    private bool dissolving = false;
    private float minRender;
    private float dissolveStrength = 2f;
    private float startDelay = 0.25f;
    private string[] matNames;
   
    


    /**
     * Called before the first frame update.
     */
    void Start()
    {
        this.SetMatColors();
        collider = this.GetComponent<Collider>();
        renderers = this.GetComponentsInChildren<Renderer>();
        minRender = armorMat.GetFloat("minimumRender");
        matNames = this.generateMatNames();
    }

    /**
     * Sets the color of the enemie's mats off spawn.
     */
    public void SetMatColors()
    {
        armorMat.SetColor("dissolveColor", ColorDataBase.GetMainEnemyArmorColor());
        armorStrip.SetColor("armorEdgeColor", ColorDataBase.GetEnemyArmorStripColor());
        armorStrip.SetColor("armorStripAlbedo", ColorDataBase.GetEnemyStripAlbedo());
        offSetDissolveMaterial.SetColor("mainColor", ColorDataBase.GetStripDissolveColor());
        offSetDissolveMaterial.SetColor("dissolveEdgeColor", ColorDataBase.GetStripDissolveColor());
        mineMaterial.SetColor("mineColor", ColorDataBase.GetStripDissolveColor());
        mineMaterial.SetColor("mineAlbedo", ColorDataBase.GetMainEnemyArmorColor());

    }

    /**
     * Called every fixed framerate frame and updates the game state.
     */
    private void FixedUpdate()
    {
        if (dissolving)
        {
            this.Dissolve();
        }
    }


    /**
     * Detects when the player has struck the enemy. The collider then disables
     * and the enemy sets to destroy itself.
     * 
     * @param collision The collision between the player and enemy.
     */
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collider.enabled = false;
            // can delete following line of code to make enemy bounce back
            // slightly upon getting hit
            this.GetComponent<Rigidbody>().isKinematic = true;

        

            float pitch = Random.Range(0.93f,1.2f);

            FindObjectOfType<AudioManager>().Play("SliceSound", pitch);
             
         
            if (this.GetComponent<ShootScript>() == null)
            {
                this.GetComponent<ShotgunShoot>().enabled = false;
            }
            else
            {
                this.GetComponent<ShootScript>().enabled = false;
            }
            
            this.Invoke("SwapMats", startDelay);
            this.Destruct();
        }
    }

    /**
     * Destroys the enemy model.
     */
    private void Destruct()
    {
        Destroy(this.gameObject, shaderLifetime);
    }

    /**
     * Swaps out the materials so the enemy can dissolve.
     */
    private void SwapMats()
    {
        // change materials of each renderer on this object
        foreach (Renderer rend in renderers)
        {
            var mats = new Material[rend.materials.Length];
             for (var i = 0; i < rend.materials.Length; i++)
            {
                // set the new dissolve material according to what 
                // this material is
                if (rend.materials[i].name == matNames[0])
                {
                    mats[i] = armorMat;
                }
                else if (rend.materials[i].name == matNames[1])
                {
                    mats[i] = offSetDissolveMaterial;
                }
                else
                {
                    mats[i] = armorMat;
                }
            }
            rend.materials = mats;
        }

        dissolving = true;
    }

    /**
     * Creates the disintegration effect by scaling up the intensity of the
     * dissolve effect.
     */
    private void Dissolve()
    {
        minRender += Time.deltaTime * dissolveStrength;
        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                mat.SetFloat("minimumRender", minRender);
            }
        }
    }

    /**
     * Helper method that generates the names of our materials in the form
     * of strings in order to allow for comparison.
     * 
     * @return The names of the materials in an array of strings.
     */
    private string[] generateMatNames()
    {
        // should only ever have two mats, can adjust later if necessary
        string[] names = new string[2];
        // first entry is main material, second entry is armor strip
        names[0] = $"{armorMat} (Instance)";
        names[0] = names[0].Replace(" (UnityEngine.Material)", "");
        names[1] = $"{armorStrip} (Instance)";
        names[1] = names[1].Replace(" (UnityEngine.Material)", "");
        return names;
    }
}
