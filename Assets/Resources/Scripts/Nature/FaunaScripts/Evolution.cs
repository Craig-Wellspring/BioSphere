using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(CreatureStats))]
public class Evolution : ObjectSpawner
{
    [Header("State")]
    [SerializeField] int morphLevel = 1;
    [SerializeField] int morphTier = 1;
    [SerializeField] bool energySurplus = false;


    [Header("Surplus")]
    [Tooltip("Energy Stored is considered In Surplus if beyond this Threshold")]
    [SerializeField] float surplusThreshold = 50;
    [SerializeField] float energyMinimum = 10;


    [Header("Seed Settings")]
    [SerializeField] GameObject offspringSeed = null;

    [Space(10)]
    [Tooltip("Maximum energy passed on to Seed offspring. Energy beyond threshold will be returned to Source. If 0, SeedNV will be used.")]
    [SerializeField] float maxSeedEnergy = 50f;
    [SerializeField, Range(0, 50)] int seedingRadius = 2;
    [Tooltip("Must spawn seed above sea level.")]
    [SerializeField] bool aboveWaterOnly = true;
    [SerializeField] bool randomYRotation = true;
    [SerializeField] bool spawnScale0 = true;


    [Header("Morphology")]
    [SerializeField] List<MorphData> possibleMorphs;



    [Header("Debug")]
    [SerializeField] bool logSeedLaying = false;
    [SerializeField] bool logMorphs = false;



    // Cache
    EnergyData eData;
    CreatureStats cStats;

    void Start()
    {
        eData = GetComponentInParent<EnergyData>();
        eData.EnergyAdded += SurplusQuery;
        eData.EnergyRemoved += SurplusQuery;
        SurplusQuery();

        cStats = GetComponent<CreatureStats>();
        cStats.LevelUpFinishing += AttemptTransMorph;
    }
    void OnDisable()
    {
        eData.EnergyAdded -= SurplusQuery;
        eData.EnergyRemoved -= SurplusQuery;

        cStats.LevelUpFinishing -= AttemptTransMorph;
    }



    // Check for energy surplus, set bool and trigger events
    public void SurplusQuery()
    {
        energySurplus = eData.energyReserve >= surplusThreshold + energyMinimum ? true : false;

        if (energySurplus)
            LevelUp();
    }

    void LevelUp()
    {
        if (offspringSeed != null)
            SpawnSeed(surplusThreshold);
        else eData.ReturnEnergyToSource(surplusThreshold);

        surplusThreshold = Mathf.RoundToInt(surplusThreshold * 1.1f);

        GetComponent<CreatureStats>()?.TriggerLevelUp();
    }

    #region Seeds
    //// Spawn Seed \\\\
    public void SpawnSeed(float _energyEndowed, GameObject _offspringSeed = null)
    {
        // Choose Cast-off Seed
        if (_offspringSeed == null)
            _offspringSeed = offspringSeed;

        // If maxSeedEnergy is 0, use SeedNV as max
        if (maxSeedEnergy == 0)
            maxSeedEnergy = _offspringSeed.GetComponentInChildren<FoodData>().nutritionalValue.y;

        // Calculate excess energy if more than maximum
        if (_energyEndowed > maxSeedEnergy)
        {
            eData.ReturnEnergyToSource(_energyEndowed - maxSeedEnergy, false);
            _energyEndowed = maxSeedEnergy;
        }

        // Expend Energy and plant Seed with the Energy spent
        GameObject spawnedFruit = SpawnObject(_offspringSeed, eData, _energyEndowed, null, randomYRotation, seedingRadius, aboveWaterOnly);


        // Adjust scale
        if (spawnScale0)
            spawnedFruit.GetComponentInChildren<Animator>(true).transform.localScale = Vector3.zero;

        // Debug
        if (logSeedLaying)
            Debug.Log(transform.root.name + " planted a Seed " + "[" + _offspringSeed.name + "]");
    }

    public void PlantSeedButton()
    {
        if (eData.energyReserve < maxSeedEnergy)
        {
            if (Servius.Server.GetComponent<GlobalLifeSource>().energyReserve > maxSeedEnergy)
            {
                float energyToLeech = maxSeedEnergy - eData.energyReserve;
                eData.energyReserve += energyToLeech;
                Servius.Server.GetComponent<GlobalLifeSource>().energyReserve -= energyToLeech;
            }
            else Debug.LogWarning("Not enough Energy remaining in Global Pool to Spawn Seed");
        }

        if (eData.energyReserve >= maxSeedEnergy)
            SpawnSeed(maxSeedEnergy);
    }
    #endregion


    #region Morphology
    //// Check what Forms are eligible to Morph into \\\\
    public void AttemptTransMorph()
    {
        morphLevel += 1;

        foreach (MorphData _morph in possibleMorphs)
        {
            if (morphLevel >= _morph.minMorphLevel)
            {
                int reqsMet = 0;

                foreach (MorphRequirement _req in _morph.requirements)
                {
                    bool disqualified = false;
                    switch (_req.requirementType)
                    {
                        case MorphRequirement.RequirementType.Diet:

                            foreach (DietData foodType in GetComponent<Metabolism>().dietHistory)
                                if (foodType.foodTag.Equals(_req.id))
                                    if (foodType.energyUnits >= _req.threshold)
                                    {
                                        if (_req.greaterThan)
                                            reqsMet++;
                                        else
                                            disqualified = true;

                                        break;
                                    }

                            if (!_req.greaterThan && !disqualified)
                                reqsMet++;

                            break;


                        case MorphRequirement.RequirementType.Stat:

                            foreach (CreatureStat stat in cStats.statBlock)
                                if (stat.id.Equals(_req.id))
                                    if (stat.value >= _req.threshold)
                                    {
                                        if (_req.greaterThan)
                                            reqsMet++;
                                        else
                                            disqualified = true;

                                        break;
                                    }

                            if (!_req.greaterThan && !disqualified)
                                reqsMet++;

                            break;
                    }
                }

                if (reqsMet == _morph.requirements.Count)
                    TransMorph(_morph.morphForm);
            }
        }
    }

    //// Morph into available Form \\\\
    void TransMorph(GameObject _newForm)
    {
        SpawnForm(_newForm);

        //Debug
        if (logMorphs)
            Debug.Log(transform.root.name + " has morphed into " + _newForm.name);

        DespawnForm();
    }



    //// Functions used by Animator: Create new form and destroy old form \\\\
    public void SpawnForm(GameObject _newForm)
    {
        //Spawn new Creature Form
        EnergyData eData = GetComponentInParent<EnergyData>();
        GameObject newCreature = SpawnObject(_newForm, eData, eData.energyReserve);

        // Reset position to match origin
        newCreature.transform.position = transform.position;
        newCreature.transform.rotation = UtilityFunctions.GravityOrientedRotation(newCreature.transform);

        PlayerModule playerModule = transform.root.GetComponentInChildren<PlayerModule>();
        if (playerModule.isControlled)
        {
            playerModule.ReleaseControl();
            PlayerSoul.Cam.SwitchCamTo(newCreature.transform.root.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>());
            newCreature.GetComponentInChildren<PlayerModule>().TakeControl();
        }
        else if (PlayerSoul.Cam.currentTarget.transform.root == this.transform.root)
            PlayerSoul.Cam.SwitchCamTo(newCreature.transform.root.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>());


        // Pass down Current Level and stat block
        GetComponent<CreatureStats>().CopyCStats(newCreature.GetComponentInChildren<CreatureStats>());

        // Pass down Morphology
        Evolution newCreatureEvo = newCreature.GetComponentInChildren<Evolution>();
        newCreatureEvo.morphTier = morphTier + 1;
        newCreatureEvo.energyMinimum *= 2;
        newCreatureEvo.surplusThreshold = surplusThreshold;

        // Pass down Ancestry
        GetComponent<Genetics>().CopyGenes(newCreature.GetComponentInChildren<Genetics>());
    }
    public void DespawnForm()
    {
        //Despawn old Creature Form
        Destroy(transform.root.gameObject);
    }
}


[Serializable]
public class MorphData
{
    public GameObject morphForm;
    public int minMorphLevel = 1;
    public List<MorphRequirement> requirements;


    public MorphData(GameObject _morphForm, int _minMorphLevel, List<MorphRequirement> _requirements)
    {
        this.morphForm = _morphForm;
        this.minMorphLevel = _minMorphLevel;
        this.requirements = _requirements;
    }
}

[Serializable]
public class MorphRequirement
{
    public enum RequirementType { Stat, Diet }
    public RequirementType requirementType;
    public string id;
    [Tooltip("False for Less Than. Inclusive ( >= / <= | null )")]
    public bool greaterThan = true;
    public float threshold = 1;

    public MorphRequirement(RequirementType _requirementType, string _id, bool _greaterThan, float _threshold)
    {
        this.requirementType = _requirementType;
        this.id = _id;
        this.greaterThan = _greaterThan;
        this.threshold = _threshold;
    }
}
#endregion