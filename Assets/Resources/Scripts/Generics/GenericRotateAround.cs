using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericRotateAround : MonoBehaviour
{
    [Header("Settings")]
    public Transform rotationTarget;
    public enum CircuitAxis { X, Y, Z };
    public CircuitAxis circuitAxis;
    private Vector3 _circuitAxis;
    public float circuitSpeed;

    private void OnGUI()
    {
        switch (circuitAxis)
        {
            case (CircuitAxis.X):
                _circuitAxis = Vector3.right;
                break;

            case (CircuitAxis.Y):
                _circuitAxis = Vector3.up;
                break;

            case (CircuitAxis.Z):
                _circuitAxis = Vector3.forward;
                break;
        }
    }

    void Update()
    {
        transform.RotateAround(rotationTarget.position, _circuitAxis, Time.deltaTime * circuitSpeed);
    }
}
