using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GolemFertilize : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<LifeGuardian>().PlantSeedFromSource();
        animator.transform.root.GetComponent<Animator>().SetTrigger("PlantSeed");
    }
}
