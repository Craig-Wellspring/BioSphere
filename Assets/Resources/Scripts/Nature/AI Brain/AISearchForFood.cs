using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AISearchForFood : StateMachineBehaviour
{
    int jauntLength = 10;
    IAstarAI aiPath;
    Seeker seeker;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        aiPath = animator.GetComponentInParent<IAstarAI>();
        seeker = animator.GetComponentInParent<Seeker>();
        LevyWander(animator.transform.root, jauntLength, seeker);
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LevyWander(animator.transform.root, jauntLength, seeker);

        /*
        if (!aiPath.pathPending && !aiPath.hasPath)
        {
            LevyWander(animator.transform.root, jauntLength, seeker);
            jauntLength *= 2;
            if (jauntLength > 50000)
                jauntLength = 50000;
        }*/

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }


    void LevyWander(Transform _originTransform, int _jauntLength, Seeker _seeker)
    {
        RandomPath wanderPath = RandomPath.Construct(_originTransform.position, _jauntLength);
        //wanderPath.spread = 5000;
        //_seeker.StartPath(wanderPath);
    }
}
