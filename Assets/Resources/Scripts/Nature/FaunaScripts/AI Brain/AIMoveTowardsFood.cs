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
            animator.transform.Find("Eating Hitbox").gameObject.SetActive(true);
        }
    }
}