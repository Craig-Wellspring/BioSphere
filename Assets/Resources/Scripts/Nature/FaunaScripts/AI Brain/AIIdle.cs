using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AIIdle : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RandomPath wanderPath = RandomPath.Construct(animator.rootPosition, animator.GetComponent<BasicAIBrain>().wanderDistance * 1000);
        wanderPath.spread = 5000;
        animator.transform.root.GetComponent<Seeker>().StartPath(wanderPath);
    }
}
