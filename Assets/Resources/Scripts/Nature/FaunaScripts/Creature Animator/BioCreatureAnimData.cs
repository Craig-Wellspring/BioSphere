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



        metabolism.EatingBegins += BeginEating;
        metabolism.EatingEnds += CeaseEating;

        vitality.DeathOccurs += Death;
    }

    private void OnDisable()
    {
        metabolism.EatingBegins -= BeginEating;
        metabolism.EatingEnds -= CeaseEating;

        vitality.DeathOccurs -= Death;
    }

    void Update()
    {
        anim.SetFloat("Speed", rBody.velocity.magnitude);
    }



    void BeginEating()
    {
        anim.SetBool("IsEating", true);
    }
    void CeaseEating()
    {
        anim.SetBool("IsEating", false);
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
