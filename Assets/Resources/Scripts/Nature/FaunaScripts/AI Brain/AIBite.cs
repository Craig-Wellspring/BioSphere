using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBite : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        animator.transform.root.GetComponent<Animator>().SetTrigger("Bite");
    }
}
