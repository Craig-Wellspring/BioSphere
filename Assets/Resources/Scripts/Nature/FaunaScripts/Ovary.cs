using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnergyData))]
public class Ovary : ObjectSpawner
{
    [Tooltip("Return a percentage of the cost required to Lay Eggs and Seeds to the Source when spawned."), SerializeField]
    float returnPercentToSource = 50;

    [Header("Egg Settings")]
    [SerializeField] GameObject eggToSpawn;
    [SerializeField] Color newEggColor;
    [SerializeField] float newEggSize = 1f;
    [SerializeField] float incubationSpeed = 0.5f;
    [SerializeField] bool canHatch = true;

    [Space(10)]
    [SerializeField] GameObject offspringCreature;

    [Header("Seed Settings")]
    [SerializeField] float seedingRadius = 2f;
    [SerializeField] bool randomYRotation = true;
    [Space(10)]
    [SerializeField] GameObject offspringSeed;


    [Header("Debug")]
    [SerializeField] bool logEggLaying = false;
    [SerializeField] bool logSeedLaying = false;


    // Cache
    EnergyData eData;
    void Start()
    {
        eData = GetComponent<EnergyData>();
    }

    #region Eggs
    //// Spawn Egg \\\\
    public void SpawnEgg(float _energyEndowed, GameObject _offspringCreature = null)
    {
        GameObject newEgg = SpawnObject(eggToSpawn, eData, _energyEndowed, returnPercentToSource);

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
            Debug.Log(transform.root.name + " laid an Egg " + "[" + _offspringCreature.name + "]");
    }

    public void LayEggButton()
    {
        Evolution evolution = GetComponent<Evolution>();
        if (eData.energyReserve < evolution.evolutionCost)
        {
            if (Servius.Server.GetComponent<GlobalLifeSource>().lifeEnergyPool > evolution.evolutionCost)
            {
                float energyToLeech = evolution.evolutionCost - eData.energyReserve;
                eData.energyReserve += energyToLeech;
                Servius.Server.GetComponent<GlobalLifeSource>().lifeEnergyPool -= energyToLeech;

                SpawnEgg(evolution.evolutionCost);
            }
            else Debug.LogWarning("Not enough Energy remaining in Global Pool to Spawn Egg");
        }
    }
    #endregion

    #region Seeds
    //// Spawn Seed \\\\
    public void SpawnSeed(float _energyEndowed, GameObject _offspringSeed = null)
    {
        // Choose Cast-off Seed
        if (_offspringSeed == null)
            _offspringSeed = offspringSeed;

        // Expend Energy and plant Seed with the Energy spent to Evolve
        SpawnObject(_offspringSeed, eData, _energyEndowed, returnPercentToSource, null, randomYRotation, seedingRadius);


        // Debug
        if (logSeedLaying)
            Debug.Log(transform.root.name + " planted a Seed " + "[" + offspringSeed.name + "]");
    }

    public void PlantSeedButton()
    {
        Evolution evolution = GetComponent<Evolution>();
        if (eData.energyReserve < evolution.evolutionCost)
        {
            if (Servius.Server.GetComponent<GlobalLifeSource>().lifeEnergyPool > evolution.evolutionCost)
            {
                float energyToLeech = evolution.evolutionCost - eData.energyReserve;
                eData.energyReserve += energyToLeech;
                Servius.Server.GetComponent<GlobalLifeSource>().lifeEnergyPool -= energyToLeech;

                SpawnSeed(evolution.evolutionCost);
            }
            else Debug.LogWarning("Not enough Energy remaining in Global Pool to Spawn Seed");
        }
    }
    #endregion
}
