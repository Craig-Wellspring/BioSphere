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
    //[Header("Combat Settings")]
    //public float aggression = 50;
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
        // Randomly choose stat to increase
        cData.targetCreatureStat = (CreatureData.TargetCreatureStat)Random.Range(0, System.Enum.GetValues(typeof(CreatureData.TargetCreatureStat)).Length);
    }

    void Dying()
    {
        cData.ClearPathing();
        gameObject.SetActive(false);
    }
}
