using Pathfinding;
using UnityEngine;

public class BioCreatureAnimData : MonoBehaviour
{
    Animator anim;
    Metabolism metabolism;
    Vitality vitality;
    Rigidbody rBody;

    void Start()
    {
        anim = GetComponent<Animator>();
        metabolism = GetComponentInChildren<Metabolism>();
        vitality = GetComponentInChildren<Vitality>();
        rBody = transform.root.GetComponent<Rigidbody>();


        metabolism.EatingBegins += ChangeEatingStatus;
        metabolism.EatingEnds += ChangeEatingStatus;

        vitality.DeathOccurs += Death;
    }

    void OnDisable()
    {
        metabolism.EatingBegins -= ChangeEatingStatus;
        metabolism.EatingEnds -= ChangeEatingStatus;

        vitality.DeathOccurs -= Death;
    }

    void Update()
    {
        // Register Speed
        anim.SetFloat("Speed", rBody.velocity.magnitude);
    }


    public void AttemptBite()
    {
        metabolism.Bite();
    }

    void ChangeEatingStatus()
    {
        anim.SetBool("IsEating", metabolism.isEating);
    }

    void Death()
    {
        anim.SetTrigger("Die");
    }

}
