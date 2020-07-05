using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityOrient : MonoBehaviour
{
    [SerializeField] private bool onStart = true;
    [SerializeField] private bool onUpdate = false;
    

    private void Start()
    {
        if (onStart)
            Orient();

        if (!onUpdate)
            this.enabled = false;
    }

    private void Update()
    {
        PlanetCore.Core.AlignWithGravity(transform);
    }

    public void Orient()
    {
        Vector3 gravityUp = (transform.position - Vector3.zero).normalized;
        Quaternion newRot = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;

        transform.rotation = newRot;
    }
}
