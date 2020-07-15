using UnityEngine;

public class AIEating : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EatingHitbox hitbox = animator.GetComponentInChildren<EatingHitbox>();

        animator.GetComponent<Metabolism>().StartEating(hitbox.targetFood);
        hitbox.targetFood = null;
        hitbox.gameObject.SetActive(false);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //If still eating when leaving state, stop eating
        if (animator.GetParameter(5).Equals(true))
        {
            animator.GetComponent<Metabolism>().StopEating();
        }
    }
}
