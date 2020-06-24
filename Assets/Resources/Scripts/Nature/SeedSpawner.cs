using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedSpawner : MonoBehaviour
{
    [SerializeField] private GameObject newSeed;
    [SerializeField] private bool spawnSeed = true;

    private float collectedEnergy;

    
    private void OnApplicationQuit()
    {
        spawnSeed = false;
    }

    private void OnDisable()
    {
        if (spawnSeed)
        {
            CollectEnergy();

            if (collectedEnergy > 0)
            {
                PlantSeed(collectedEnergy);
                collectedEnergy = 0;
            }
        }        
    }

    private void CollectEnergy()
    {
        CreatureData cData = transform.root.GetComponentInChildren<CreatureData>();
        if (cData != null)
            if (cData.energyUnits > 0)
            {
                collectedEnergy += cData.energyUnits;
                cData.energyUnits = 0;
            }

        FoodData fData = transform.root.GetComponentInChildren<FoodData>(true);
        if (fData != null)
            if (fData.energyStored > 0 || fData.nutritionalValue > 0)
            {
                collectedEnergy += fData.energyStored + fData.nutritionalValue;
                fData.energyStored = 0;
                fData.nutritionalValue = 0;
            }
    }


    public void PlantSeed(float _passDownEnergy)
    {
        //Find seed planting location
        Quaternion newRot = Quaternion.FromToRotation(transform.root.up, (transform.root.position - PlanetCore.Core.transform.position).normalized) * transform.root.rotation;
        //Plant Seedgrass
        GameObject newSeedgrass = (GameObject)Instantiate(newSeed, transform.root.position, newRot);
        newSeedgrass.name = newSeed.name;

        //Pass on remaining energy
        FoodData seedFData = newSeedgrass.GetComponentInChildren<FoodData>();
        if (_passDownEnergy > seedFData.nutritionalValue)
            seedFData.energyStored = _passDownEnergy - seedFData.nutritionalValue;
        else
            seedFData.nutritionalValue = _passDownEnergy;
    }
}
