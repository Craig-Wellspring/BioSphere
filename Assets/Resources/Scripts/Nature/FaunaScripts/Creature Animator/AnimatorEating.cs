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
        if (metabolism.targetEData != null)
            chewRate = metabolism.targetEData.chewRateModifier * metabolism.chewSpeed;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (metabolism.targetEData != null)
            metabolism.Ingest(metabolism.targetEData, chewRate * Time.deltaTime);
    }
}
