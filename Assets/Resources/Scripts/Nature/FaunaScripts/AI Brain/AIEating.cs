using UnityEngine;

public class AIEating : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EatingHitbox hitbox = animator.transform.root.GetComponentInChildren<EatingHitbox>();

        animator.GetComponentInParent<Metabolism>().StartEating(hitbox.hitFood);
        hitbox.hitFood = null;
        animator.SetBool("ProxyFood", false);
        hitbox.gameObject.SetActive(false);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //If still eating when leaving state, stop eating
        if (animator.GetBool("Eating"))
        {
            animator.GetComponentInParent<Metabolism>().StopEating();
        }
    }
}
