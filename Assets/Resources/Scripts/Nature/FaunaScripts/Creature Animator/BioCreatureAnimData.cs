using Pathfinding;
using UnityEngine;

public class BioCreatureAnimData : MonoBehaviour
{
    Animator anim;
    Metabolism metabolism;
    Vitality vitality;

    void Start()
    {
        anim = GetComponent<Animator>();
        metabolism = GetComponentInChildren<Metabolism>();
        vitality = GetComponentInChildren<Vitality>();



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
