using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GolemFertilize : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<LifeGuardian>().PlantSeed();
    }
}
