using UnityEngine;

public class PredatorBrainModule : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackDistance;


    Animator aiBrain;
    VisualPerception vPerception;

    void Start()
    {
        aiBrain = GetComponent<Animator>();
        vPerception = GetComponentInParent<VisualPerception>();
    }

    void Update()
    {
        // Register Prey
        aiBrain.SetInteger("NearbyPrey", vPerception.nearbyPrey.Count);
    }
}
