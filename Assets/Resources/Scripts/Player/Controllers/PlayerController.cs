using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector2 mouseSensitivity = new Vector2(250f, 250f);
    public Vector2 verticalLookClamp = new Vector2(-60, 60);

    Transform mainCam;
    Animator animator;
    Metabolism metabolism;
    float verticalLookRotation;

    Vector3 moveAmount;

    private void Start()
    {
        mainCam = GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().transform;
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
        mainCam.localEulerAngles = Vector3.left * verticalLookRotation;
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
    }
}
