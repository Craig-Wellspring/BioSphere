using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorBite : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Metabolism metabolism = animator.GetComponentInChildren<Metabolism>();

        Collider[] hitFoodList = Physics.OverlapSphere(metabolism.mouth.position + (animator.transform.forward * metabolism.biteSize / 2), metabolism.biteSize, LayerMask.GetMask("FoodItem", "Corpse", "Foliage"));

        foreach (Collider _hitFood in hitFoodList)
        {
            if (metabolism.dietList.Contains(_hitFood.tag))
            {
                metabolism.StartEating(_hitFood.gameObject);
                break;
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Bite");
    }
}
