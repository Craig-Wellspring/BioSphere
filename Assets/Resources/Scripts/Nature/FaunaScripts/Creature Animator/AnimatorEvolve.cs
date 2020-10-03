using UnityEngine;

public class AnimatorEvolve : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CreatureData cData = animator.GetComponentInChildren<CreatureData>();
        if (animator.GetComponentInChildren<EnergyData>().energyReserve > cData.levelUpCost)
            cData.LevelUp();
    }
}
