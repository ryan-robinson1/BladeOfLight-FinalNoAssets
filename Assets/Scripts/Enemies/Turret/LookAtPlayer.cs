using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private TurretShootScript _ts;
    private Transform _obf;
    // Start is called before the first frame update
    void Start()
    {
        _ts = this.GetComponentInParent<TurretShootScript>();
        _obf = _ts.GetHeroRef();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(_obf);
    }
}
