using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AIIdle : StateMachineBehaviour
{

    float hasBeenIdle = 0f;
    float idleTime = 0f;

    float minIdleTime;
    float maxIdleTime;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        minIdleTime = animator.GetComponent<AIBrainData>().minIdleTime;
        maxIdleTime = animator.GetComponent<AIBrainData>().maxIdleTime;
        idleTime = Random.Range(minIdleTime, maxIdleTime);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasBeenIdle += Time.deltaTime;
        if (hasBeenIdle > idleTime)
        {
            hasBeenIdle = 0f;
            idleTime = Random.Range(minIdleTime, maxIdleTime);

            //Move around if idle for some time, Brownian pattern
            Wander(animator.transform.root);
        }
    }


    void Wander(Transform _originTransform)
    {
        RandomPath wanderPath = RandomPath.Construct(_originTransform.position, _originTransform.GetComponentInChildren<AIBrainData>().wanderDistance);
        wanderPath.spread = 5000;
        //seeker.StartPath(wanderPath);
    }
}
