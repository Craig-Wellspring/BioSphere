using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System.Collections.Generic;
using Pathfinding;

public class CreatureAI : VersionedMonoBehaviour
{

    #region Settings
    [Header("Energy Usage Settings")]
    public float morphThreshold = 120f;
    public float layEggThreshold = 100f;
    public float energyEndowed = 30f;
    public float aloneThreshold = 30f;
    [Header("Flee Settings")]
    [Tooltip("1000 ~= 1 meter")]
    public int runAwayDistance = 10000;
    [Header("Wander Settings")]
    public float minIdleTime = 1f;
    public float maxIdleTime = 10f;
    [SerializeField] private float remainingIdleTime;
    //public float wanderArea = 10f;
    //public float wanderSpeed = 1f;
    [Space(10)]
    [Tooltip("1000 = 1 meter")]
    public float minWanderDistance = 10f;
    public float maxWanderDistance = 100f;
    #endregion


    #region Internal Variables
    private float hasBeenIdle = 0f;
    private float idleTime = 0f;
    private float wanderDistance;
    private float timeSiblingLastSeen;

    public GameObject targetFood;

    private Metabolism metabolism;
    private Ovary ovary;
    private VisualPerception vPerception;
    private CreatureData cData;
    private Morphology morphology;

    private Animator AIBrain;
    private Animator animator;
    private AIDestinationSetter destinationSetter;
    private Seeker seeker;
    IAstarAI aiPath;


    #endregion

    void OnEnable()
    {
        aiPath = GetComponentInParent<IAstarAI>();
        if (aiPath != null) aiPath.onSearchPath += Update;
    }
    void OnDisable()
    {
        if (aiPath != null) aiPath.onSearchPath -= Update;
    }

    void Start()
    {
        metabolism = GetComponent<Metabolism>();
        ovary = GetComponent<Ovary>();
        vPerception = GetComponent<VisualPerception>();
        cData = GetComponent<CreatureData>();
        morphology = GetComponent<Morphology>();

        AIBrain = GetComponent<Animator>();
        animator = GetComponentInParent<Animator>();
        destinationSetter = GetComponentInParent<AIDestinationSetter>();
        seeker = GetComponentInParent<Seeker>();


        //Initialize
        idleTime = Random.Range(minIdleTime, maxIdleTime);
        wanderDistance = Random.Range(minWanderDistance, maxWanderDistance);

    }

    void Update()
    {
        // Remember when last Sibling seen
        if (vPerception.nearbySiblings.Count > 0)
            timeSiblingLastSeen = Time.time;

        // 1) Run from Predator if present
        if (vPerception.nearbyPredators.Count > 0)
        {
            if (metabolism.isEating)
                metabolism.StopEating();
            GameObject nearestPredator = GetClosestTarget(vPerception.nearbyPredators);
            FleeFromTarget(nearestPredator);
        }

        // 2) Chase target Prey
        if (vPerception.nearbyPredators.Count == 0 && vPerception.nearbyPrey.Count > 0)
        {
            if (!metabolism.isEating)
            {
                GameObject nearestPrey = GetClosestTarget(vPerception.nearbyPrey);
                if (destinationSetter.target != nearestPrey.transform)
                    SeekTarget(nearestPrey);
            }
        }

        // 3) Find Food if hungry and no threats present
        if (vPerception.nearbyPredators.Count == 0 && vPerception.nearbyPrey.Count == 0 && vPerception.nearbyFood.Count > 0)
        {
            if (metabolism.IsHungry() && !metabolism.isEating)
            {
                if (targetFood == null)
                    targetFood = GetClosestTarget(vPerception.nearbyFood);

                if (destinationSetter.target != targetFood.transform.root)
                    SeekTarget(targetFood);

                if (destinationSetter.target == targetFood.transform.root && (!aiPath.pathPending && (!aiPath.hasPath || aiPath.reachedEndOfPath)))
                    AIBrain.SetTrigger("TryToEat");
            }
        }


        // 4) If nothing is in range
        if (!metabolism.isEating)
        {
            // a) Lay egg if over threshold
            if (cData.energyUnits >= layEggThreshold)
                if (Time.time - timeSiblingLastSeen > aloneThreshold)
                    ovary.SpawnEgg(energyEndowed);

            // b) Morph into next form if over threshold
            if (cData.energyUnits >= morphThreshold)
                morphology.Evolve();


            // c) Wander if nothing else to do
            if (!aiPath.pathPending && (!aiPath.hasPath || aiPath.reachedEndOfPath))
            {
                hasBeenIdle += Time.deltaTime;
                remainingIdleTime = idleTime - hasBeenIdle;
                if (hasBeenIdle > idleTime)
                {
                    hasBeenIdle = 0f;
                    idleTime = Random.Range(minIdleTime, maxIdleTime);
                    //Move around if idle for some time
                    Wander();
                }
            }
        }


        //Update Animator
        //animator.SetFloat("MoveSpeed", ai.velocity.magnitude);
    }



    #region Movement

    void SeekTarget(GameObject _seekTarget)
    {
        destinationSetter.target = _seekTarget.transform.root;
    }

    void FleeFromTarget(GameObject _fleeTarget)
    {
        FleePath fleePath = FleePath.Construct(transform.position, _fleeTarget.transform.position, runAwayDistance);
        fleePath.aimStrength = 1;
        fleePath.spread = 4000;
        seeker.StartPath(fleePath);
    }

    void Wander()
    {
        RandomPath wanderPath = RandomPath.Construct(transform.position, Mathf.RoundToInt(wanderDistance * metabolism.hungerPercentage));
        wanderPath.spread = 5000;
        //seeker.StartPath(wanderPath);

        wanderDistance = Random.Range(minWanderDistance, maxWanderDistance);
    }

    public void ClearPathing()
    {
        destinationSetter.target = null;
        aiPath.SetPath(null);
        aiPath.destination = Vector3.positiveInfinity;
        seeker.CancelCurrentPathRequest();
    }

    void StopEverything()
    {
        metabolism.StopEating();
        //ClearPathing();
    }
    #endregion





    #region Utility Functions
    public GameObject GetClosestTarget(List<Collider> _targets)
    {
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Collider potentialTarget in _targets)
        {
            if (potentialTarget != null)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr && potentialTarget.gameObject != this.gameObject)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget.gameObject;
                }
            }
        }
        return bestTarget;
    }
    #endregion
}
