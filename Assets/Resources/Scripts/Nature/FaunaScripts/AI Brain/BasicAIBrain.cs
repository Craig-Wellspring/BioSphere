using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Animator))]
public class BasicAIBrain : VersionedMonoBehaviour
{

    #region Settings
    [Header("Feeding Settings")]
    public bool pickRandomFood = true;


    [Header("Flee Settings")]
    public int runAwayDistance = 10;


    [Header("Idle Settings")]
    public bool sings = false;


    //[Header("Combat Settings")]
    //public float aggression = 50;
    #endregion


    #region Internal Variables
    //Cache
    Vitality vitality;
    Metabolism metabolism;
    VisualPerception vPerception;
    CreatureStats cStats;

    AIPath aiPath;
    Animator aiBrain;
    Animator animator;
    #endregion


    void Start()
    {
        vitality = GetComponentInParent<Vitality>();
        metabolism = GetComponentInParent<Metabolism>();
        vPerception = GetComponentInParent<VisualPerception>();
        cStats = GetComponentInParent<CreatureStats>();

        aiPath = transform.root.GetComponent<AIPathAlignedToSurface>();
        aiBrain = GetComponent<Animator>();
        animator = transform.root.GetComponent<Animator>();


        metabolism.EatingBegins += EatingChange;
        metabolism.EatingEnds += EatingChange;
        metabolism.HungerChange += HungerChange;
        metabolism.WastingChange += WastingChange;

        cStats.LevelUpBeginning += LevelingUp;
    }

    private void OnDisable()
    {
        metabolism.EatingBegins -= EatingChange;
        metabolism.EatingEnds -= EatingChange;
        metabolism.HungerChange -= HungerChange;
        metabolism.WastingChange -= WastingChange;

        cStats.LevelUpBeginning -= LevelingUp;
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
        //ClearPathing();
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

    public void ClearPathing()
    {
        transform.root.GetComponent<AIDestinationSetter>().target = null;
        aiPath.SetPath(null);
        aiPath.destination = Vector3.positiveInfinity;
        transform.root.GetComponent<Seeker>().CancelCurrentPathRequest();
    }
}
