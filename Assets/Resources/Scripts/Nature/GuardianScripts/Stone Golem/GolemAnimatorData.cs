using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemAnimatorData : MonoBehaviour
{
    Animator golemAnim;
    Rigidbody rBody;

    private void Start()
    {
        golemAnim = GetComponent<Animator>();
        rBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        golemAnim.SetFloat("Speed", rBody.velocity.magnitude);
    }
}
