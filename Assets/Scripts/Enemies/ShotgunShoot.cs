using UnityEngine;

public class ShotgunShoot : MonoBehaviour
{
    public GameObject shotgunShell;



    private PlayerController hero1;
    private Transform playerToFollow;
    private GameObject playerObject1;
    public Transform shotgunBarrel;
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



    private float ammo = 3;
    private float timeInBetweenShots = 1.6f;
    private float reloadTime = 0f;
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


        _moveBackwardDistance = 25;
        _rb = this.GetComponent<Rigidbody>();
        _anim = this.GetComponentInChildren<Animator>();
        _muzzleFlash = this.GetComponentInChildren<ParticleSystem>();

        playerObject1 = GameObject.FindGameObjectWithTag("Player");
        hero1 = playerObject1.GetComponent<PlayerController>();
        playerToFollow = playerObject1.transform.GetChild(1).transform;

        // muzzle flash color
        _mfMain = _muzzleFlash.main;
        _mfMain.startColor = ColorDataBase.GetEnemyStripAlbedo();
        dm = playerObject1.GetComponent<DamageManager>();

    }


    /**
    *  Called every fixed frame rate (used bc of physics calculations)
    */
    private void FixedUpdate()
    {
        positionDifference = transform.position.x - (playerToFollow.position.x);
        MoveBackwards();
        if (positionDifference < 50 && positionDifference > 6 && !shooting)
        {
            InvokeRepeating("Shoot", 0f, timeInBetweenShots);
            shooting = true;
        }
        else
        {
            //_anim.SetBool("shooting", false);
        }

        destroyModel();
        LookAtTarget();



    }

    /**
     * Gets hero ref
     */
    public Transform GetHero()
    {
        return playerToFollow;
    }
    /**
    *  Rotates the object to face the lookat point
    */
    private void LookAtTarget()
    {
        
        Vector3 _lookDirection = playerToFollow.position - transform.position;
        _lookDirection.y = 0;
        Quaternion _rot = Quaternion.LookRotation(_lookDirection, Vector3.up);
        _rot.y += 0.28f;

        torso.transform.rotation = Quaternion.Lerp(transform.rotation, _rot, _torsoMoveSpeed * Time.time);
        head.transform.rotation = Quaternion.Lerp(transform.rotation, _rot, _headMoveSpeed * Time.time);
        legs.transform.rotation = Quaternion.Lerp(transform.rotation, _rot, _legMoveSpeed * Time.time);
        
       // this.transform.LookAt(playerToFollow);

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
            _anim.SetTrigger("walk");
        }
    }

    /**
     *  Shoots the gun at the predicted point returned by CalculateInterceptPosition. Is modified by 'ammo,' 'reloadTime,' and 'timeInBetweenShots.'
     */
    private void Shoot()
    {
        if (!dm.isDead() && ammo > 0 && Time.time - reloadTimer > reloadTime + Random.Range(0, 0.4f) && positionDifference > 6)
        {
            Vector3 _pointToAimAt = CalculateInterceptPosition(playerToFollow.position);
            _direction = (_pointToAimAt - transform.position).normalized;
            _lookRotation = Quaternion.LookRotation(_direction);

            Quaternion rightRot = _lookRotation;
            rightRot.y -= 0.05f;
            Quaternion leftRot = _lookRotation;
            leftRot.y += 0.05f;


            RaycastHit hit;


            Vector3 spawnPos = shotgunBarrel.transform.position;

            //bullet to the right
            GameObject a = Instantiate(shotgunShell, spawnPos, rightRot);
            a.GetComponent<BulletScript>().bulletSpeed = _bulletSpeed;

            // bullet to the middle
            GameObject b = Instantiate(shotgunShell, spawnPos, _lookRotation);
            b.GetComponent<BulletScript>().bulletSpeed = _bulletSpeed;

            // bullet to the left
            GameObject c = Instantiate(shotgunShell, spawnPos, leftRot);
            c.GetComponent<BulletScript>().bulletSpeed = _bulletSpeed;
 
            _anim.SetTrigger("shoot");
            _muzzleFlash.Play();
            ammo--;
        }
        else if (ammo == 0)
        {
           // _anim.SetBool("shooting", false);
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
            hero1.GetComponent<PlayerAnimationController>().resetAttackMultiplier();
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
        Vector3 interceptPoint = ShootScript.FirstOrderIntercept(
            this.transform.position, _rb.velocity, _bulletSpeed, new Vector3(targetPosition.x, targetPosition.y - 1.5f, targetPosition.z), hero1.getVelocity());
        Vector3 inaccurateInterceptPoint = new Vector3(Random.Range(targetPosition.x, interceptPoint.x), interceptPoint.y, Random.Range(targetPosition.z, interceptPoint.z));

        if (hero1.timeSinceLastDodge > 8)
        {
            return interceptPoint;
        }
        return inaccurateInterceptPoint;
    }
}
