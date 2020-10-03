using UnityEngine;

public class AnimatorPlantSeed : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponentInChildren<Ovary>().SpawnSeed(animator.GetComponentInChildren<CreatureData>().levelUpCost);
    }
}
