using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorBite : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Collider[] hitFoodList = Physics.OverlapSphere(animator.rootPosition + animator.transform.forward * 0.2f + animator.transform.up * 0.2f, 0.4f, LayerMask.GetMask("FoodItem") + LayerMask.GetMask("Corpse") + LayerMask.GetMask("Foliage"));

        foreach (Collider _hitFood in hitFoodList)
        {
            if (animator.GetComponentInChildren<Metabolism>().dietList.Contains(_hitFood.tag))
            {
                animator.GetComponentInChildren<Metabolism>().StartEating(_hitFood.gameObject);
                break;
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Bite");
    }
}
