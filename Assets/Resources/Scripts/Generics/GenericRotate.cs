using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericRotate : MonoBehaviour
{
    public enum RotationAxis { X, Y, Z };
    public RotationAxis rotationAxis;
    Vector3 _rotationAxis;

    [SerializeField] float rotationSpeed;

    void OnGUI()
    {
        switch (rotationAxis)
        {
            case (RotationAxis.X):
                _rotationAxis = Vector3.right;
                break;

            case (RotationAxis.Y):
                _rotationAxis = Vector3.up;
                break;

            case (RotationAxis.Z):
                _rotationAxis = Vector3.forward;
                break;
        }
    }

    void Update()
    {
        transform.Rotate(_rotationAxis * rotationSpeed * Time.deltaTime);
    }
}
