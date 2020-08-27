using UnityEngine;
using Pathfinding;

public class GolemFindPath : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RandomPath wanderPath = RandomPath.Construct(animator.rootPosition, animator.GetComponent<LifeGuardian>().roamingArea * 1000);
        wanderPath.spread = animator.GetComponent<LifeGuardian>().pathingSpread;
        //int searchGraphMask = (1 << 0) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5) | (1 << 6) | (1 << 7) | (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11) | (1 << 12) | (1 << 13) | (1 << 14) | (1 << 15) | (1 << 16) | (1 << 17) | (1 << 18) | (1 << 19) | (1 << 20) | (1 << 21) | (1 << 22) | (1 << 23);
        animator.GetComponentInParent<Seeker>().StartPath(wanderPath);//.startPoint, wanderPath.endPoint, null, searchGraphMask);

        if (Servius.Server.GetComponent<GlobalLifeSource>().lifeEnergyPool > animator.GetComponent<LifeGuardian>().minimumGlobalEnergy + 1)
        {
            animator.SetTrigger("PlantSeed");
        }
    }
}
