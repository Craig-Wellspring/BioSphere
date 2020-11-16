using UnityEngine;

public class PredatorBrainModule : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackDistance;
    [SerializeField] bool drawAttackRay = false;


    Animator aiBrain;
    VisualPerception vPerception;

    void Start()
    {
        aiBrain = GetComponent<Animator>();
        vPerception = transform.root.GetComponentInChildren<VisualPerception>();
    }

    void Update()
    {
        // Register Prey
        aiBrain.SetInteger("NearbyPrey", vPerception.nearbyPrey.Count);
    }

    void OnDrawGizmosSelected()
    {
        if (drawAttackRay)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (transform.forward * attackDistance));
        }
    }
}
