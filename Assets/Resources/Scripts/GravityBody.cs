using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBody : MonoBehaviour
{
    private Transform myTransform;
    private Rigidbody rbody;
    
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        rbody.constraints = RigidbodyConstraints.FreezeRotation;
        rbody.useGravity = false;
        myTransform = transform;
    }

    void Update()
    {
        PlanetCore.Core.Attract(myTransform);
    }
}
