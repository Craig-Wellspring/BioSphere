using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PlayerController : AdvancedMonoBehaviour
{
    public Vector2 mouseSensitivity = new Vector2(250f, 250f);
    public Vector2 verticalLookClamp = new Vector2(-60, 60);
    public int jumpPower = 10;

    #region Internal Variables
    Cinemachine.CinemachineVirtualCamera vCam;
    Camera mainCam;
    
    AIDestinationSetter destinationSetter;
    Seeker seeker;

    Animator animator;
    Metabolism metabolism;
    float verticalLookRotation;
    LayerMask clickToMoveMask;

    //Vector3 moveAmount;
    #endregion

    private void Start()
    {
        clickToMoveMask = LayerMask.NameToLayer("Ground");
        vCam = GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
        mainCam = PlayerSoul.Cam.GetComponent<Camera>();
        destinationSetter = transform.root.GetComponent<AIDestinationSetter>();
        seeker = transform.root.GetComponent<Seeker>();

        animator = transform.root.GetComponent<Animator>();
        metabolism = GetComponentInParent<Metabolism>();
    }

    void Update()
    {
        MouseInput();
        KeyboardInput();
    }

    private void MouseInput()
    {
        transform.root.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity.x);
        verticalLookRotation += Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity.y;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, verticalLookClamp.x, verticalLookClamp.y);
        vCam.transform.localEulerAngles = Vector3.left * verticalLookRotation;

        // Click to move
        if (Input.GetMouseButtonDown(0))
        {
            Ray clickRay = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit clickHitInfo;

            if (Physics.Raycast(clickRay, out clickHitInfo, 1000, clickToMoveMask))
            {
                ABPath walkPath = ABPath.Construct(transform.root.position, clickHitInfo.point);
                seeker.StartPath(walkPath);
            }
        }
    }

    private void KeyboardInput()
    {
        // Eat
        if (Input.GetKeyDown(KeyCode.E) && !animator.GetBool("IsEating"))
        {
            animator.SetTrigger("Bite");
        }
        if (Input.GetKeyUp(KeyCode.E) && animator.GetBool("IsEating"))
        {
            metabolism.StopEating();
            animator.SetBool("IsEating", false);
        }

        // Sing
        if (Input.GetKeyDown(KeyCode.C))
        {
            animator.SetBool("IsSinging", true);
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            animator.SetBool("IsSinging", false);
        }

        // Jump

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void Jump()
    {
        animator.SetTrigger("Jump");
        transform.root.GetComponent<Rigidbody>().AddForce(GravityVector(transform.root.position) * jumpPower);
    }
}
