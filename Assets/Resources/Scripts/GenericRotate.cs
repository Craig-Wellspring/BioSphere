using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericRotate : MonoBehaviour
{
    public bool xRotate = false;
    public float xRotationSpeed = 1f;
    public bool yRotate = false;
    public float yRotationSpeed = 1f;
    public bool zRotate = false;
    public float zRotationSpeed = 1f;

    void Update()
    {
        if (xRotate)
            transform.Rotate(Time.deltaTime * xRotationSpeed, 0, 0);

        if (yRotate)
            transform.Rotate(0, Time.deltaTime * yRotationSpeed, 0);

        if (zRotate)
            transform.Rotate(0, 0, Time.deltaTime * zRotationSpeed);
    }
}
