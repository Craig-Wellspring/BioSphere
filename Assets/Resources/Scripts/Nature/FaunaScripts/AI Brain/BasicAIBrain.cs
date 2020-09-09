using UnityEngine;
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
    //Morphology morphology;
    CreatureData cData;

    [HideInInspector] public Animator aiBrain;
    [HideInInspector] public AIPath aiPath;
    #endregion


    void Start()
    {
        eData = GetComponentInParent<EnergyData>();
        vitality = GetComponentInParent<Vitality>();
        metabolism = GetComponentInParent<Metabolism>();
        vPerception = GetComponentInParent<VisualPerception>();
        evo = GetComponentInParent<Evolution>();
        //morphology = GetComponentInParent<Morphology>();
        cData = GetComponentInParent<CreatureData>();

        aiBrain = GetComponent<Animator>();
        aiPath = transform.root.GetComponent<AIPathAlignedToSurface>();





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
        aiBrain.SetInteger("NearbyMates", vPerception.nearbyMates.Count);

        // Register Predators
        aiBrain.SetInteger("NearbyPredators", vPerception.nearbyPredators.Count);

        // Register Food
        aiBrain.SetInteger("NearbyFood", vPerception.nearbyFood.Count);

        // Cache distance to target
        aiBrain.SetFloat("TargetDistance", aiPath.remainingDistance);
    }




    void SetHungry()
    {
        aiBrain.SetBool("Hungry", true);
    }
    void SetSatiated()
    {
        aiBrain.SetBool("Hungry", false);
    }

    void BeginEating()
    {
        aiBrain.SetBool("Eating", true);
    }
    void CeaseEating()
    {
        aiBrain.SetBool("Eating", false);

        //Reset pathing
        cData.ClearPathing();
    }

    void BeginWasting()
    {
        aiBrain.SetBool("Wasting", true);
    }
    void CeaseWasting()
    {
        aiBrain.SetBool("Wasting", false);
    }


    void EnergyAboveSurplus()
    {
        aiBrain.SetBool("EnergySurplus", true);
    }
    void EnergyBelowSurplus()
    {
        aiBrain.SetBool("EnergySurplus", false);
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

        //Choose stat at random, Random.Max is exclusive
        int random = Random.Range(1, 4);

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
        cData.ClearPathing();
        gameObject.SetActive(false);
    }
}
