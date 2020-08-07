using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AIIdle : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.root.GetComponent<Animator>().SetBool("IsSinging", true);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.root.GetComponent<Animator>().SetBool("IsSinging", false);
    }
}
