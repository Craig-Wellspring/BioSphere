using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIIdle : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<BasicAIBrain>().sings)
            animator.transform.root.GetComponent<Animator>().SetBool("IsSinging", true);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<BasicAIBrain>().sings)
            animator.transform.root.GetComponent<Animator>().SetBool("IsSinging", false);
    }
}
