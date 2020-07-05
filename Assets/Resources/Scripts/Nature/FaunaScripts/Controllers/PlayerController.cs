using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector2 mouseSensitivity = new Vector2(250f, 250f);
    public Vector2 verticalLookClamp = new Vector2(-60, 60);

    Transform mainCam;
    float verticalLookRotation;

    Vector3 moveAmount;

    private void Start()
    {
        mainCam = GetComponentInChildren<Camera>().transform;
    }

    void Update()
    {
        transform.root.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity.x);
        verticalLookRotation += Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity.y;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, verticalLookClamp.x, verticalLookClamp.y);
        mainCam.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    private void FixedUpdate()
    {
        
    }
}
