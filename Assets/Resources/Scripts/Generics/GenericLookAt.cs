using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericLookAt : MonoBehaviour
{
    public Transform targetTransform;
    public bool inverted;
    

    void FixedUpdate()
    {
        if (inverted)
            transform.rotation = Quaternion.LookRotation(transform.position - targetTransform.position);
        else
            transform.LookAt(targetTransform.position);
    }
}
