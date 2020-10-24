using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PlayerController : AdvancedMonoBehaviour
{
    // Inspector Variables
    [SerializeField] Vector2 mouseSensitivity = new Vector2(250f, 250f);
    [SerializeField] Vector2 verticalLookClamp = new Vector2(-60, 60);
    [SerializeField] int jumpPower = 10;
    [SerializeField] LayerMask surfaceMask;


    #region Internal Variables
    // Camera Variables
    Cinemachine.CinemachineVirtualCamera vCam;
    Camera mainCam;


    // Private Variables
    Respiration respiration;
    Seeker seeker;
    AIPathAlignedToSurface pathing;
    Rigidbody rBody;
    Animator animator;


    // Movement Variables
    float verticalLookRotation;
    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;
    #endregion

    void Start()
    {
        vCam = GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
        mainCam = PlayerSoul.Cam.GetComponent<Camera>();

        respiration = GetComponentInParent<Respiration>();
        seeker = transform.root.GetComponent<Seeker>();
        pathing = transform.root.GetComponent<AIPathAlignedToSurface>();
        rBody = transform.root.GetComponent<Rigidbody>();
        animator = transform.root.GetComponent<Animator>();
    }

    void Update()
    {
        MouseInput();
        MovementInput();
        AuxInput();

        animator.SetFloat("Speed", Input.GetAxisRaw("Vertical") * rBody.velocity.magnitude);
    }
    void FixedUpdate()
    {
        if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
            rBody.MovePosition(rBody.position + transform.root.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    void MouseInput()
    {
        // Move Camera
        transform.root.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity.x);
        verticalLookRotation += Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity.y;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, verticalLookClamp.x, verticalLookClamp.y);

        vCam.transform.localEulerAngles = Vector3.left * verticalLookRotation;


        // Click to move
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit clickHitInfo, 1000, surfaceMask))
            {
                ABPath walkPath = ABPath.Construct(transform.root.position, clickHitInfo.point);
                seeker.StartPath(walkPath);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            ClearPathing();
        }
    }

    void MovementInput()
    {
        // Sprint
        if (Input.GetButtonDown("Sprint"))
            respiration.ToggleSprinting(true);
        if (Input.GetButtonUp("Sprint"))
            respiration.ToggleSprinting(false);


        // Calculate movement
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        Vector3 targetMoveAmount = Input.GetAxisRaw("Vertical") < 0 ? moveDir * (pathing.maxSpeed) / 2 : moveDir * (pathing.maxSpeed);
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, 0.15f);
    }

    void AuxInput()
    {
        // Eat
        if (Input.GetButtonDown("Eat") && !animator.GetBool("IsEating"))
        {
            animator.SetTrigger("Bite");
        }
        if (Input.GetButtonUp("Eat") && animator.GetBool("IsEating"))
        {
            GetComponentInParent<Metabolism>().StopEating();
            animator.SetBool("IsEating", false);
        }

        // Sing
        if (Input.GetButtonDown("Sing"))
            animator.SetBool("IsSinging", true);
        if (Input.GetButtonUp("Sing"))
            animator.SetBool("IsSinging", false);

        // Jump
        if (Input.GetButtonDown("Jump") && IsGrounded())
            Jump();
    }

    void Jump()
    {
        animator.SetTrigger("Jump");
        rBody.AddForce(GravityVector(transform.root.position) * jumpPower);
    }

    bool IsGrounded()
    {
        Ray ray = new Ray(transform.root.position + (transform.root.up * 0.2f), -transform.root.up);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.4f, surfaceMask))
            return true;
        else return false;
    }


    void ClearPathing()
    {
        transform.root.GetComponent<AIDestinationSetter>().target = null;
        pathing.SetPath(null);
        pathing.destination = Vector3.positiveInfinity;
        seeker.CancelCurrentPathRequest();
    }
}
