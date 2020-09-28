using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEating : StateMachineBehaviour
{
    Metabolism metabolism;

    float chewRate = 1;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Cache Metabolism
        metabolism = animator.GetComponentInChildren<Metabolism>();

        //Set chewing speed
        if (metabolism.targetNV != null)
            chewRate = metabolism.targetNV.chewRateModifier * metabolism.chewSpeed;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (metabolism.targetNV != null)
            metabolism.Bite(metabolism.targetNV, chewRate * Time.deltaTime);
    }
}
