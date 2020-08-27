using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorMorph : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Morphology morphology = animator.GetComponentInChildren<Morphology>();
        morphology.TransMorph(morphology.availableMorph);
    }
}
