using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericRotate : MonoBehaviour
{
    [Header("Rotate Settings")]
    public bool xRotate = false;
    public float xRotationSpeed = 1f;
    public bool yRotate = false;
    public float yRotationSpeed = 1f;
    public bool zRotate = false;
    public float zRotationSpeed = 1f;

    [Header("Orbit Settings")]
    public bool rotateAround = false;
    public Transform rotationTarget;
    public enum CircuitAxis { X, Y, Z };
    public CircuitAxis circuitAxis;
    private Vector3 _circuitAxis;
    public float circuitSpeed;

    private void OnGUI()
    {
        if (circuitAxis == CircuitAxis.X)
            _circuitAxis = Vector3.right;
        if (circuitAxis == CircuitAxis.Y)
            _circuitAxis = Vector3.up;
        if (circuitAxis == CircuitAxis.Z)
            _circuitAxis = Vector3.forward;
    }

    void Update()
    {
        if (xRotate)
            transform.Rotate(Time.deltaTime * xRotationSpeed, 0, 0);

        if (yRotate)
            transform.Rotate(0, Time.deltaTime * yRotationSpeed, 0);

        if (zRotate)
            transform.Rotate(0, 0, Time.deltaTime * zRotationSpeed);


        if (rotateAround && rotationTarget != null)
        {
            transform.RotateAround(rotationTarget.position, _circuitAxis, Time.deltaTime * circuitSpeed);
        }
    }
}
