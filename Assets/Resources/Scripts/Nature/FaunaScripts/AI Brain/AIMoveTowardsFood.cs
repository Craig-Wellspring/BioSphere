using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AIMoveTowardsFood : StateMachineBehaviour
{
    IAstarAI aiPath;
    AIBrainData AIData;
    AIDestinationSetter destinationSetter;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        aiPath = animator.GetComponentInParent<IAstarAI>();
        AIData = animator.GetComponent<AIBrainData>();
        destinationSetter = animator.transform.root.GetComponent<AIDestinationSetter>();

        if (AIData.targetFood == null)
            AIData.targetFood = animator.GetComponent<VisualPerception>().closestFood;
        if (AIData.targetFood != null)
            if (destinationSetter.target != AIData.targetFood.transform)
                destinationSetter.target = AIData.targetFood.transform;

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (AIData.targetFood != null)
        {
            if (destinationSetter.target == AIData.targetFood.transform && (!aiPath.pathPending && (!aiPath.hasPath || aiPath.reachedEndOfPath)))
                animator.SetBool("ProxyFood", true);
            else animator.SetBool("ProxyFood", false);
        }
        else AIData.targetFood = animator.GetComponent<VisualPerception>().closestFood;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
