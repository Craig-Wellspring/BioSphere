using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AIBite : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.root.GetComponent<Animator>().SetTrigger("Bite");

        // Look at target
        //animator.transform.root.LookAt(animator.transform.root.GetComponent<AIDestinationSetter>().target, animator.rootPosition);
        //PlanetCore.Core.AlignWithGravity(animator.transform.root, true);
    }
}
