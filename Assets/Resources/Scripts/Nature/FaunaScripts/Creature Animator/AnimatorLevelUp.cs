using UnityEngine;

public class AnimatorLevelUp : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnergyData eData = animator.GetComponentInChildren<EnergyData>();
        if (eData.energyReserve > eData.surplusThreshold)
            animator.GetComponentInChildren<CreatureData>().LevelUp();
    }
}
