using UnityEngine;
using Pathfinding;

public class AIMoveTowardsFood : StateMachineBehaviour
{
    VisualPerception vPerception;
    AIDestinationSetter destinationSetter;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        vPerception = animator.GetComponentInParent<VisualPerception>();
        destinationSetter = animator.transform.root.GetComponent<AIDestinationSetter>();

        destinationSetter.target = vPerception.closestFood ? vPerception.closestFood.transform : null;
    }



    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        destinationSetter.target = vPerception.closestFood ? vPerception.closestFood.transform : null;

        if (!destinationSetter.target)
            destinationSetter.target = null;

        if (destinationSetter.target == null)
            animator.Play("Search For Food");
    }
}