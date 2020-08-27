using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

[RequireComponent(typeof(Animator))]
public class BasicAIBrain : VersionedMonoBehaviour
{

    #region Settings
    [Header("Flee Settings")]
    public int runAwayDistance = 10;
    [Header("Wander Settings")]
    public int wanderDistance = 10;
    #endregion


    #region Internal Variables
    //Cache
    EnergyData eData;
    Vitality vitality;
    Metabolism metabolism;
    VisualPerception vPerception;
    Evolution evo;
    Morphology morphology;

    Animator AIBrain;

    AIDestinationSetter destinationSetter;
    Seeker seeker;
    IAstarAI aiPath;
    #endregion


    void Start()
    {
        eData = GetComponentInParent<EnergyData>();
        vitality = GetComponentInParent<Vitality>();
        metabolism = GetComponentInParent<Metabolism>();
        vPerception = GetComponentInParent<VisualPerception>();
        evo = GetComponentInParent<Evolution>();
        morphology = GetComponentInParent<Morphology>();

        AIBrain = GetComponent<Animator>();

        destinationSetter = transform.root.GetComponent<AIDestinationSetter>();
        seeker = transform.root.GetComponent<Seeker>();
        aiPath = transform.root.GetComponent<IAstarAI>();





        metabolism.NowHungry += SetHungry;
        metabolism.NowFull += SetSatiated;
        metabolism.EatingBegins += BeginEating;
        metabolism.EatingEnds += CeaseEating;
        metabolism.WastingBegins += BeginWasting;
        metabolism.WastingEnds += CeaseWasting;

        eData.EnergyAboveSurplus += EnergyAboveSurplus;
        eData.EnergyBelowSurplus += EnergyBelowSurplus;
        evo.EvolutionBeginning += Evolving;

        vitality.DeathOccurs += Dying;

    }

    private void OnDisable()
    {
        metabolism.NowHungry -= SetHungry;
        metabolism.NowFull -= SetSatiated;
        metabolism.EatingBegins -= BeginEating;
        metabolism.EatingEnds -= CeaseEating;
        metabolism.WastingBegins -= BeginWasting;
        metabolism.WastingEnds -= CeaseWasting;

        eData.EnergyAboveSurplus -= EnergyAboveSurplus;
        eData.EnergyBelowSurplus -= EnergyBelowSurplus;
        evo.EvolutionBeginning -= Evolving;

        vitality.DeathOccurs -= Dying;
    }


    void Update()
    {
        // Register Mates
        AIBrain.SetInteger("NearbyMates", vPerception.nearbyMates.Count);

        // Register Predators
        AIBrain.SetInteger("NearbyPredators", vPerception.nearbyPredators.Count);

        // Register Prey
        AIBrain.SetInteger("NearbyPrey", vPerception.nearbyPrey.Count);

        // Register Food
        AIBrain.SetInteger("NearbyFood", vPerception.nearbyFood.Count);

        // Cache distance to target
        AIBrain.SetFloat("TargetDistance", aiPath.remainingDistance);
    }




    void SetHungry()
    {
        AIBrain.SetBool("Hungry", true);
    }
    void SetSatiated()
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

        //Reset pathing
        ClearPathing();
    }

    void BeginWasting()
    {
        AIBrain.SetBool("Wasting", true);
    }
    void CeaseWasting()
    {
        AIBrain.SetBool("Wasting", false);
    }


    void EnergyAboveSurplus()
    {
        AIBrain.SetBool("EnergySurplus", true);
    }
    void EnergyBelowSurplus()
    {
        AIBrain.SetBool("EnergySurplus", false);
    }

    void Evolving()
    {
        //Decide which stat to increase
        /*
        if (metabolism.hungerPercentage >= 60)
            evo.statToEvolve = Evolution.StatToEvolve.MetabolismSpeed;
        else if (vPerception.nearbyFood.Count < 3)
            evo.statToEvolve = Evolution.StatToEvolve.PerceptionRadius;
        else
            evo.statToEvolve = Evolution.StatToEvolve.MaxHealth;
        */

        //Choose stat at random
        int random = Random.Range(1, 3);

        switch (random)
        {
            case 1:
                evo.statToEvolve = Evolution.StatToEvolve.MetabolismSpeed;
                break;

            case 2:
                evo.statToEvolve = Evolution.StatToEvolve.PerceptionRadius;
                break;

            case 3:
                evo.statToEvolve = Evolution.StatToEvolve.MaxHealth;
                break;
        }
    }

    void Dying()
    {
        ClearPathing();
        gameObject.SetActive(false);
    }

    public void ClearPathing()
    {
        destinationSetter.target = null;
        aiPath.SetPath(null);
        aiPath.destination = Vector3.positiveInfinity;
        seeker.CancelCurrentPathRequest();
    }

    /*void StopEverything()
    {
        metabolism.StopEating();
        //ClearPathing();
    }*/
}
