using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System.Collections.Generic;

namespace Pathfinding
{
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

        private GameObject targetFood;

        private Metabolism metabolism;
        private Ovary ovary;
        private VisualPerception vPerception;
        private CreatureData cData;
        private Morphology morphology;

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
                FleeFromTarget(GetClosestTarget(vPerception.nearbyPredators).gameObject);
            }

            // 2) Chase target Prey
            if (vPerception.nearbyPredators.Count == 0 && vPerception.nearbyPrey.Count > 0)
            {
                if (!metabolism.isEating)
                    SeekTarget(GetClosestTarget(vPerception.nearbyPrey).gameObject);
            }

            // 3) Find Food if hungry and no threats present
            if (vPerception.nearbyPredators.Count == 0 && vPerception.nearbyPrey.Count == 0 && vPerception.nearbyFood.Count > 0)
            {
                if (metabolism.IsHungry() && !metabolism.isEating)
                {
                    targetFood = GetClosestTarget(vPerception.nearbyFood);
                    if (targetFood != null)
                    {
                        if (destinationSetter.target != targetFood.transform)
                        {
                            SeekTarget(targetFood);
                            return;
                        }
                        else
                            if (!aiPath.pathPending && (!aiPath.hasPath || aiPath.reachedEndOfPath))
                                metabolism.StartEating(targetFood);
                    }
                }
            }


            // 4) If nothing is in range
            if (!metabolism.isEating)
            {
                //Lay egg if over threshold
                if (cData.energyUnits >= layEggThreshold)
                    if (Time.time - timeSiblingLastSeen > aloneThreshold)
                        ovary.SpawnEgg(energyEndowed);

                if (cData.energyUnits >= morphThreshold)
                    morphology.Evolve();
                    
                
                //Wander if nothing else to do
                if (!aiPath.pathPending && (!aiPath.hasPath || aiPath.reachedEndOfPath))
                {
                    hasBeenIdle += Time.deltaTime;
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
            destinationSetter.target = _seekTarget.transform;
        }

        void FleeFromTarget(GameObject _fleeTarget)
        {
            ClearPathing();

            FleePath fleePath = FleePath.Construct(transform.position, _fleeTarget.transform.position, runAwayDistance);
            fleePath.aimStrength = 1;
            fleePath.spread = 4000;
            seeker.StartPath(fleePath);
        }

        void Wander()
        {
            ClearPathing();

            RandomPath wanderPath = RandomPath.Construct(transform.position, Mathf.RoundToInt(wanderDistance * metabolism.hungerPercentage));
            wanderPath.spread = 5000;
            seeker.StartPath(wanderPath);

            wanderDistance = Random.Range(minWanderDistance, maxWanderDistance);
        }

        void ClearPathing()
        {
            destinationSetter.target = null;
            aiPath.SetPath(null);
            aiPath.destination = Vector3.positiveInfinity;
        }

        void StopEverything()
        {
            metabolism.StopEating();
            ClearPathing();
        }
        #endregion





        #region Utility Functions
        GameObject GetClosestTarget(List<Collider> _targets)
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
}
