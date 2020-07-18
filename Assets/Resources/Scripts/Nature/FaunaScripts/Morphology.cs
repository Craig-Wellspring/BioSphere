using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morphology : MonoBehaviour
{
    [Header("Current")]
    public GameObject availableMorph = null;

    [Header("Settings")]
    public GameObject carniMorph;
    public GameObject herbiMorph;
    
    [Header("Debug")]
    public bool logMorphs = false;


    #region Private Variables
    //Cache
    Metabolism metabolism;
    #endregion


    private void Start()
    {
        metabolism = GetComponent<Metabolism>();
    }

    

    //// Check what Forms are eligible to Morph into \\\\
    public void CalculateMorphology()
    {
        foreach (DietData foodType in metabolism.dietHistory)
        {
            if (foodType.foodTag.Contains("Meat"))
            {
                availableMorph = carniMorph;
                break;
            }
            if (foodType.foodTag.Contains("Grass"))
            {
                availableMorph = herbiMorph;
                break;
            }
        }
    }

    //// Morph into chosen available Form \\\\
    public void Morph()
    {
        //****Activate animation that triggers SpawnForm and DespawnForm****
        SpawnForm();
        DespawnForm();


        //Debug
        if (logMorphs)
            Debug.Log(transform.root.name + " is trying to morph into " + availableMorph.name);
    }



    //// Functions used by Animator: Create new form and destroy old form \\\\
    public void SpawnForm()
    {
        EnergyData eData = GetComponent<EnergyData>();
        //Spawn new Creature Form
        GetComponent<ObjectSpawner>().SpawnObject(availableMorph, 0, false, null, eData.energyReserve, eData);
    }
    public void DespawnForm()
    {
        //Despawn old Creature Form
        Destroy(transform.root);
    }
}
