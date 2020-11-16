using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AIChase : StateMachineBehaviour
{
    AIDestinationSetter destinationSetter;
    VisualPerception vPerception;
    PredatorBrainModule predatorBrain;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        destinationSetter = animator.transform.root.GetComponent<AIDestinationSetter>();
        vPerception = animator.transform.root.GetComponentInChildren<VisualPerception>();
        predatorBrain = animator.GetComponent<PredatorBrainModule>();
        
        animator.transform.root.GetComponentInChildren<Respiration>().ToggleSprinting(true);
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (vPerception.closestPrey && vPerception.closestPrey.transform != destinationSetter.target)
            destinationSetter.target = vPerception.closestPrey.transform;

        if (!destinationSetter.target)
            destinationSetter.target = null;

        if (animator.GetFloat("TargetDistance") - predatorBrain.attackDistance <= 0)
            animator.SetBool("TargetInAttackRange", true);
        else animator.SetBool("TargetInAttackRange", false);
    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        destinationSetter.target = null;
            
        animator.transform.root.GetComponentInChildren<Respiration>().ToggleSprinting(false);
    }
}
