using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerAnimationController))]
public class PlayerController : MonoBehaviour
{

    private PlayerAnimationController _animController;
    private BulletScript bScript;

    [HideInInspector]
    public ScoreCounter sCounter;

    public TimeManagerScript timeManager;
    public Image StaminaBar;

    [SerializeField]
    private ParticleSystem hitEffect;
    [SerializeField]
    private Transform lookAtPoint;

    [HideInInspector]
    public bool deflecting;

    Rigidbody _rb;
    Transform _tf;
    CapsuleCollider _cc;
    AudioManager _am;
    public CamShaker _cShaker;

    [HideInInspector]
    public int _playerPositionOffset = 0;
    private float speed = 15f;
    private int deflects = 0;

    public float timeSinceLastDodge = 0f;

    Dictionary<int, float> positionMap =
    new Dictionary<int, float>();
    public Image staminaFillShader;
    private Color grey = new Vector4(0.58f, 0.58f, 0.58f,1f);
    private Color white = new Vector4(0.88f, 0.88f, 0.88f, 0.88f);

    public bool setBarWhiteBool;


    public List<Transform> deflectParticlePositions;

    //Deflect timers
    [HideInInspector]
    public float deflectLength = 1.7f; //1.5
    private float deflectTimer = float.PositiveInfinity;
    private float staminaRefreshLength = 3f;
    private float staminaRefreshTimer = float.NegativeInfinity;
    private float currentFillCapacity = 0f;

    //pitch timer
    private float pitchComboTimer = float.PositiveInfinity;
    private float deflectComboLength = 0f;
    private float currentPitch = 1f;

    private float transitionSpeed = 6.25f;

    public bool loading = false;



    private void Start()
    {
        _animController = this.GetComponent<PlayerAnimationController>();
        _rb = this.GetComponent<Rigidbody>();
        _tf = this.GetComponent<Transform>();
        _cc = this.GetComponent<CapsuleCollider>();
        _am = FindObjectOfType<AudioManager>();
        _rb.velocity = new Vector3(speed, 0, 0);
        Invoke("timeSinceLastDodgeCounter", 1f);

        positionMap[-1] = -1.75f;
        positionMap[-2] = -3.5f;
        positionMap[0] = 0f;
        positionMap[1] = 1.75f;
        positionMap[2] = 3.5f;






    }

    public Vector3 getVelocity() { return new Vector3(speed, 0, 0); }

    private void Update() {

        UpdateMove();
        if (!loading){
            ChangeLookAtPoint();
            resetCurrentPitch();
            Deflect();

        }

        
        
       
       
    }
    void ChangeLookAtPoint()
    {
        if (_animController.IsRunning())
        {

            lookAtPoint.localPosition = new Vector3(0, 0.4f, 0);


        }
        else if (_animController.IsSliding())
        {
            lookAtPoint.localPosition = new Vector3(0, -0.4f, 0);
        }

        if (Input.GetKeyDown(KeyCode.S) || SwipeInput.Instance.SwipeDown)
        {
            _am.Pause("Footsteps");
            this.ChangeBCHeight();
        }
    }

    /**
     * Changes the height of the collider in case we slide.
     */
    public void ChangeBCHeight()
    {
        try
        {
            _cc.height = 1f;
            _cc.center = new Vector3(_cc.center.x, -0.5f, _cc.center.z);
        }
        catch
        {

        }
    }
    void resetBC()
    {
        _cc.center = new Vector3(_cc.center.x, 0.5f, _cc.center.z);
        _cc.height = 3f;
        _am.UnPause("Footsteps");
    }
    void UpdateMove()
    {
        _rb.velocity = new Vector3(speed, 0, 0);
        if (!loading)
        {
            if (Time.time % 600 == 0 && speed < 18)
            {
                speed++;
            }

            if ((Input.GetKeyDown(KeyCode.A) || SwipeInput.Instance.SwipeLeft))
            {
                if (_playerPositionOffset < 2)
                {
                    _playerPositionOffset++;
                    timeSinceLastDodge = 0;
                }
            }

            else if ((Input.GetKeyDown(KeyCode.D) || SwipeInput.Instance.SwipeRight))
            {
                if (_playerPositionOffset > -2)
                {
                    _playerPositionOffset--;
                    timeSinceLastDodge = 0;
                }
            }

            _tf.position = Vector3.Lerp(_tf.position,
                new Vector3(_tf.position.x, _tf.position.y,
                positionMap[_playerPositionOffset]),
                this.GetTransitionOffset(Time.deltaTime * transitionSpeed));
        }
       

    }
    private void timeSinceLastDodgeCounter()
    {
        timeSinceLastDodge++;
        Invoke("timeSinceLastDodgeCounter", 1f);
    }
    
    /**
     * Allows the dodge to start slow, become fast in the middle, then
     * slow back down again towards the end. Gives the dodge more of
     * a "jumpy" feel.
     * 
     * @param x The argument being passed into the circle function.
     * @return The transition speed offset.
     */
    private float GetTransitionOffset(float x)
    {
        // equation for least squares line
        float y = -1 / 25 + (27 / 25) * x;
        return y;
    }

    /**
     * Gets the number of deflects in one deflect animation cycle.
     */

    public int getDeflects()
    {
        return deflects;
    }

    /**
     * Sets the number of deflects
     */
    public int setDeflects(int def)
    {
        deflects = def;
        return deflects;
    }

    void Deflect()
    {
        if (Time.time - staminaRefreshTimer >= staminaRefreshLength)
        {

            if (Time.time - deflectTimer >= deflectLength)
            {
                deflecting = false;
                deflectTimer = float.PositiveInfinity;
                StaminaBar.fillAmount = 0;
                staminaRefreshTimer = Time.time;
                _animController.UpdateDeflect();
                currentFillCapacity = 0;
                deflects = 0;
             
                staminaFillShader.color = grey;

            }
            else if ((SwipeInput.Instance.SwipeDown || Input.GetKeyDown(KeyCode.S)) && deflecting)
            {
                currentFillCapacity = StaminaBar.fillAmount;
                staminaFillShader.color = grey;
                staminaRefreshTimer = Time.time;
                deflecting = false;
                deflects = 0;
                deflectTimer = float.PositiveInfinity;
                _animController.UpdateDeflect();
            }
            else if ((SwipeInput.Instance.SwipeUp || Input.GetKeyDown(KeyCode.W)) && !deflecting)
            {
                deflecting = true;
                deflectTimer = Time.time;
                _animController.UpdateDeflect();
            }
            else if (deflectTimer != float.PositiveInfinity)
            {
                StaminaBar.fillAmount = 1 - ((Time.time - deflectTimer) / deflectLength);
            }
        }
        else
        {
            StaminaBar.fillAmount = currentFillCapacity + ((Time.time - (staminaRefreshTimer)) / staminaRefreshLength);

            if (!setBarWhiteBool)
            {
               // Invoke("setBarWhite", staminaRefreshLength-0.05f);
                setBarWhiteBool = true;
            }
         
            if (StaminaBar.fillAmount >= 0.98)
            {
                staminaRefreshTimer = 0;
              
               
            }
        }
        if (StaminaBar.fillAmount >= 0.98)
        {
            staminaFillShader.color = white;
        }
        



    }
    public void setBarWhite()
    {
        
        setBarWhiteBool = false;
    }
    public void loadDeflect()
    {
        deflecting = true;
    }
    //Plays the hit particle system. Activated by BulletScript
    public void PlayHitEffect(Transform t)
    {
        /*  float pitch = Random.Range(0.95f, 1.15f);
          if (Time.time - pitchComboTimer < deflectComboLength)
          {
              pitch = currentPitch + 0.02f;

          }
          currentPitch = pitch;
          pitchComboTimer = Time.time;*/

        float pitch = Random.Range(0.95f, 1.10f);
        string[] sounds = {"Deflect1","Deflect2","Deflect3","Deflect4"};
        float[] weightCDF = {0.45f,0.65f,0.85f,1};
        _am.PlaySoundFromArray(sounds, weightCDF,pitch);
       


        ParticleSystem particle = Instantiate(hitEffect, GetClosestObject(deflectParticlePositions, t).position, Quaternion.Euler(0, 100, 0), this.gameObject.transform);
        var main = particle.main;
        // main.simulationSpeed = 0.3f;
        particle.Play();

    }

    /* 
     * Resets the default pitch back to 1 after the combo time period has passed
     */
    void resetCurrentPitch()
    {
        if (Time.time - pitchComboTimer > deflectComboLength)
        {
            currentPitch = 1f;
            Debug.Log("reset");
        }
    }
    Transform GetClosestObject(List<Transform> objects, Transform fromThis)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = fromThis.position;
        foreach (Transform potentialTarget in objects)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }
        return bestTarget;
    }

    /**
     * Detects a collision with a trigger. Used for detecting collisions
     * with powerup objects.
     * 
     */
    private void OnTriggerEnter(Collider other)
    {
        // powerup layer
        if (other.gameObject.layer == 9)
        {
            if (other.gameObject.CompareTag("HealthPack"))
            {
                other.gameObject.GetComponent<HealthPackScript>().
                    HealPlayer(this.gameObject);
                sCounter.JustHealed();
                _animController.SetElimSinceHeal();
            }
        }
    }


    /* 
     * Executed by bulletScript if within the range defined by the variable "bulletrange" in bulletScript's update function
     * 
     * @return True if we are deflecting and there is a bullet within range.
     */
    public void BulletWithinRange()
    {
        if (deflecting)
        {
            deflects++;
            sCounter.AddDeflectScore();
            if (deflects >= 5)
            {
                deflects = 1;
            }
        }
    }
}
