using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AIBite : StateMachineBehaviour
{
    AIDestinationSetter destinationSetter;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.root.GetComponent<Animator>().SetTrigger("Bite");

        destinationSetter = animator.transform.root.GetComponent<AIDestinationSetter>();
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.root.LookAt(destinationSetter.target, animator.rootPosition);
    }
}
