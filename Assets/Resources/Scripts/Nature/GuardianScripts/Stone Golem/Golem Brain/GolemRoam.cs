using UnityEngine;
using Pathfinding;

public class GolemRoam : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RandomPath wanderPath = RandomPath.Construct(animator.rootPosition, animator.GetComponent<MeteorGuardian>().roamingArea * 1000);
        wanderPath.spread = 5000;
        animator.GetComponentInParent<Seeker>().StartPath(wanderPath);
    }
}
