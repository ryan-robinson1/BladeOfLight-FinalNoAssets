using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    public Transform objectToFollow;
    public Vector3 goalOffset;
    private Vector3 startOffset;
    private Vector3 offset;
    public float followSpeed = 10f;
    public float lookSpeed = 10;
    private bool inPosition = false;

    private void Start()
    {
        startOffset = transform.position - objectToFollow.position;
        startOffset.x += 0.5f;
        startOffset.z += 1.5f;
        offset = startOffset;
    }

    public void LookAtTarget()
    {
        Vector3 _lookDirection = objectToFollow.position - transform.position;
        Quaternion _rot = Quaternion.LookRotation(_lookDirection, Vector3.up);

        transform.rotation = Quaternion.Lerp(transform.rotation, _rot, lookSpeed * Time.deltaTime);
    }
    public void MoveToTarget()
    {

        Vector3 _targetPos = new Vector3(objectToFollow.position.x,objectToFollow.position.y, objectToFollow.position.z) +
                             objectToFollow.forward * offset.z +
                             objectToFollow.right * offset.x +
                             objectToFollow.up * offset.y;
        transform.position = Vector3.Lerp(transform.position, _targetPos, followSpeed * Time.deltaTime);
    }

    /**
     * Helps move the cameraOffset at the start of the game.
     */
    private void SetCameraOffset()
    {
        if (!inPosition)
        {
            offset = Vector3.Lerp(offset, goalOffset, 1.25f * Time.deltaTime);
            if (offset == goalOffset)
            {
                inPosition = true;
            }
        }
    }
    private void FixedUpdate()
    {
        SetCameraOffset();
        LookAtTarget();
        MoveToTarget();
    }
   

}


