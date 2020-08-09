using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowTarget : MonoBehaviour
{
    public Transform target;
    private Vector3 targetPos;
    private Quaternion targetRot;
    public Vector3 offset = new Vector3(0, 5, -10);
    public float smoothTime = .3f;
    private Vector3 velocity = Vector3.zero;

    void FixedUpdate()
    {
        if (target == null)
        {
            targetPos = transform.position + offset;
        }
        else
        {
            targetPos = target.TransformPoint(offset);
            targetRot = Quaternion.LookRotation(target.root.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 5 * Time.deltaTime);
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }
}
