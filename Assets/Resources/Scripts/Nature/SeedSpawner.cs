using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FoodData))]
public class SeedSpawner : MonoBehaviour
{
    private FoodData fData;

    private void Start()
    {
        fData = GetComponent<FoodData>();
    }

    public void PlantSeed()
    {
        //Find seed planting location
        Quaternion newRot = Quaternion.FromToRotation(transform.root.up, (transform.root.position - PlanetCore.Core.transform.position).normalized) * transform.root.rotation;
        //Plant Seedgrass
        GameObject newSeedgrass = (GameObject)Instantiate(Resources.Load<GameObject>("Prefabs/Flora-Kingdom/Grass-Family/Bushgrass-Genus/Seedgrass"), transform.root.position, newRot);
        newSeedgrass.name = "Seedgrass";

        //Pass on remaining energy
        FoodData seedFData = newSeedgrass.GetComponentInChildren<FoodData>();
        if (fData.energyStored + fData.nutritionalValue > seedFData.nutritionalValue)
            seedFData.energyStored = fData.energyStored + fData.nutritionalValue - seedFData.nutritionalValue;
        else
            seedFData.nutritionalValue = fData.energyStored + fData.nutritionalValue;
    }
}
