using UnityEngine;
using Pathfinding;

public class AIMoveTowardsFood : StateMachineBehaviour
{
    Metabolism metabolism;
    VisualPerception vPerception;
    AIDestinationSetter destinationSetter;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        metabolism = animator.GetComponentInParent<Metabolism>();
        vPerception = animator.GetComponentInParent<VisualPerception>();
        destinationSetter = animator.transform.root.GetComponent<AIDestinationSetter>();

        if (!metabolism.currentTargetFood && !destinationSetter.target)
            SearchForTarget();
    }



    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!destinationSetter.target)
            destinationSetter.target = null;

        if (!metabolism.currentTargetFood)
            metabolism.currentTargetFood = null;

        if (!metabolism.currentTargetFood && !destinationSetter.target)
            SearchForTarget();
    }

    void SearchForTarget()
    {
        if (vPerception.closestFood)
        {
            metabolism.currentTargetFood = vPerception.closestFood;
            destinationSetter.target = metabolism.currentTargetFood.transform;
        }
    }
}