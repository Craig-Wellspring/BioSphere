using Pathfinding;
using UnityEngine;

public class AnimationData : MonoBehaviour
{
    Animator anim;
    //Seeker seeker;
    Rigidbody rBody;
    //CreatureAI ai;
    public bool logVelocity = false;
    private float currentVelocity;

    void Start()
    {
        anim = GetComponent<Animator>();
        //seeker = GetComponent<Seeker>();
        rBody = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        currentVelocity = rBody.velocity.magnitude;

        //anim.SetFloat("Velocity", currentVelocity);
        if (logVelocity && currentVelocity != 0)
            Debug.Log(currentVelocity);
        //anim.SetFloat("TurnSpeed", rBody.angularVelocity.magnitude);
    }
}
