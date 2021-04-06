using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controls the color of the blade object.
 * 
 * @author Maxfield Barden
 */
public class SwordColorScript : MonoBehaviour
{
    public Material bladeColor;
    public Material slashMaterial;
    private ParticleSystem.MainModule bladeSmoke;
    private Color swordEmission;
    private Color swordAlbedo;

    /**
     * Called before the first frame update.
     */
    void Start()
    {
        setSwordColor();
    }
    public void setSwordColor()
    {
        swordEmission = ColorDataBase.GetSwordColor();
        swordAlbedo = ColorDataBase.GetSwordAlbedo();
        try
        {
            bladeSmoke = transform.GetChild(0).GetComponentInChildren<
                ParticleSystem>().main;
            bladeSmoke.startColor = swordAlbedo;
        }
        catch 
        {
        }

        slashMaterial.SetColor("slashColor", swordEmission);

        bladeColor.SetColor("baseColor",
            swordAlbedo);
        bladeColor.SetColor("emissionColor",
            swordEmission);
    }

}
