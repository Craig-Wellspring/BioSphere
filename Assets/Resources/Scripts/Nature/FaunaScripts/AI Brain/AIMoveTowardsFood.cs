using UnityEngine;
using Pathfinding;

public class AIMoveTowardsFood : StateMachineBehaviour
{
    IAstarAI aiPath;
    Metabolism metabolism;
    AIDestinationSetter destinationSetter;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        aiPath = animator.transform.root.GetComponent<IAstarAI>();
        destinationSetter = animator.transform.root.GetComponent<AIDestinationSetter>();
        metabolism = animator.GetComponentInParent<Metabolism>();

        if (metabolism.currentTargetFood == null)
        {
            metabolism.currentTargetFood = animator.GetComponentInParent<VisualPerception>().closestFood;
            destinationSetter.target = metabolism.currentTargetFood.transform;
            //animator.transform.root.GetComponentInChildren<EatingHitbox>(true).gameObject.SetActive(true);
        }
    }
}