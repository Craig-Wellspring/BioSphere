using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorLayEgg : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponentInChildren<Ovary>().SpawnEgg(animator.GetComponentInChildren<EnergyData>().surplusThreshold);
    }
}
