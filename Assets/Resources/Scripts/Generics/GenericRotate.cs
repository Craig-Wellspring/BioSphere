using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericRotate : MonoBehaviour
{
    public enum RotationAxis { X, Y, Z };
    public RotationAxis rotationAxis;
    private Vector3 _rotationAxis;

    public float rotationSpeed;

    private void OnGUI()
    {
        if (rotationAxis == RotationAxis.X)
            _rotationAxis = Vector3.right;
        if (rotationAxis == RotationAxis.Y)
            _rotationAxis = Vector3.up;
        if (rotationAxis == RotationAxis.Z)
            _rotationAxis = Vector3.forward;
    }

    void Update()
    {
        transform.Rotate(_rotationAxis * rotationSpeed * Time.deltaTime);
    }
}
