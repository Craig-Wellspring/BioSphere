using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FertilizePlanet : MonoBehaviour
{
    public float energyReserve;
    public GameObject seedToPlant;
    public float energyToPlant;
    public float plantingArea = 2;

    [Space(10)]
    [SerializeField] bool manualPlantSeed = false;


    private void Start()
    {
        //Spawn new Guardian if destroyed
        GetComponent<OnDestroyEvent>().BeingDestroyed += Servius.Server.GetComponent<GlobalLifeSource>().SpawnMeteor;
    }


    //Plant Seedgrass
    public void PlantSeed()
    {
        if (Servius.Server.GetComponent<GlobalLifeSource>().lifeEnergyPool > energyToPlant + energyReserve)
        {
            GetComponent<ObjectSpawner>().SpawnObject(seedToPlant, plantingArea, true, null, energyToPlant, null);
            Servius.Server.GetComponent<GlobalLifeSource>().lifeEnergyPool -= energyToPlant;
        }
    }

    //Manually plant a seed
    private void OnValidate()
    {
        if (manualPlantSeed)
        {
            PlantSeed();
            manualPlantSeed = false;
        }
    }
}
