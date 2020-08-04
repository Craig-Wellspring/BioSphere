using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorBite : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        Collider[] hitFoodList = Physics.OverlapSphere(animator.rootPosition + animator.transform.forward * 0.2f + animator.transform.up * 0.2f, 0.4f, LayerMask.GetMask("FoodItem") + LayerMask.GetMask("Corpse") + LayerMask.GetMask("Foliage"));

        foreach (Collider _hitFood in hitFoodList)
        {
            if (_hitFood.gameObject == animator.GetComponentInChildren<Metabolism>().currentTargetFood){
                animator.SetBool("IsEating", true);
                animator.GetComponentInChildren<Metabolism>().StartEating(_hitFood.gameObject);

                break;
            }
        }

        // Debug \\
        Debug.Log(hitFoodList.Length);
        GameObject debugSphere = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), animator.rootPosition + animator.transform.forward * 0.2f + animator.transform.up * 0.2f, animator.transform.rotation);
        debugSphere.transform.localScale *= 0.4f;
        Destroy(debugSphere, 10);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        animator.ResetTrigger("Bite");
    }   
}
