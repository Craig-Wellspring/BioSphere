using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AIFlee : StateMachineBehaviour
{
    VisualPerception visualPerception;
    Seeker seeker;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        visualPerception = animator.transform.root.GetComponentInChildren<VisualPerception>();
        seeker = animator.transform.root.GetComponent<Seeker>();

        animator.GetComponentInParent<Metabolism>().StopEating();

        if (visualPerception.closestPredator != null)
            FleeFromTarget(animator.transform.root, visualPerception.closestPredator.transform);

        animator.SetBool("Fleeing", true);
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!visualPerception.closestPredator)
            animator.SetBool("Fleeing", false);
    }

    void FleeFromTarget(Transform _self, Transform _fleeTarget)
    {
        FleePath fleePath = FleePath.Construct(_self.position, _fleeTarget.position, _self.GetComponentInChildren<BasicAIBrain>().runAwayDistance);
        fleePath.aimStrength = 1;
        fleePath.spread = 0;
        seeker.CancelCurrentPathRequest();
        seeker.StartPath(fleePath);
    }
}
