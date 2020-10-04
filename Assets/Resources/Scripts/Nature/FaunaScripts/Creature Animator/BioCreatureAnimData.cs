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
        rBody = GetComponent<Rigidbody>();



        metabolism.EatingBegins += ChangeEatingStatus;
        metabolism.EatingEnds += ChangeEatingStatus;

        vitality.DeathOccurs += Death;
    }

    private void OnDisable()
    {
        metabolism.EatingBegins -= ChangeEatingStatus;
        metabolism.EatingEnds -= ChangeEatingStatus;

        vitality.DeathOccurs -= Death;
    }

    void Update()
    {
        anim.SetFloat("Speed", rBody.velocity.magnitude);
    }



    void ChangeEatingStatus()
    {
        anim.SetBool("IsEating", metabolism.isEating);
    }

    public void TriggerMorph()
    {
        anim.SetTrigger("Morph");
    }

    void Death()
    {
        anim.SetTrigger("Die");
    }
}
