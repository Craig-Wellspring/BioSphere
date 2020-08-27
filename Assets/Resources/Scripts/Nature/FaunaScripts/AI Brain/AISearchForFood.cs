using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AISearchForFood : StateMachineBehaviour
{
    int jauntLength = 10;
    Seeker seeker;
    AIDestinationSetter destinationSetter;
    Metabolism metabolism;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        seeker = animator.transform.root.GetComponent<Seeker>();
        destinationSetter = animator.transform.root.GetComponent<AIDestinationSetter>();
        metabolism = animator.GetComponentInParent<Metabolism>();

        FindRandomPath(animator.rootPosition, jauntLength);
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!destinationSetter.target)
            destinationSetter.target = null;

        if (!metabolism.currentTargetFood)
            metabolism.currentTargetFood = null;
    }

    void FindRandomPath(Vector3 _fromPos, int _jauntLength)
    {
        RandomPath levyPath = RandomPath.Construct(_fromPos, _jauntLength * 1000);
        levyPath.spread = 1000;
        seeker.StartPath(levyPath);
    }
}
