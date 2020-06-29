using UnityEngine;
//using UnityEngine.AI;
//using System.Linq;
using System.Collections.Generic;
using Pathfinding;

[RequireComponent(typeof(Animator))]
public class AIBrainData : VersionedMonoBehaviour
{

    #region Settings
    [Header("Energy Usage Settings")]
    [Tooltip("Energy Stored is considered In Surplus if beyond this Threshold")]
    public float excessEnergyThreshold;
    //[Header("Flee Settings")]
    //[Tooltip("1000 ~= 1 meter")]
    //public int runAwayDistance = 10000;
    [Header("Wander Settings")]
    public float minIdleTime = 1f;
    public float maxIdleTime = 10f;
    [Tooltip("1000 = 1 meter")]
    public int wanderDistance = 10;
    #endregion


    #region Internal Variables
    public GameObject targetFood;

    private Metabolism metabolism;
    private VisualPerception vPerception;
    private AIDestinationSetter destinationSetter;
    private Seeker seeker;
    IAstarAI aiPath;

    private Animator AIBrain;
    #endregion
    

    void Start()
    {
        metabolism = GetComponent<Metabolism>();
        vPerception = GetComponent<VisualPerception>();

        destinationSetter = transform.root.GetComponent<AIDestinationSetter>();
        seeker = transform.root.GetComponent<Seeker>();
        aiPath = GetComponentInParent<IAstarAI>();

        AIBrain = GetComponent<Animator>();


        metabolism.BecomeHungry += BecomeHungry;
        metabolism.BecomeSatiated += BecomeSatiated;
        metabolism.BeginEating += BeginEating;
        metabolism.CeaseEating += CeaseEating;
        metabolism.BeginWasting += BeginWasting;
        metabolism.CeaseWasting += CeaseWasting;
        metabolism.EnergyGained += EnergyGained;
        metabolism.EnergySpent += EnergySpent;

    }

    private void OnDisable()
    {
        metabolism.BecomeHungry -= BecomeHungry;
        metabolism.BecomeSatiated -= BecomeSatiated;
        metabolism.BeginEating -= BeginEating;
        metabolism.CeaseEating -= CeaseEating;
        metabolism.BeginWasting -= BeginWasting;
        metabolism.CeaseWasting -= CeaseWasting;
        metabolism.EnergyGained -= EnergyGained;
        metabolism.EnergySpent -= EnergySpent;
    }


    void Update()
    {
        /*
        // Check for Mates
        if (vPerception.nearbyMates.Count > 0)
            AIBrain.SetBool("NearbyMate", true);
        else AIBrain.SetBool("NearbyMate", false);

        // Check for Predators
        if (vPerception.nearbyPredators.Count > 0)
            AIBrain.SetBool("NearbyPredator", true);
        else AIBrain.SetBool("NearbyPredator", false);
        
        // Check for Prey
        if (vPerception.nearbyPrey.Count > 0)
            AIBrain.SetBool("NearbyPrey", true);
        else AIBrain.SetBool("NearbyPrey", false);
        */
    
        // Check for Food
        if (vPerception.nearbyFood.Count > 0)
            AIBrain.SetBool("NearbyFood", true);
        else AIBrain.SetBool("NearbyFood", false);
    }


    void BecomeHungry()
    {
        AIBrain.SetBool("Hungry", true);
    }
    void BecomeSatiated()
    {
        AIBrain.SetBool("Hungry", false);
    }

    void BeginEating()
    {
        AIBrain.SetBool("Eating", true);
    }
    void CeaseEating()
    {
        AIBrain.SetBool("Eating", false);
        AIBrain.SetBool("ProxyFood", false);
    }

    void BeginWasting()
    {
        AIBrain.SetBool("Wasting", true);
    }
    void CeaseWasting()
    {
        AIBrain.SetBool("Wasting", false);
    }


    void EnergyGained()
    {
        if (metabolism.storedEnergy >= excessEnergyThreshold)
        {
            AIBrain.SetBool("EnergySurplus", true);
        }
    }
    void EnergySpent()
    {
        if (metabolism.storedEnergy < excessEnergyThreshold)
        {
            AIBrain.SetBool("EnergySurplus", false);
        }
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
}
