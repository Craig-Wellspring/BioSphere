using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureEating : StateMachineBehaviour
{
    Metabolism metabolism;

    float chewRate = 1;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        metabolism = animator.GetComponentInChildren<Metabolism>();

        chewRate = metabolism.targetFData.chewRateModifier * metabolism.chewSpeed;
        
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Exchange energy
        if (metabolism.targetFData != null)
            metabolism.Ingest(metabolism.targetFData, chewRate * Time.deltaTime);
        else metabolism.StopEating();
    }
}
