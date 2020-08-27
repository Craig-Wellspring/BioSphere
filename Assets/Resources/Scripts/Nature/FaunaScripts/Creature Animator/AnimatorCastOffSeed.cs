using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorCastOffSeed : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponentInChildren<Evolution>().CastOffSeed();
    }
}
