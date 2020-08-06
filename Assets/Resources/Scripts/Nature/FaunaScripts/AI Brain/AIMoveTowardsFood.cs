using UnityEngine;
using Pathfinding;

public class AIMoveTowardsFood : StateMachineBehaviour
{
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){

        Metabolism metabolism = animator.GetComponentInParent<Metabolism>();

        if (metabolism.currentTargetFood == null){
            metabolism.currentTargetFood = animator.GetComponentInParent<VisualPerception>().closestFood;
            if (metabolism.currentTargetFood != null){
                animator.transform.root.GetComponent<AIDestinationSetter>().target = metabolism.currentTargetFood.transform;
            }
        }
    }
}