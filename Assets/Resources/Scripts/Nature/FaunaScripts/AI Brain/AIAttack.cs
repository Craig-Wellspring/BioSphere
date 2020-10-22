using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AIAttack : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.root.LookAt(animator.transform.root.GetComponent<AIDestinationSetter>().target, animator.transform.root.up);
        PlanetCore.Core.AlignWithGravity(animator.transform.root, true);
        animator.transform.root.GetComponent<Animator>().SetTrigger("Attack1");
    }
}
