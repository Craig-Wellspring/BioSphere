using UnityEngine;
using Pathfinding;

public class AIMoveTowardsFood : StateMachineBehaviour
{
    VisualPerception vPerception;
    AIDestinationSetter destinationSetter;
    GameObject targetFood = null;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        vPerception = animator.transform.root.GetComponentInChildren<VisualPerception>();
        destinationSetter = animator.transform.root.GetComponent<AIDestinationSetter>();

        if (vPerception.nearbyFood.Count > 0)
            targetFood = animator.GetComponent<BasicAIBrain>().pickRandomFood ? vPerception.nearbyFood[Random.Range(0, vPerception.nearbyFood.Count)] : vPerception.closestFood;

        destinationSetter.target = targetFood ? targetFood.transform : null;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!destinationSetter.target)
        {
            if (vPerception.nearbyFood.Count > 0)
                targetFood = animator.GetComponent<BasicAIBrain>().pickRandomFood ? vPerception.nearbyFood[Random.Range(0, vPerception.nearbyFood.Count)] : vPerception.closestFood;

            destinationSetter.target = targetFood ? targetFood.transform : null;
        }
    }
}