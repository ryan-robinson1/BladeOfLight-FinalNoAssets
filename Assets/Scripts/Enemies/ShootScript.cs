using UnityEngine;

public class ShootScript : MonoBehaviour
{
    public GameObject bullet;



    public PlayerController player;
    public Transform objectToFollow;
    public GameObject playerObject;
    public Transform gunBarrel;
    public Transform torso;
    public Transform head;
    public Transform legs;
    private Rigidbody _rb;


    private float _bulletSpeed = 20f;
    private float _walkSpeed = 3f;
    private float _removalDistance = 10;
    private float _moveBackwardDistance;
    private float positionDifference;


    private float _torsoMoveSpeed;
    private float _headMoveSpeed;
    private float _legMoveSpeed;



    private float ammo = 5;
    private float timeInBetweenShots = 0.85f;
    private float reloadTime = 3f;
    private float reloadTimer = float.NegativeInfinity;
    private bool shooting = false;

    Quaternion _lookRotation;
    Vector3 _direction;
    Animator _anim;
    ParticleSystem _muzzleFlash;
    private ParticleSystem.MainModule _mfMain;

    private DamageManager dm;


    private void Start()
    {

        _torsoMoveSpeed = 40;
        _headMoveSpeed = 45;
        _legMoveSpeed = 30;


        if (torso == null)
            Debug.Log("Did not assign torso in hierarchy");
        if (head == null)
            Debug.Log("Did not assign head in hierarchy");
        if (legs == null)
            Debug.Log("Did not assign legs in hierarchy");


        _moveBackwardDistance = 17;
        _rb = this.GetComponent<Rigidbody>();
        _anim = this.GetComponentInChildren<Animator>();
        _muzzleFlash = this.GetComponentInChildren<ParticleSystem>();

        playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<PlayerController>();
        objectToFollow = playerObject.transform.GetChild(1).transform;

        // muzzle flash color
        _mfMain = _muzzleFlash.main;
        _mfMain.startColor = ColorDataBase.GetEnemyStripAlbedo();

        dm = player.GetComponent<DamageManager>();
    }


    /**
    *  Called every fixed frame rate (used bc of physics calculations)
    */
    private void FixedUpdate()
    {
        positionDifference = transform.position.x - (objectToFollow.position.x);
        MoveBackwards();
        if (positionDifference < Random.Range(80, 10) && !shooting)
        {
            InvokeRepeating("Shoot", 0f, timeInBetweenShots);
            shooting = true;
        }
        else
        {
            _anim.SetBool("shooting", false);
        }

        destroyModel();
        LookAtTarget();



    }
    /**
    *  Rotates the object to face the lookat point
    */
    private void LookAtTarget()
    {

        Vector3 _lookDirection = objectToFollow.position - transform.position;
        _lookDirection.y = 0;
        Quaternion _rot = Quaternion.LookRotation(_lookDirection, Vector3.up);

        torso.transform.rotation = Quaternion.Lerp(transform.rotation, _rot, _torsoMoveSpeed * Time.time);
        head.transform.rotation = Quaternion.Lerp(transform.rotation, _rot, _headMoveSpeed * Time.time);
        legs.transform.rotation = Quaternion.Lerp(transform.rotation, _rot, _legMoveSpeed * Time.time);

    }

    /**
     * Returns the look rotation so it can be used by the DissolveEnemyScript.
     * 
     * @return The look rotation.
     */
    public Quaternion getLookRotation()
    {
        return _lookRotation;
    }

    /**
     * Sets the _lookRotation.
     * 
     * @param rot The look rotation to be set.
     */
    public void setLookRotation(Quaternion rot)
    {
        _lookRotation = rot;
    }

    /**
    * Makes the enemy walk backwards if the player gets within a certain distance (positionDifference)
    */
    private void MoveBackwards()
    {
        if (positionDifference < _moveBackwardDistance && positionDifference > 0)
        {
            _rb.velocity = new Vector3(transform.forward.x * -_walkSpeed, 0, transform.forward.z * -(1f / positionDifference * 2));
            _anim.SetTrigger("PlayWalkBackward");
        }
    }

    /**
     *  Shoots the gun at the predicted point returned by CalculateInterceptPosition. Is modified by 'ammo,' 'reloadTime,' and 'timeInBetweenShots.'
     */
    private void Shoot()
    {
        if (!dm.isDead() && ammo > 0 && Time.time - reloadTimer > reloadTime+Random.Range(0,0.4f) && positionDifference > 6)
        {
            Vector3 _pointToAimAt = CalculateInterceptPosition(objectToFollow.position);
            _direction = (_pointToAimAt - transform.position).normalized;
            _lookRotation = Quaternion.LookRotation(_direction);

            RaycastHit hit;


            Vector3 spawnPos = gunBarrel.transform.position + gunBarrel.transform.forward;

            //Sets the bullet speed in the script
            GameObject b = Instantiate(bullet, spawnPos, _lookRotation);
            b.GetComponent<BulletScript>().bulletSpeed = _bulletSpeed;
            _anim.SetBool("shooting", true);
            _muzzleFlash.Play();
        }
        else if (ammo == 0)
        {
            _anim.SetBool("shooting", false);
            reloadTimer = Time.time;
            ammo = 5;
        }


    }

    /**
     * Destroys the enemy model after the player has passed it. 
     */
    private void destroyModel()
    {
        if (-positionDifference > _removalDistance)
        {
            player.GetComponent<PlayerAnimationController>().resetAttackMultiplier();
            Destroy(this.gameObject);
        }
    }
    /**
    *  Helper method that calculates the point where the enemy should shoot at
    *  
    *  @param targetPosition The current position of the target
    *  
    *  @return A random vector3 in between the target position and the predicted intercept position. Accuracy of shot can be modified in inaccurateInterceptPoint
    */
    private Vector3 CalculateInterceptPosition(Vector3 targetPosition)
    {
        Vector3 interceptPoint = FirstOrderIntercept(
            this.transform.position, _rb.velocity, _bulletSpeed, new Vector3(targetPosition.x, targetPosition.y - 1.5f, targetPosition.z), player.getVelocity());
        Vector3 inaccurateInterceptPoint = new Vector3(Random.Range(targetPosition.x, interceptPoint.x), interceptPoint.y, Random.Range(targetPosition.z, interceptPoint.z));

        if (player.timeSinceLastDodge > 8)
        {
            return interceptPoint;
        }
        return inaccurateInterceptPoint;
    }

    /**
     *  Calculates the intercept 
     *  
     *  @param shooterPosition, shooterVelocity, shotSpeed, targetPosition, targetVelocity
     *  
     *  @return The calculated intercept point that the player will be at if it continues on current trajectory
     */
    public static Vector3 FirstOrderIntercept
    (
        Vector3 shooterPosition,
        Vector3 shooterVelocity,
        float shotSpeed,
        Vector3 targetPosition,
        Vector3 targetVelocity
    )
    {
        Vector3 targetRelativePosition = targetPosition - shooterPosition;
        Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;


        float t = FirstOrderInterceptTime
        (
            shotSpeed,
            targetRelativePosition,
            targetRelativeVelocity
        );

        return targetPosition + t * (targetRelativeVelocity);
    }
    //Uses relative position to calculate the first intercept
    public static float FirstOrderInterceptTime
    (
        float shotSpeed,
        Vector3 targetRelativePosition,
        Vector3 targetRelativeVelocity
    )
    {
        float velocitySquared = targetRelativeVelocity.sqrMagnitude;
        if (velocitySquared < 0.001f)
            return 0f;

        float a = velocitySquared - shotSpeed * shotSpeed;

        //handles alike velocities
        if (Mathf.Abs(a) < 0.001f)
        {
            float t = -targetRelativePosition.sqrMagnitude /
            (
                2f * Vector3.Dot
                (
                    targetRelativeVelocity,
                    targetRelativePosition
                )
            );
            return Mathf.Max(t, 0f);
        }

        float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
        float c = targetRelativePosition.sqrMagnitude;
        float determinant = b * b - 4f * a * c;


        if (determinant > 0f)
        { //determinant > 0; two intercept paths (most common)

            float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a),
                    t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);

            if (t1 > 0f)
            {
                if (t2 > 0f)
                    return Mathf.Min(t1, t2); //both are positive
                else
                    return t1; //only t1 is positive
            }
            else
                return Mathf.Max(t2, 0f); //don't shoot back in time
        }
        else if (determinant < 0f) //determinant < 0; no intercept path
            return 0f;
        else //determinant = 0; one intercept path, pretty much never happens
            return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
    }


}
