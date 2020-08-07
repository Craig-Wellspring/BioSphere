using UnityEngine;
using Pathfinding;

public class GolemRoam : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RandomPath wanderPath = RandomPath.Construct(animator.rootPosition, animator.GetComponent<LifeGuardian>().roamingArea * 1000);
        wanderPath.spread = animator.GetComponent<LifeGuardian>().pathingSpread;
        animator.GetComponentInParent<Seeker>().StartPath(wanderPath);
    }
}
