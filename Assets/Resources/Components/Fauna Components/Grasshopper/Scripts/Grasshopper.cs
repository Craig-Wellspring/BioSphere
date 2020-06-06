using UnityEngine;
using System.Collections;

public class Grasshopper : MonoBehaviour {
    Animator grasshopper;
	// Use this for initialization
	void Start () {
        grasshopper = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.S))
        {
            grasshopper.SetBool("idle", true);
            grasshopper.SetBool("singing", false);
            grasshopper.SetBool("jump", false);
            grasshopper.SetBool("walk", false);
            grasshopper.SetBool("turnleft", false);
            grasshopper.SetBool("turnright", false);
            grasshopper.SetBool("eat", false);
        }
        if (Input.GetKey(KeyCode.R))
        {
            grasshopper.SetBool("singing", true);
            grasshopper.SetBool("idle", false);
        }
        if (Input.GetKey(KeyCode.E))
        {
            grasshopper.SetBool("eat", true);
            grasshopper.SetBool("idle", false);
            grasshopper.SetBool("singing", false);
            grasshopper.SetBool("walk", false);
        }
        if (Input.GetKey(KeyCode.W))
        {
            grasshopper.SetBool("walk", true);
            grasshopper.SetBool("turnleft", false);
            grasshopper.SetBool("turnright", false);
            grasshopper.SetBool("jump", false);
            grasshopper.SetBool("idle", false);
            grasshopper.SetBool("jump", false);
            grasshopper.SetBool("eat", false);

        }
        if (Input.GetKey(KeyCode.A))
        {
            grasshopper.SetBool("turnleft", true);
            grasshopper.SetBool("turnright", false);
            grasshopper.SetBool("walk", false);
            grasshopper.SetBool("idle", false);
        }
        if (Input.GetKey(KeyCode.D))
        {
            grasshopper.SetBool("turnright", true);
            grasshopper.SetBool("turnleft", false);
            grasshopper.SetBool("walk", false);
            grasshopper.SetBool("idle", false);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            grasshopper.SetBool("jump", true);
            grasshopper.SetBool("walk", false);
            grasshopper.SetBool("idle", false);
        }
        if (Input.GetKey(KeyCode.F))
        {
            grasshopper.SetBool("die", true);
            grasshopper.SetBool("jump", false);
            grasshopper.SetBool("walk", false);
        }
    }
}
