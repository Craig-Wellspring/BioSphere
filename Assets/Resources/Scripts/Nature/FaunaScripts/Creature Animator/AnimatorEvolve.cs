using UnityEngine;

public class AnimatorEvolve : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Evolution evolution = animator.GetComponentInChildren<Evolution>();
        if (animator.GetComponentInChildren<EnergyData>().energyReserve > evolution.evolutionCost)
            evolution.Evolve();
    }
}
