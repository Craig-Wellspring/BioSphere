using UnityEngine;

public class AILevelUp : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.root.GetComponent<Animator>().SetTrigger("LevelUp");
    }
}
