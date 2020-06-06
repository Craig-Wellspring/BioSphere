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
        

    private void OnDisable()
    {
        if (Application.isPlaying)
            PlantSeed();
    }

    void PlantSeed()
    {
        if (energyStored >= 10)
        {
            //Find seed planting location
            Quaternion newRot = Quaternion.FromToRotation(transform.root.up, (transform.root.position - PlanetCore.Core.transform.position).normalized) * transform.root.rotation;
            //Plant Seedgrass
            GameObject newSeedgrass = (GameObject)Instantiate(Resources.Load<GameObject>("Prefabs/Flora-Kingdom/Grass-Family/Bushgrass-Genus/Seedgrass"), transform.root.position, newRot);
            newSeedgrass.name = "Seedgrass";

            //Pass on remaining energy
            var newFData = newSeedgrass.GetComponentInChildren<FoodData>();
            newFData.energyStored = energyStored - newFData.nutritionalValue;
        }

    }
}
