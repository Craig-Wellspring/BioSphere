﻿using UnityEngine;
using Pathfinding;

public class GolemFindPath : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LifeGuardian lifeGuardian = animator.GetComponent<LifeGuardian>();
        GlobalLifeSource lifeSource = Servius.Server.GetComponent<GlobalLifeSource>();

        RandomPath wanderPath = RandomPath.Construct(animator.rootPosition, lifeGuardian.roamingArea * 1000);
        wanderPath.spread = lifeGuardian.pathingSpread * 1000;
        animator.GetComponentInParent<Seeker>().StartPath(wanderPath);

        if (lifeSource.energyReserve > lifeSource.minimumEnergyReserve)
            animator.SetTrigger("PlantSeed");
    }
}
