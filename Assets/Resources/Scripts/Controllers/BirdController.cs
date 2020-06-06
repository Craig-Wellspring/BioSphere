using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    public Vector2 mouseSensitivity = new Vector2(250f, 250f);
    public Vector2 verticalLookClamp = new Vector2(-60, 60);

    Transform mainCamera;
    float verticalLookRotation;
    
    public float moveSpeed = 50;
    public float shiftModifier = 3f;
    public bool enableStrafe = true;
    public float strafeSpeed = 50;
    public float flapPower = 1500;

    private float finalMoveSpeed;
    private float verticalInput;
    private float horizontalInput;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        mainCamera = GetComponentInChildren<Camera>().transform;
    }


    private void Update()
    {
        //Get Inputs
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
            Flap();

        if (Input.GetKey(KeyCode.LeftShift))
            finalMoveSpeed = moveSpeed * shiftModifier;
        else
            finalMoveSpeed = moveSpeed;
    }


    private void FixedUpdate()
    {
        //Apply rotation
        transform.root.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity.x);
        verticalLookRotation += Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity.y;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, verticalLookClamp.x, verticalLookClamp.y);
        mainCamera.localEulerAngles = Vector3.left * verticalLookRotation;


        //Apply forces
        rb.AddForce(mainCamera.forward * verticalInput * finalMoveSpeed);
        if (enableStrafe)
            rb.AddForce(mainCamera.right * horizontalInput * strafeSpeed);

    }


    private void Flap()
    {
        rb.AddForce(transform.up * flapPower);
    }
}
