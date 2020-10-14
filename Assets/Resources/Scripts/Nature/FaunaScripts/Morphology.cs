using UnityEngine;
using System;

[RequireComponent(typeof(CreatureStats), typeof(EnergyData))]
public class Morphology : MonoBehaviour
{
    [Header("Current")]
    [SerializeField] int morphLevel = 1;
    [SerializeField] int morphTier = 1;


    [Header("Settings")]
    [Tooltip("Energy Stored is considered In Surplus if beyond this Threshold")]
    public float surplusThreshold = 50;
    [SerializeField] int minMorphLevel = 3;
    [Space(10)]
    public GameObject carniMorph;
    public GameObject herbiMorph;


    [Header("Debug"), SerializeField]
    public bool energySurplus = false;
    [Space(10)]
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

        if (morphLevel >= minMorphLevel)
        {
            foreach (DietData foodType in GetComponent<Metabolism>().dietHistory)
            {
                if (foodType.foodTag.Contains("Meat") && foodType.energyUnits > 25 && carniMorph != null)
                {
                    TransMorph(carniMorph);
                    break;
                }
                if (foodType.foodTag.Contains("Grass") && foodType.energyUnits > 50 && herbiMorph != null)
                {
                    TransMorph(herbiMorph);
                    break;
                }
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
