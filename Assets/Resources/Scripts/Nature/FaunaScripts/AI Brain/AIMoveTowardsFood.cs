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

        if (animator.transform.root.GetComponentInChildren<Metabolism>().isWasting)
            animator.transform.root.GetComponentInChildren<Respiration>()?.ToggleSprinting(true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.root.GetComponentInChildren<Respiration>()?.ToggleSprinting(false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!destinationSetter.target)
        {
            if (vPerception.nearbyFood.Count > 0)
                targetFood = animator.GetComponent<BasicAIBrain>().pickRandomFood ? vPerception.nearbyFood[Random.Range(0, vPerception.nearbyFood.Count)] : vPerception.closestFood;

            destinationSetter.target = targetFood ? targetFood.transform : null;
        }

        if (destinationSetter.target)
        {
            Vector3 targetDirection = destinationSetter.target.position - animator.transform.root.position;
            if (targetDirection.magnitude < 4)
            {
                Vector3 newDirection = Vector3.RotateTowards(animator.transform.forward, targetDirection, Time.deltaTime, 0);
                animator.transform.root.rotation = Quaternion.LookRotation(newDirection, -UtilityFunctions.GravityVector(animator.transform.root.position));
            }
        }
    }
}