using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class GravityBody : MonoBehaviour
{
    private Rigidbody rbody;
    
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        rbody.constraints = RigidbodyConstraints.FreezeRotation;
        rbody.useGravity = false;
    }

    void FixedUpdate()
    {
        PlanetCore.Core.Attract(transform);
    }
}
