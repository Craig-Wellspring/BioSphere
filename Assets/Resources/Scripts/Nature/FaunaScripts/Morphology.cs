using UnityEngine;

[RequireComponent(typeof(Evolution))]
public class Morphology : MonoBehaviour
{
    [Header("Current")]
    public GameObject availableMorph = null;


    [Header("Settings")]
    public GameObject carniMorph;
    public GameObject herbiMorph;


    [Header("Debug"), SerializeField]
    bool logMorphs = false;



    // Cache
    Evolution evolution;

    void Start()
    {
        evolution = GetComponent<Evolution>();

        evolution.EvolutionFinishing += CalculateMorphology;
    }
    void OnDisable()
    {
        evolution.EvolutionFinishing -= CalculateMorphology;
    }


    //// Check what Forms are eligible to Morph into \\\\
    public void CalculateMorphology()
    {
        foreach (DietData foodType in GetComponent<Metabolism>().dietHistory)
        {
            if (foodType.foodTag.Contains("Meat") && foodType.energyUnits > 10)
            {
                availableMorph = carniMorph;
                break;
            }
            if (foodType.foodTag.Contains("Grass") && foodType.energyUnits > 50)
            {
                availableMorph = herbiMorph;
                break;
            }
        }

        TryTriggerMorph();
    }

    void TryTriggerMorph()
    {
        if (availableMorph != null)
        {
            transform.root.GetComponent<BioCreatureAnimData>().TriggerMorph();
        }
    }

    //// Morph into available Form \\\\
    public void TransMorph(GameObject _newForm)
    {
        //****Activate animation that triggers SpawnForm and DespawnForm****
        SpawnForm(_newForm);
        DespawnForm();


        //Debug
        if (logMorphs)
            Debug.Log(transform.root.name + " is trying to morph into " + _newForm.name);
    }



    //// Functions used by Animator: Create new form and destroy old form \\\\
    public void SpawnForm(GameObject _newForm)
    {
        EnergyData eData = GetComponent<EnergyData>();
        //Spawn new Creature Form
        GetComponent<Ovary>().SpawnObject(_newForm, 0, false, null, eData.energyReserve, eData);
    }
    public void DespawnForm()
    {
        //Despawn old Creature Form
        Destroy(transform.root.gameObject);
    }
}
