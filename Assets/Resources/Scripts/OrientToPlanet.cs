using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientToPlanet : MonoBehaviour
{
    private void Start()
    {
        Vector3 gravityUp = (transform.position - PlanetCore.Core.transform.position).normalized;
        Quaternion newRot = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
    
        transform.rotation = newRot;
    }
}
