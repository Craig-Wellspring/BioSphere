using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AIBite : StateMachineBehaviour
{
    AIDestinationSetter destinationSetter;
    Metabolism metabolism;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.root.GetComponent<Animator>().SetTrigger("Bite");

        destinationSetter = animator.transform.root.GetComponent<AIDestinationSetter>();
        metabolism = animator.GetComponentInParent<Metabolism>();

        /*if (metabolism.currentTargetFood)
        {
            if ((animator.rootPosition - metabolism.currentTargetFood.transform.position).magnitude < 0.25f)
            {
                metabolism.StartEating(metabolism.currentTargetFood);
            }
        }*/
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.root.LookAt(destinationSetter.target, animator.rootPosition);
    }
}
