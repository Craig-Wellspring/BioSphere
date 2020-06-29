using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AIFlee : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Metabolism>().StopEating();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        FleeFromTarget(animator.gameObject, animator.GetComponent<VisualPerception>().closestPredator);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    void FleeFromTarget(GameObject _self, GameObject _fleeTarget)
    {
        FleePath fleePath = FleePath.Construct(_self.transform.root.position, _fleeTarget.transform.position, _self.GetComponent<AIBrainData>().runAwayDistance);
        fleePath.aimStrength = 1;
        fleePath.spread = 4000;
        _self.GetComponentInParent<Seeker>().StartPath(fleePath);
    }
}
