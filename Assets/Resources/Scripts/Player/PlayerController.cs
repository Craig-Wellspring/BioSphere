using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PlayerController : MonoBehaviour
{
    // Inspector Variables
    [SerializeField] bool mouseCamera = false;
    [SerializeField] Vector2 mouseSensitivity = new Vector2(250f, 250f);
    [SerializeField] Vector2 verticalLookClamp = new Vector2(-60, 60);
    [Space(10)]
    [SerializeField] float turnSpeed = 120f;
    [SerializeField] int jumpPower = 10;
    [SerializeField] LayerMask surfaceMask;


    #region Internal Variables
    // Camera Variables
    Cinemachine.CinemachineVirtualCamera vCam;
    Camera mainCam;


    // Private Variables
    Respiration respiration;
    Metabolism metabolism;

    Runner runner;
    Seeker seeker;
    Rigidbody rBody;
    Animator animator;
    BioCreatureAnimData animData;


    // Movement Variables
    float verticalLookRotation;
    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;
    #endregion

    void Start()
    {
        vCam = GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
        mainCam = PlayerSoul.Cam.GetComponent<Camera>();

        respiration = transform.root.GetComponentInChildren<Respiration>();
        metabolism = transform.root.GetComponentInChildren<Metabolism>();

        runner = transform.root.GetComponent<Runner>();
        seeker = transform.root.GetComponent<Seeker>();
        rBody = transform.root.GetComponent<Rigidbody>();
        animator = transform.root.GetComponent<Animator>();
        animData = transform.root.GetComponent<BioCreatureAnimData>();
    }

    void Update()
    {
        MouseInput();
        MovementInput();
        AuxInput();

        animator.SetFloat("Speed", Input.GetAxisRaw("Vertical") * animator.GetFloat("Speed"));
        //animator.SetFloat("TurnSpeed", Input.GetAxisRaw("Horizontal"));
    }
    void FixedUpdate()
    {
        if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
            rBody.MovePosition(rBody.position + transform.root.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    void MouseInput()
    {
        // Move Camera
        if (mouseCamera)
        {
            verticalLookRotation += Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity.x;
            verticalLookRotation += Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity.y;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, verticalLookClamp.x, verticalLookClamp.y);

            vCam.transform.localEulerAngles = Vector3.left * verticalLookRotation;
        }


        if (Input.GetMouseButtonDown(0))
        {
        }

        if (Input.GetMouseButtonDown(1))
        {
        }
    }

    void MovementInput()
    {
        // Sprint
        if (Input.GetButtonDown("Sprint"))
            respiration.ToggleSprinting(true);
        if (Input.GetButtonUp("Sprint"))
            respiration.ToggleSprinting(false);

        // Calculate rotation
        transform.root.Rotate(Vector3.up * Input.GetAxis("Horizontal") * Time.deltaTime * turnSpeed);

        // Calculate movement
        Vector3 moveDir = new Vector3(0, 0, Input.GetAxisRaw("Vertical")).normalized;
        Vector3 targetMoveAmount = Input.GetAxisRaw("Vertical") < 0 ? moveDir * (runner.moveSpeed) / 2 : moveDir * (runner.moveSpeed);
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, 0.15f);
    }

    void AuxInput()
    {
        // Eat
        if (Input.GetButtonDown("Eat") && !metabolism.isEating)
            animator.SetTrigger("Bite");
        if (Input.GetButtonUp("Eat") && metabolism.isEating)
            metabolism.StopEating();

        // Sing
        if (Input.GetButtonDown("Sing") && animator.parameters.ToString().Contains("IsSinging"))
            animator.SetBool("IsSinging", true);
        if (Input.GetButtonUp("Sing") && animator.parameters.ToString().Contains("IsSinging"))
            animator.SetBool("IsSinging", false);

        // Jump
        if (Input.GetButtonDown("Jump") && IsGrounded())
            Jump();
    }

    void Jump()
    {
        animator.SetTrigger("Jump");
        rBody.AddForce(UtilityFunctions.GravityVector(transform.root.position) * jumpPower);
    }

    bool IsGrounded()
    {
        Ray ray = new Ray(transform.root.position + (transform.root.up * 0.2f), -transform.root.up);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.4f, surfaceMask))
            return true;
        else return false;
    }

    /*
    void ClearPathing()
    {
        transform.root.GetComponent<AIDestinationSetter>().target = null;
        pathing.SetPath(null);
        pathing.destination = Vector3.positiveInfinity;
        seeker.CancelCurrentPathRequest();
    }
    */
}
