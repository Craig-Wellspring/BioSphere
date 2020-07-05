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
    
    public bool logMorphs = false;


    #region Private Variables
    //Cache
    Metabolism metabolism;
    Evolution evo;
    SeedSpawner seedSpawner;
    #endregion


    private void Start()
    {
        metabolism = GetComponent<Metabolism>();
        evo = GetComponent<Evolution>();
        seedSpawner = transform.root.GetComponentInChildren<SeedSpawner>(true); //Includes inactive GameObjects
    }

    

    //// Check what Forms are eligible to Morph into \\\\
    public void CalculateMorphology()
    {
        if (metabolism.dietHistory.Contains("Meat"))
            availableMorph = carniMorph;
        else if (metabolism.dietHistory.Contains("Shrub"))
            availableMorph = herbiMorph;
    }

    //// Morph into chosen available Form \\\\
    public void Morph()
    {
        //****Activate animation that triggers SpawnForm and DespawnForm****
        SpawnForm();
        DespawnForm();

        if (logMorphs)
            Debug.Log(transform.root.name + " is trying to morph into " + availableMorph.name);
    }



    //// Functions used by Animator: Create new form and destroy old form \\\\
    public void SpawnForm()
    {
        //Spawn new Creature Form
        GameObject creatureToSpawn = Instantiate(availableMorph, transform.position, transform.rotation);
        PlanetCore.Core.AlignWithGravity(creatureToSpawn.transform);
        creatureToSpawn.name = availableMorph.name;


        //Allocate Energy
        creatureToSpawn.GetComponentInChildren<Metabolism>().GainEnergy(metabolism.storedEnergy);
        metabolism.storedEnergy = 0;
    }
    public void DespawnForm()
    {
        //Despawn old Creature Form
        Destroy(transform.root);
    }
}
