using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 15;
    private Vector3 moveDir;
    private Rigidbody rbody;

    private void Start()
    {
        rbody = GetComponentInParent<Rigidbody>();
    }

    void Update()
    {
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
    }

    private void FixedUpdate()
    {
        rbody.MovePosition(rbody.position + transform.TransformDirection(moveDir) * moveSpeed * Time.deltaTime);
    }
}
