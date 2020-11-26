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
        vitality = transform.root.GetComponentInChildren<Vitality>();
        metabolism = transform.root.GetComponentInChildren<Metabolism>();
        vPerception = transform.root.GetComponentInChildren<VisualPerception>();
        cStats = transform.root.GetComponentInChildren<CreatureStats>();

        aiPath = transform.root.GetComponent<AIPathAlignedToSurface>();
        aiBrain = GetComponent<Animator>();
        animator = transform.root.GetComponent<Animator>();

        vitality.DeathOccurs += Die;

        metabolism.EatingBegins += EatingChange;
        metabolism.EatingEnds += EatingChange;
        metabolism.HungerChange += HungerChange;
        metabolism.WastingChange += WastingChange;

        cStats.LevelUpBeginning += LevelingUp;
    }

    private void OnDisable()
    {
        vitality.DeathOccurs -= Die;

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
        if (!metabolism.isEating)
            ClearPathing();
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
        while (cStats.unappliedLevels > 0)
            cStats.ConfirmLevelUp(cStats.statBlock[Random.Range(0, cStats.statBlock.Count)]);
    }


    void Die()
    {
        this.gameObject.SetActive(false);
    }

    public void ClearPathing()
    {
        transform.root.GetComponent<AIDestinationSetter>().target = null;
        aiPath.SetPath(null);
        aiPath.destination = Vector3.positiveInfinity;
        transform.root.GetComponent<Seeker>().CancelCurrentPathRequest();
    }
}
