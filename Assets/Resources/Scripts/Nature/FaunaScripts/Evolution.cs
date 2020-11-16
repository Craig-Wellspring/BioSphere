using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(CreatureStats))]
public class Evolution : MonoBehaviour
{
    [Header("State")]
    [SerializeField] int morphLevel = 1;
    [SerializeField] int morphTier = 1;
    [Space(10)]
    public bool energySurplus = false;
    //public float energyMinimum = 0;


    [Header("Settings")]
    [Tooltip("Energy Stored is considered In Surplus if beyond this Threshold")]
    public float surplusThreshold = 50;
    [Space(10)]
    [SerializeField] List<MorphData> possibleMorphs;


    [Header("Debug"), SerializeField]
    bool logMorphs = false;



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
        energySurplus = eData.energyReserve >= surplusThreshold ? true : false;

        if (energySurplus)
            LevelUp();
    }

    void LevelUp()
    {
        if (TryGetComponent<Reproduction>(out Reproduction reproduction))
        {
            if (reproduction.offspringSeed != null)
                reproduction.SpawnSeed(surplusThreshold);
            else eData.ReturnEnergyToReserve(surplusThreshold);
        }
        else eData.ReturnEnergyToReserve(surplusThreshold);

        GetComponent<CreatureStats>()?.TriggerLevelUp();
    }


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
                    switch (_req.requirementType)
                    {
                        case MorphRequirement.RequirementType.Diet:
                            foreach (DietData foodType in GetComponent<Metabolism>().dietHistory)
                                if (foodType.foodTag.Equals(_req.id))
                                {
                                    if (_req.greaterThan)
                                    {
                                        if (foodType.energyUnits >= _req.threshold)
                                        {
                                            reqsMet++;
                                            break;
                                        }
                                    }
                                    else if (foodType.energyUnits <= _req.threshold)
                                    {
                                        reqsMet++;
                                        break;
                                    }
                                }
                            break;

                        case MorphRequirement.RequirementType.Stat:
                            foreach (CreatureStat stat in cStats.statBlock)
                                if (stat.id.Equals(_req.id))
                                {
                                    if (_req.greaterThan)
                                    {
                                        if (stat.value >= _req.threshold)
                                        {
                                            reqsMet++;
                                            break;
                                        }
                                    }
                                    else if (stat.value <= _req.threshold)
                                    {
                                        reqsMet++;
                                        break;
                                    }
                                }
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
        GameObject newCreature = GetComponent<Reproduction>().SpawnObject(_newForm, eData, eData.energyReserve);

        PlayerModule playerModule = transform.root.GetComponentInChildren<PlayerModule>();
        if (playerModule.isControlled)
        {
            playerModule.ReleaseControl();
            newCreature.GetComponentInChildren<PlayerModule>().TakeControl();
        }


        // Pass down Current Level and stat block
        GetComponent<CreatureStats>().CopyCStats(newCreature.GetComponentInChildren<CreatureStats>());

        // Pass down Morphology
        newCreature.GetComponentInChildren<Evolution>().morphTier = morphTier + 1;

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