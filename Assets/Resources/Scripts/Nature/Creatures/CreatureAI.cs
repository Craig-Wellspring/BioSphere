using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

namespace Pathfinding
{
    public class CreatureAI : VersionedMonoBehaviour
    {
        #region Settings
        public Slider chewTimeBar;
        public Collider selfCollider;

        [Header("Fleeing Settings")]
        public LayerMask fleeLayerMask;
        public float fleeRange;
        public float fleeSpeed = 12f;
        public float runAwayDistance = 10f;
        public List<string> predatorList;

        [Header("Aggression Settings")]
        public LayerMask aggroLayerMask;
        public float aggroRange;
        public float chaseSpeed = 11f;
        public List<string> preyList;

        [Header("Eating Settings")]
        public LayerMask eatingLayerMask;
        public float eatingRange = 10f;
        public float gallopSpeed = 5f;
        public int grazeAtHungerIndex = 2;
        private List<string> dietList;

        [Header("Wander Settings")]
        public float minIdleTime = 1f;
        public float maxIdleTime = 10f;
        public float wanderArea = 10f;
        public float wanderSpeed = 1f;

        [Header("Stats - to be moved")]
        public float power;
        public float toughness;
        public float attackSpeed;
        #endregion

        #region Internal Variables
        [HideInInspector] public bool eating = false;
        private float chewingTimer = 0f;
        private float chewTime;
        private float hasBeenIdle = 0f;
        private float idleTime = 0f;
        private GameObject targetFood;
        private Animator animator;

        IAstarAI ai;
        #endregion

        void OnEnable()
        {
            ai = GetComponent<IAstarAI>();
            if (ai != null) ai.onSearchPath += Update;
        }
        void OnDisable()
        {
            if (ai != null) ai.onSearchPath -= Update;
        }

        void Start()
        {
            animator = GetComponentInParent<Animator>();
            dietList = GetComponentInParent<Metabolism>().dietList;

            //Initialize
            idleTime = Random.Range(minIdleTime, maxIdleTime);
        }

        void FixedUpdate()
        {
            // Check surroundings

            //a) Look for Predators
            List<Collider> fleeFrom = new List<Collider>();
            List<Collider> withinFleeRange = Physics.OverlapSphere(transform.root.position, fleeRange, fleeLayerMask).ToList();
            if (withinFleeRange.Contains(selfCollider))
                withinFleeRange.Remove(selfCollider);
            foreach (Collider col in withinFleeRange)
                if (predatorList.Contains(col.transform.root.tag))
                    fleeFrom.Add(col);


            //b) Look for Prey
            List<Collider> aggroTo = new List<Collider>();
            List<Collider> withinAggroRange = Physics.OverlapSphere(transform.root.position, aggroRange, aggroLayerMask).ToList();
            if (withinAggroRange.Contains(selfCollider))
                withinAggroRange.Remove(selfCollider);
            foreach (Collider col in withinAggroRange)
                if (preyList.Contains(col.transform.root.tag))
                    aggroTo.Add(col);


            //c) Look for Food
            List<Collider> canEat = new List<Collider>();
            List<Collider> withinEatingRange = Physics.OverlapSphere(transform.root.position, eatingRange, eatingLayerMask).ToList();
            if (withinEatingRange.Contains(selfCollider))
                withinEatingRange.Remove(selfCollider);
            foreach (Collider col in withinEatingRange)
                if (dietList.Contains(col.transform.root.tag))
                    canEat.Add(col);





            // 1) Run from Predator if present
            if (fleeFrom.Count > 0)
            {
                if (eating)
                    StopEating();
                FleeFromTarget(GetClosestTarget(fleeFrom).gameObject);
            }

            // 2) Chase target Prey
            if (fleeFrom.Count == 0 && aggroTo.Count > 0)
            {
                if (eating)
                    StopEating();
                ChaseTarget(GetClosestTarget(aggroTo).gameObject);
            }

            // 3) Find Food if hungry and no threats present
            if (fleeFrom.Count == 0 && aggroTo.Count == 0 && canEat.Count > 0 && !eating && GetComponentInParent<Metabolism>().hungerIndex >= grazeAtHungerIndex)
            {
                targetFood = GetClosestTarget(canEat).gameObject;
                RunToFood(targetFood);
            }

            // 4) If nothing is in range
            if (!eating && ai.desiredVelocity.magnitude == 0)
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

        #region Eating
        void Update()
        {
            //Eat
            if (eating)
            {
                if (targetFood != null)
                {
                    //Turn to look at food
                    /*var turnTo = Quaternion.LookRotation(targetFood.transform.position - transform.root.position, transform.root.up);
                    transform.root.rotation = Quaternion.RotateTowards(transform.root.rotation, turnTo, 360 * Time.deltaTime);*/

                    chewingTimer += Time.deltaTime;
                    chewTimeBar.value = chewingTimer;
                    if (chewingTimer > chewTime)
                    {
                        chewingTimer = 0f;
                        FinishEating(targetFood);
                    }
                }
                else StopEating();
            }
        }

        public void RunToFood(GameObject target)
        {
            //Go to food if out of range
            if (!ai.hasPath)
            {
                ai.destination = target.transform.position;
                ai.SearchPath();
                ai.maxSpeed = gallopSpeed;
            }

            //Eat food is in range
            else if (ai.remainingDistance < 0.7f)
            {
                StartEating();
            }
        }

        public void StartEating()
        {
            StopEverything();

            eating = true;
            //animator.SetBool("Eating", true);

            chewTime = targetFood.GetComponent<Food>().timeToEat;
            chewTimeBar.maxValue = targetFood.GetComponent<Food>().timeToEat;
        }

        public void StopEating()
        {
            eating = false;
            //animator.SetBool("Eating", false);

            chewTimeBar.value = 0;
            chewTimeBar.maxValue = 1;
        }

        void FinishEating(GameObject target)
        {
            if (target.gameObject != null)
            {
                //Satiate
                Debug.Log(this.transform.root.name + " ate " + target.transform.root.name);
                GetComponentInParent<Metabolism>().Ingest(target);


                //Destroy food
                if (target.transform.parent != null) /*&& target.transform.parent.childCount == 1*/
                {
                    Destroy(target.transform.parent.gameObject);
                }
                else
                {
                    Destroy(target);
                }
            }

            StopEating();
            targetFood = null;
        }
        #endregion


        #region Combat
        void ChaseTarget(GameObject target)
        {
            if (!ai.hasPath)
            {
                ai.destination = target.transform.position;
                ai.SearchPath();
                ai.maxSpeed = chaseSpeed;
            }

            if (ai.isStopped)
            {
                StopEverything();
            }

            /*if (navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                if (!IsInvoking("PerformAttack"))
                    InvokeRepeating("PerformAttack", .5f, attackSpeed);
            }
            else
            {
                CancelInvoke("PerformAttack");
            }*/
        }

        public void PerformAttack()
        {
            //player.TakeDamage(5);
        }

        void FleeFromTarget(GameObject target)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - target.transform.position);

            Vector3 runToPos = transform.position + transform.forward * runAwayDistance;
            NavMeshHit hit;
            NavMesh.SamplePosition(runToPos, out hit, runAwayDistance, NavMesh.AllAreas);

            ai.destination = hit.position;
            ai.SearchPath();
            ai.maxSpeed = fleeSpeed;
        }
        #endregion

        void Wander()
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderArea;
            randomDirection += transform.position;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, wanderArea, NavMesh.AllAreas);

            ai.destination = hit.position;
            ai.SearchPath();
            ai.maxSpeed = wanderSpeed;
        }

        void StopEverything()
        {
            StopEating();
            ai.SetPath(null);
        }


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
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr && potentialTarget.gameObject != this.gameObject)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget;
                }
            }
            return bestTarget;
        }
        #endregion
    }
}
