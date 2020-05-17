using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class GravityBody : MonoBehaviour
{
    public bool constrainRotation = true;
    public bool useGravity = true;
    public bool alignWithGravity = true;

    private Rigidbody rbody;
    
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        rbody.useGravity = false;
        if (constrainRotation)
            rbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        if (useGravity)
            PlanetCore.Core.Attract(transform);
        if (alignWithGravity)
            PlanetCore.Core.AlignWithGravity(transform);
    }
}
