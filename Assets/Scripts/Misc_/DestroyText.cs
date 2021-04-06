
using UnityEngine;

public class DestroyText : MonoBehaviour
{
    // Start is called before the first frame update

    public float destroyTime=3f;
    void Start()
    {
        Destroy(this.gameObject, destroyTime);
    }
}
