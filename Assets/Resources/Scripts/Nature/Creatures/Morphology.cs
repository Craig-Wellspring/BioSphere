using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morphology : MonoBehaviour
{
    public GameObject carniMorph;
    public GameObject herbiMorph;

    public float energyReserve = 20f;

    public bool logEvolutions = false;
    
    CreatureData cData;
    SeedSpawner seedSpawner;

    private bool carnivore = false;



    private void Start()
    {
        cData = GetComponent<CreatureData>();
        seedSpawner = transform.root.GetComponentInChildren<SeedSpawner>(true); //Includes inactive GameObjects
    }



    private void CalculateMorphology()
    {
        if (cData.lifetimeDiet.Contains("Meat"))
            carnivore = true;
    }

    private GameObject ChooseNewForm()
    {
        CalculateMorphology();

        if (carnivore)
            return carniMorph;
        else
            return herbiMorph;
    }

    public void Evolve()
    {
        GameObject morphToForm = ChooseNewForm();

        //Spawn Seedgrass with excess energy
        seedSpawner.PlantSeed(cData.energyUnits - energyReserve);
        cData.energyUnits = energyReserve;

        //Activate animation that triggers SpawnForm and DespawnForm

        if (logEvolutions)
            Debug.Log(transform.root.name + " is trying to morph into " + morphToForm.name);
    }

    public void SpawnForm()
    {
        //Spawn new Creature Form
            //Allocate energy
    }

    public void DespawnForm()
    {
        //Despawn old Creature Form
    }
}
