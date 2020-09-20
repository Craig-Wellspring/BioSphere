using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnergyData))]
public class Ovary : ObjectSpawner
{
    [Header("Settings")]
    [SerializeField] GameObject eggToSpawn;
    [SerializeField] Color newEggColor;
    [SerializeField] float newEggSize = 1f;
    [SerializeField] float incubationSpeed = 0.5f;
    [SerializeField] bool canHatch = true;

    [Space(10)]
    [SerializeField] GameObject offspringCreature;

    [Space(15)]
    [SerializeField]  GameObject offspringSeed;

    
    [Header("Debug")]
    [SerializeField] bool logEggLaying = false;
    [SerializeField] bool logSeedLaying = false;



    //// Spawn Egg \\\\
    public void SpawnEgg(float _energyEndowed, GameObject _offspringCreature = null)
    {
        GameObject newEgg = SpawnObject(eggToSpawn, 0, false, null, _energyEndowed, GetComponent<EnergyData>());

        // Set offspring
        if (_offspringCreature == null)
            _offspringCreature = offspringCreature;
        newEgg.GetComponentInChildren<HatchCreature>().creatureToHatch = _offspringCreature;

        // Set color
        newEgg.GetComponentInChildren<MeshRenderer>().material.color = newEggColor;

        // Set size
        newEgg.transform.GetChild(0).transform.localScale = new Vector3(newEggSize, newEggSize, newEggSize);
        newEgg.GetComponentInChildren<Rigidbody>().mass = newEggSize;

        // Set incubation speed
        newEgg.GetComponentInChildren<Animator>().SetFloat("IncubationSpeed", incubationSpeed);

        // Set hatchability
        newEgg.GetComponentInChildren<Animator>().SetBool("CanHatch", canHatch);
        

        // Debug
        if (logEggLaying)
            Debug.Log(transform.root.name + " laid an Egg " + "[" + _offspringCreature + "]");
    }

    public void LayEggButton()
    {
        SpawnEgg(GetComponent<EnergyData>().energyReserve);
    }

    //// Spawn Seed \\\\
    public void SpawnSeed(float _energyEndowed, GameObject _offspringSeed = null)
    {
        // Choose Cast-off Seed
        if (_offspringSeed == null)
            _offspringSeed = offspringSeed;

        // Expend Energy and plant Seed with the Energy spent to Evolve
        SpawnObject(_offspringSeed, 2, false, null, _energyEndowed, GetComponent<EnergyData>());


        // Debug
        if (logSeedLaying)
            Debug.Log(transform.root.name + " planted a Seed " + "[" + offspringSeed + "]");
    }

    public void PlantSeedButton()
    {
        SpawnSeed(GetComponent<EnergyData>().energyReserve);
    }
}
