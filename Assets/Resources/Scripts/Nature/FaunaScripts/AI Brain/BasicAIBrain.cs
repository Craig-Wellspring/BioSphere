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
    Vitality vitality;
    Metabolism metabolism;
    VisualPerception vPerception;
    CreatureStats cStats;

    [HideInInspector] public Animator aiBrain;
    [HideInInspector] public AIPath aiPath;
    #endregion


    void Start()
    {
        vitality = GetComponentInParent<Vitality>();
        metabolism = GetComponentInParent<Metabolism>();
        vPerception = GetComponentInParent<VisualPerception>();
        cStats = GetComponentInParent<CreatureStats>();

        aiBrain = GetComponent<Animator>();
        aiPath = transform.root.GetComponent<AIPathAlignedToSurface>();



        metabolism.EatingBegins += EatingChange;
        metabolism.EatingEnds += EatingChange;
        metabolism.HungerChange += HungerChange;
        metabolism.WastingChange += WastingChange;

        cStats.LevelUpBeginning += LevelingUp;

        vitality.DeathOccurs += Dying;
    }

    private void OnDisable()
    {
        metabolism.EatingBegins -= EatingChange;
        metabolism.EatingEnds -= EatingChange;
        metabolism.HungerChange -= HungerChange;
        metabolism.WastingChange -= WastingChange;

        cStats.LevelUpBeginning -= LevelingUp;

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


    void LevelingUp()
    {
        // Randomly choose stat to increase
        if (cStats.unappliedLevels > 0)
            cStats.ConfirmLevelUp(cStats.statBlock[Random.Range(0, cStats.statBlock.Count)]);
    }

    void Dying()
    {
        ClearPathing();
        gameObject.SetActive(false);
    }
    
    public void ClearPathing()
    {
        AIDestinationSetter destinationSetter = transform.root.GetComponent<AIDestinationSetter>();
        AIPath aiPath = transform.root.GetComponent<AIPathAlignedToSurface>();
        Seeker seeker = transform.root.GetComponent<Seeker>();

        destinationSetter.target = null;
        aiPath.SetPath(null);
        aiPath.destination = Vector3.positiveInfinity;
        seeker.CancelCurrentPathRequest();
    }
}
