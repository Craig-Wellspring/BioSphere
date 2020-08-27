using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AIChase : StateMachineBehaviour
{
    AIDestinationSetter destinationSetter;
    VisualPerception vPerception;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        destinationSetter = animator.transform.root.GetComponent<AIDestinationSetter>();
        vPerception = animator.GetComponentInParent<VisualPerception>();
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (vPerception.closestPrey && vPerception.closestPrey.transform != destinationSetter.target)
            destinationSetter.target = vPerception.closestPrey.transform;

        if (!destinationSetter.target)
            destinationSetter.target = null;
    }
    

    override public void OnStateExit(Animator animator1, AnimatorStateInfo stateInfo1, int layerIndex)
    {
        destinationSetter.target = null;
    }
}
