using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class CreatureData : MonoBehaviour
{
    [Header("Body Parts")]
    [Tooltip("GameObjects that contain the Creature's body colliders")]
    public List<Collider> bodyColliders;
    [Tooltip("GameObject that represents the Creature's corpse")]
    [Space(10)]
    public GameObject corpse;


    AIDestinationSetter destinationSetter;
    AIPath aiPath;
    Seeker seeker;

    void Start()
    {
        destinationSetter = transform.root.GetComponent<AIDestinationSetter>();
        aiPath = transform.root.GetComponent<AIPathAlignedToSurface>();
        seeker = transform.root.GetComponent<Seeker>();
    }


    
    public void ClearPathing()
    {
        destinationSetter.target = null;
        aiPath.SetPath(null);
        aiPath.destination = Vector3.positiveInfinity;
        seeker.CancelCurrentPathRequest();
    }
}
