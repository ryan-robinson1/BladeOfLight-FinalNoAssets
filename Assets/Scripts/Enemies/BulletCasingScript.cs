using UnityEngine;


/**
 * Controls the bullet_casing prefab. Destroys the bullet casing after 6 seconds
 * and applies drag to the object once it hits the ground.
 * 
 * @author Maxfield Barden
 */
public class BulletCasingScript : MonoBehaviour
{

    private float lifeTime = 6f;
    
    /**
     * Called every fixed framerate frame and updates the game state.
     */
    private void FixedUpdate()
    {
        Destroy(this.gameObject, lifeTime);
    }

    /**
     * Increases the drag over time once the bullet casing collides with the ground.
     * This slows down the casing's velocity and gives it more of a random position
     * on the ground.
     * 
     * @param collision Detects when the object has been collided with.
     */
    private void OnCollisionEnter(Collision collision)
    { 
        float dragForce = 5f;
        dragForce += Time.deltaTime;
        this.GetComponent<Rigidbody>().drag = dragForce;
    }
}
