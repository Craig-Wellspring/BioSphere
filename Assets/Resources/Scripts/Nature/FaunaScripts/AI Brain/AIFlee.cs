using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AIFlee : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponentInParent<Metabolism>().StopEating();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        FleeFromTarget(animator.transform.root, animator.GetComponentInParent<VisualPerception>().closestPredator.transform);
    }

    void FleeFromTarget(Transform _self, Transform _fleeTarget)
    {
        FleePath fleePath = FleePath.Construct(_self.position, _fleeTarget.position, _self.GetComponentInChildren<BasicAIBrain>().runAwayDistance);
        fleePath.aimStrength = 1;
        fleePath.spread = 4000;
        _self.GetComponent<Seeker>().StartPath(fleePath);
    }
}
