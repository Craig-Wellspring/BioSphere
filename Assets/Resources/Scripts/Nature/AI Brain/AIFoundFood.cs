using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFoundFood : StateMachineBehaviour
{
    CreatureAI cAI;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        cAI = animator.gameObject.GetComponent<CreatureAI>();
        if (cAI.targetFood == null)
            cAI.targetFood = cAI.GetClosestTarget(animator.gameObject.GetComponent<VisualPerception>().nearbyFood);
        /*
        if (destinationSetter.target != cAI.targetFood.transform.root)
            SeekTarget(cAI.targetFood);

        if (destinationSetter.target == cAI.targetFood.transform.root && (!aiPath.pathPending && (!aiPath.hasPath || aiPath.reachedEndOfPath)))
            AIBrain.SetTrigger("TryToEat");
       */
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
