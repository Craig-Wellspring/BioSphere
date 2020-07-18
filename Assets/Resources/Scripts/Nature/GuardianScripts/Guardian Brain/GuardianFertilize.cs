using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GuardianFertilize : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<FertilizePlanet>().PlantSeed();
    }
}
