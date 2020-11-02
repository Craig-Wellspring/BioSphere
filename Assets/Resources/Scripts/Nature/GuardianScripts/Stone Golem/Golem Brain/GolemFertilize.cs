using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GolemFertilize : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ClearPathing(animator.transform.root);

        animator.GetComponent<LifeGuardian>().PlantSeedFromSource();
        animator.transform.root.GetComponent<Animator>().SetTrigger("PlantSeed");
    }

    
    public void ClearPathing(Transform _transform)
    {
        AIPathAlignedToSurface aiPath = _transform.root.GetComponent<AIPathAlignedToSurface>();

        _transform.root.GetComponent<AIDestinationSetter>().target = null;
        aiPath.SetPath(null);
        aiPath.destination = Vector3.positiveInfinity;
        _transform.root.GetComponent<Seeker>().CancelCurrentPathRequest();
    }
}
