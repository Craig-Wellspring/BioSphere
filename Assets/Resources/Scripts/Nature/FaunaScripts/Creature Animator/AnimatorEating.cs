using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEating : StateMachineBehaviour
{
    Metabolism metabolism;
    string currentTarget;

    float chewRate = 1;

    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        metabolism = animator.GetComponentInChildren<Metabolism>();

        chewRate = metabolism.targetFData.chewRateModifier * metabolism.chewSpeed;

        currentTarget = metabolism.targetFData.name;
        
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Exchange energy
        if (metabolism.targetFData.name == currentTarget)
            metabolism.Ingest(metabolism.targetFData, chewRate * Time.deltaTime);
        else metabolism.StopEating();
    }
}
