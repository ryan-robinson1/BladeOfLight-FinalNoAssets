using UnityEngine;

public class ShotgunFollow : MonoBehaviour
{
    private ShotgunShoot _ss;
    private Transform _obf;
    // Start is called before the first frame update
    void Start()
    {
        _ss = this.GetComponentInParent<ShotgunShoot>();
        _obf = _ss.GetHero();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(_obf.position);
    }
}
