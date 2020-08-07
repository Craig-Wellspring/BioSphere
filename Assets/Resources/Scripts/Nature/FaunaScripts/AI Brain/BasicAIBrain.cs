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
    private Vitality vitality;
    private Metabolism metabolism;
    private VisualPerception vPerception;
    private Evolution evo;
    private Morphology morphology;

    private Animator AIBrain;

    private AIDestinationSetter destinationSetter;
    private Seeker seeker;
    IAstarAI aiPath;
    #endregion


    void Start()
    {
        vitality = GetComponentInParent<Vitality>();
        metabolism = GetComponentInParent<Metabolism>();
        vPerception = GetComponentInParent<VisualPerception>();
        evo = GetComponentInParent<Evolution>();
        morphology = GetComponentInParent<Morphology>();

        AIBrain = GetComponent<Animator>();

        destinationSetter = transform.root.GetComponent<AIDestinationSetter>();
        seeker = transform.root.GetComponent<Seeker>();
        aiPath = transform.root.GetComponent<IAstarAI>();





        metabolism.NowHungry += BecomeHungry;
        metabolism.NowFull += BecomeSatiated;
        metabolism.EatingBegins += BeginEating;
        metabolism.EatingEnds += CeaseEating;
        metabolism.WastingBegins += BeginWasting;
        metabolism.WastingEnds += CeaseWasting;

        evo.EnergyAboveSurplus += EnergyAboveSurplus;
        evo.EnergyBelowSurplus += EnergyBelowSurplus;
        evo.EvolutionBeginning += Evolving;
        evo.EvolutionFinishing += Morphing;

        vitality.DeathOccurs += Dying;

    }

    private void OnDisable()
    {
        metabolism.NowHungry -= BecomeHungry;
        metabolism.NowFull -= BecomeSatiated;
        metabolism.EatingBegins -= BeginEating;
        metabolism.EatingEnds -= CeaseEating;
        metabolism.WastingBegins -= BeginWasting;
        metabolism.WastingEnds -= CeaseWasting;

        evo.EnergyAboveSurplus -= EnergyAboveSurplus;
        evo.EnergyBelowSurplus -= EnergyBelowSurplus;
        evo.EvolutionBeginning -= Evolving;
        evo.EvolutionFinishing -= Morphing;

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
        int random = Random.Range(0, 2);

        switch (random)
        {
            case 0:
                evo.statToEvolve = Evolution.StatToEvolve.MetabolismSpeed;
                break;

            case 1:
                evo.statToEvolve = Evolution.StatToEvolve.PerceptionRadius;
                break;

            case 2:
                evo.statToEvolve = Evolution.StatToEvolve.MaxHealth;
                break;
        }
    }

    void Morphing()
    {
        //Morph if possible
        if (morphology.availableMorph != null)
            AIBrain.SetTrigger("TriggerMorph");
    }


    void Dying()
    {
        ClearPathing();
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
