using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AISearchForFood : StateMachineBehaviour
{
    int jauntLength = 10;

    Seeker seeker;
    AIDestinationSetter destinationSetter;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        seeker = animator.transform.root.GetComponent<Seeker>();
        destinationSetter = animator.transform.root.GetComponent<AIDestinationSetter>();

        FindRandomPath(animator.rootPosition, jauntLength);
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!destinationSetter.target)
            destinationSetter.target = null;
    }

    void FindRandomPath(Vector3 _fromPos, int _jauntLength)
    {
        RandomPath levyPath = RandomPath.Construct(_fromPos, _jauntLength * 1000);

        seeker.CancelCurrentPathRequest();
        seeker.StartPath(levyPath);
    }
}
