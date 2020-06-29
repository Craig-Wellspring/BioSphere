using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morphology : MonoBehaviour
{
    public GameObject carniMorph;
    public GameObject herbiMorph;

    public float energyReserve = 20f;

    public bool logEvolutions = false;
    
    Metabolism metabolism;
    SeedSpawner seedSpawner;

    private bool carnivore = false;
    private GameObject morphToForm;



    private void Start()
    {
        metabolism = GetComponent<Metabolism>();
        seedSpawner = transform.root.GetComponentInChildren<SeedSpawner>(true); //Includes inactive GameObjects
    }



    private void CalculateMorphology()
    {
        if (metabolism.dietHistory.Contains("Meat"))
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

    public void Morph()
    {
        morphToForm = ChooseNewForm();

        //Spawn Seedgrass with excess energy
        seedSpawner.PlantSeed(metabolism.storedEnergy - energyReserve);
        metabolism.storedEnergy = energyReserve;

        //Activate animation that triggers SpawnForm and DespawnForm

        if (logEvolutions)
            Debug.Log(transform.root.name + " is trying to morph into " + morphToForm.name);
    }

    public void SpawnForm()
    {
        //Spawn new Creature Form
        GameObject creatureToSpawn = Instantiate(morphToForm, transform.position, transform.rotation);
        PlanetCore.Core.AlignWithGravity(creatureToSpawn.transform);
        creatureToSpawn.name = morphToForm.name;


        //Allocate Energy
        creatureToSpawn.GetComponentInChildren<Metabolism>().storedEnergy = metabolism.storedEnergy;
        metabolism.storedEnergy = 0;
    }

    public void DespawnForm()
    {
        //Despawn old Creature Form
    }
}
