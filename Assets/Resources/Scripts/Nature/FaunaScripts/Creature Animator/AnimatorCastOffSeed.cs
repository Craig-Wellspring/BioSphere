using UnityEngine;

public class AnimatorCastOffSeed : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponentInChildren<Ovary>().SpawnSeed(animator.GetComponentInChildren<Evolution>().evolutionCost);
    }
}
