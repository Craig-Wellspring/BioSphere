using System;
using System.Collections;
using UnityEngine;

public class FoodData : MonoBehaviour
{
    [Tooltip("On Destroy, entity will spawn Seedgrass with remaining Energy Stored")]
    public float energyStored;
    public float nutritionalValue = 1f;
    public float timeToEat = 5f;
    //public float chewTimeModifier = 1f;
    //public bool currentlyEdible = true;
    [Tooltip("Destroy the parent entity when eaten")]
    public bool destroyParent = true;

    private bool spawnSeed = true;


    private void OnApplicationQuit()
    {
        spawnSeed = false;
    }

    private void OnDisable()
    {
        if (spawnSeed && (energyStored > 0 || nutritionalValue > 0))
            PlantSeed();
    }

    private void PlantSeed()
    {
        //Find seed planting location
        Quaternion newRot = Quaternion.FromToRotation(transform.root.up, (transform.root.position - PlanetCore.Core.transform.position).normalized) * transform.root.rotation;
        //Plant Seedgrass
        GameObject newSeedgrass = (GameObject)Instantiate(Resources.Load<GameObject>("Prefabs/Flora-Kingdom/Grass-Family/Bushgrass-Genus/Seedgrass"), transform.root.position, newRot);
        newSeedgrass.name = "Seedgrass";

        //Pass on remaining energy
        FoodData newFData = newSeedgrass.GetComponentInChildren<FoodData>();
        if (energyStored + nutritionalValue > newFData.nutritionalValue)
            newFData.energyStored = energyStored + nutritionalValue - newFData.nutritionalValue;
        else
            newFData.nutritionalValue = energyStored + nutritionalValue;
    }
}
