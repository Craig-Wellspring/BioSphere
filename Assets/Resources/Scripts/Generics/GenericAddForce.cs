using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAddForce : MonoBehaviour
{
    private enum ForceDirection { Forward, Backward, Up, Down, Right, Left };
    [SerializeField] private ForceDirection forceDirection;
    [SerializeField] private float velocity;
    [SerializeField] private bool onUpdate = false;
    [SerializeField] private bool instantForce = false;

    Vector3 direction;

    Rigidbody rBody;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();

        if (instantForce)
            rBody.AddForce(direction * velocity * rBody.mass * 10);

        if (!onUpdate)
            enabled = false;
    }
    
    void FixedUpdate()
    {
        rBody.AddForce(direction * velocity);
    }

    private void OnValidate()
    {
        if (forceDirection == ForceDirection.Forward)
            direction = transform.forward;
        else if (forceDirection == ForceDirection.Backward)
            direction = -transform.forward;
        else if (forceDirection == ForceDirection.Up)
            direction = transform.up;
        else if (forceDirection == ForceDirection.Down)
            direction = -transform.up;
        else if (forceDirection == ForceDirection.Right)
            direction = transform.right;
        else if (forceDirection == ForceDirection.Left)
            direction = -transform.right;
    }
}
