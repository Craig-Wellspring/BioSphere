using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(CreatureStats), typeof(EnergyData))]
public class Morphology : MonoBehaviour
{
    [Header("Current")]
    [SerializeField] int morphLevel = 1;
    [SerializeField] int morphTier = 1;
    [Space(10)]
    public bool energySurplus = false;


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
        eData = GetComponent<EnergyData>();
        eData.EnergyAdded += EnergyChange;
        eData.EnergyRemoved += EnergyChange;
        EnergyChange();

        cStats = GetComponent<CreatureStats>();
        cStats.LevelUpFinishing += AttemptTransMorph;
    }
    void OnDisable()
    {
        eData.EnergyAdded -= EnergyChange;
        eData.EnergyRemoved -= EnergyChange;

        cStats.LevelUpFinishing -= AttemptTransMorph;
    }


    void EnergyChange()
    {
        if (SurplusQuery())
        {
            Ovary ovary = GetComponent<Ovary>();
            if (ovary)
                ovary.SpawnSeed(surplusThreshold);
            else eData.ReturnEnergyToReserve(surplusThreshold);

            GetComponent<CreatureStats>()?.TriggerLevelUp();
        }
    }

    // Check for energy surplus, set bool and trigger events
    //public event System.Action EnergySurplusChange;
    public bool SurplusQuery()
    {
        bool _energySurplus = eData.energyReserve >= surplusThreshold ? true : false;
        if (energySurplus != _energySurplus)
        {
            energySurplus = _energySurplus;
            //EnergySurplusChange?.Invoke();
        }
        return _energySurplus;
    }


    //// Check what Forms are eligible to Morph into \\\\
    public void AttemptTransMorph()
    {
        morphLevel += 1;

        List<GameObject> availableForms = new List<GameObject>();
        List<DietData> dietHistory = GetComponent<Metabolism>().dietHistory;

        foreach (MorphData _form in possibleMorphs)
            if (morphLevel >= _form.minMorphLevel)
                foreach (DietData foodType in dietHistory)
                    if (foodType.foodTag.Contains(_form.foodTag) && foodType.energyUnits > _form.energyMinimum)
                    {
                        availableForms.Add(_form.morphForm);
                        break;
                    }

        // If multiple forms are possible at level up, choose one at random
        if (availableForms.Count > 0)
            TransMorph(availableForms[UnityEngine.Random.Range(0, availableForms.Count)]);
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
        EnergyData eData = GetComponent<EnergyData>();
        GameObject newCreature = GetComponent<Ovary>().SpawnObject(_newForm, eData, eData.energyReserve);

        // Pass down Current Level and stat block
        GetComponent<CreatureStats>().CopyCStats(newCreature.GetComponentInChildren<CreatureStats>());

        // Pass down Morphology
        newCreature.GetComponentInChildren<Morphology>().morphTier = morphTier + 1;

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
    public string foodTag;
    public float energyMinimum;

    public MorphData(GameObject _morphForm, int _minMorphLevel, string _foodTag, float _energyMinimum)
    {
        this.morphForm = _morphForm;
        this.minMorphLevel = _minMorphLevel;
        this.foodTag = _foodTag;
        this.energyMinimum = _energyMinimum;
    }
}