using UnityEngine;
using Pathfinding;

public class AIMoveTowardsFood : StateMachineBehaviour
{
    IAstarAI aiPath;
    //BasicAIBrain BrainData;
    Metabolism metabolism;
    AIDestinationSetter destinationSetter;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        aiPath = animator.transform.root.GetComponent<IAstarAI>();
        metabolism = animator.GetComponent<Metabolism>();
        destinationSetter = animator.transform.root.GetComponent<AIDestinationSetter>();

        if (metabolism.currentTargetFood == null)
        {
            metabolism.currentTargetFood = animator.GetComponent<VisualPerception>().closestFood;
            destinationSetter.target = metabolism.currentTargetFood.transform;
        }

    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (metabolism.currentTargetFood != null)
        {
            if (destinationSetter.target == metabolism.currentTargetFood.transform && (!aiPath.pathPending && (!aiPath.hasPath || aiPath.reachedEndOfPath)))
                animator.SetBool("ProxyFood", true);
            else animator.SetBool("ProxyFood", false);
        }
        else metabolism.currentTargetFood = animator.GetComponent<VisualPerception>().closestFood;
    }
}
