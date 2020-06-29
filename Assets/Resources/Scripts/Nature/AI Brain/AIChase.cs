using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AIChase : StateMachineBehaviour
{
    AIDestinationSetter destinationSetter;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        destinationSetter = animator.transform.root.GetComponent<AIDestinationSetter>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (destinationSetter.target != animator.GetComponent<VisualPerception>().closestPrey.transform)
            destinationSetter.target = animator.GetComponent<VisualPerception>().closestPrey.transform;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
