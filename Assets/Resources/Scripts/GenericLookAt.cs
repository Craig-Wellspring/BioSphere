using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericLookAt : MonoBehaviour
{
    public Transform targetTransform;
    

    void FixedUpdate()
    {
        transform.LookAt(targetTransform);
    }
}
