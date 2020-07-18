using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GuardianRoam : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Wander(animator.transform.root);
    }

    void Wander(Transform _originTransform)
    {
        RandomPath wanderPath = RandomPath.Construct(_originTransform.position, 10);
        wanderPath.spread = 5000;
        //seeker.StartPath(wanderPath);

        Debug.Log("Wander path: " + wanderPath.startPoint + " to " + wanderPath.endPoint);
    }
}
