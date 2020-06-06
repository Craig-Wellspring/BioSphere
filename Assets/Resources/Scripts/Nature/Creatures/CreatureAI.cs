using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System.Collections.Generic;

namespace Pathfinding
{
    public class CreatureAI : VersionedMonoBehaviour
    {

        #region Settings
        [Header("Flee Settings")]
        [Tooltip("1000 ~= 1 meter")]
        public int runAwayDistance = 10000;
        [Header("Wander Settings")]
        public float minIdleTime = 1f;
        public float maxIdleTime = 10f;
        public float wanderArea = 10f;
        public float wanderSpeed = 1f;
        #endregion
        

        #region Internal Variables
        private float hasBeenIdle = 0f;
        private float idleTime = 0f;
        private GameObject targetFood;
        private Animator animator;
        private Metabolism metabolism;
        private VisualPerception vPerception;

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
            destinationSetter = GetComponentInParent<AIDestinationSetter>();
            seeker = GetComponentInParent<Seeker>();
            animator = GetComponentInParent<Animator>();
            metabolism = GetComponent<Metabolism>();
            vPerception = GetComponent<VisualPerception>();

            
            //Initialize
            idleTime = Random.Range(minIdleTime, maxIdleTime);
        }

        void Update()
        {
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
                    targetFood = GetClosestTarget(vPerception.nearbyFood).gameObject;
                    if (destinationSetter.target != targetFood.transform)
                        SeekTarget(targetFood);

                    if (!aiPath.pathPending && aiPath.reachedDestination && targetFood.transform == destinationSetter.target)
                    {
                        destinationSetter.target = null;
                        aiPath.SetPath(null);
                        metabolism.StartEating(targetFood);
                    }
                }
            }

            // 4) If nothing is in range
            if (!metabolism.isEating && !aiPath.hasPath && !aiPath.pathPending)
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


            //Update Animator
            //animator.SetFloat("MoveSpeed", ai.velocity.magnitude);
        }



        #region Movement
        public void PerformAttack()
        {
            //player.TakeDamage(5);
        }


        void SeekTarget(GameObject target)
        {
            destinationSetter.target = target.transform;
        }

        void FleeFromTarget(GameObject target)
        {
            FleePath fleePath = FleePath.Construct(transform.position, target.transform.position, runAwayDistance);
            fleePath.aimStrength = 1;
            fleePath.spread = 4000;
            seeker.StartPath(fleePath);
        }

        void Wander()
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderArea;
            randomDirection += transform.position;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, wanderArea, NavMesh.AllAreas);

            aiPath.destination = hit.position;
            aiPath.SearchPath();
            aiPath.maxSpeed = wanderSpeed;
        }

        void StopEverything()
        {
            metabolism.StopEating();
            destinationSetter.target = null;
            aiPath.SetPath(null);
        }
        #endregion





        #region Utility Functions
        Vector3 GetRandomPoint()
        {
            float xRandom = 0;
            float zRandom = 0;

            xRandom = (float)Random.Range(transform.position.x - wanderArea, transform.position.x + wanderArea);
            zRandom = (float)Random.Range(transform.position.z - wanderArea, transform.position.z + wanderArea);

            return new Vector3(xRandom, 0.0f, zRandom);
        }


        Collider GetClosestTarget(List<Collider> targets)
        {
            Collider bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            foreach (Collider potentialTarget in targets)
            {
                if (potentialTarget != null)
                {
                    Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;
                    if (dSqrToTarget < closestDistanceSqr && potentialTarget.gameObject != this.gameObject)
                    {
                        closestDistanceSqr = dSqrToTarget;
                        bestTarget = potentialTarget;
                    }
                }
            }
            return bestTarget;
        }
        #endregion
    }
}
