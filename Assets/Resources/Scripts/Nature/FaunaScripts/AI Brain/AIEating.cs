using UnityEngine;

public class AIEating : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        //If still eating when leaving state, stop eating
        if (animator.GetBool("Eating")){
            animator.GetComponentInParent<Metabolism>().StopEating();
        }
    }
}
