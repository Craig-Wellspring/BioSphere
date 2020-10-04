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
        cData = GetComponentInParent<CreatureData>();

        aiBrain = GetComponent<Animator>();
        aiPath = transform.root.GetComponent<AIPathAlignedToSurface>();



        metabolism.EatingBegins += EatingChange;
        metabolism.EatingEnds += EatingChange;
        metabolism.HungerChange += HungerChange;
        metabolism.WastingChange += WastingChange;

        eData.EnergySurplusChange += UpdateEnergySurplus;
        cData.LevelUpBeginning += LevelingUp;

        vitality.DeathOccurs += Dying;

        // Check Energy levels
        eData.SurplusCheck();
        UpdateEnergySurplus();
    }

    private void OnDisable()
    {
        metabolism.EatingBegins -= EatingChange;
        metabolism.EatingEnds -= EatingChange;
        metabolism.HungerChange -= HungerChange;
        metabolism.WastingChange -= WastingChange;

        eData.EnergySurplusChange -= UpdateEnergySurplus;
        cData.LevelUpBeginning -= LevelingUp;

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





    void EatingChange()
    {
        aiBrain.SetBool("Eating", metabolism.isEating);
    }
    void HungerChange()
    {
        aiBrain.SetBool("Hungry", metabolism.isHungry);
    }
    void WastingChange()
    {
        aiBrain.SetBool("Wasting", metabolism.isWasting);
    }


    void UpdateEnergySurplus()
    {
        aiBrain.SetBool("EnergySurplus", eData.energySurplus);
    }

    void LevelingUp()
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
