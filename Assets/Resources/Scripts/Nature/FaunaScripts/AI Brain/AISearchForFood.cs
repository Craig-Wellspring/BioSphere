using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AISearchForFood : StateMachineBehaviour
{
    int jauntLength = 10;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Seeker seeker = animator.transform.root.GetComponent<Seeker>();
        LevyWander(animator.rootPosition, jauntLength, seeker);

        RandomPath levyPath = RandomPath.Construct(animator.rootPosition, jauntLength * 1000);
        levyPath.spread = 1000;
        seeker.StartPath(levyPath);
    }


    void LevyWander(Vector3 _originPos, int _jauntLength, Seeker _seeker)
    {
        RandomPath levyPath = RandomPath.Construct(_originPos, _jauntLength * 1000);
        levyPath.spread = 1000;
        _seeker.StartPath(levyPath);
    }
}
