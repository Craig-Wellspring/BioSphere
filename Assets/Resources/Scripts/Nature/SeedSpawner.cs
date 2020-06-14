using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedSpawner : MonoBehaviour
{
    public void PlantSeed(float _passDownEnergy)
    {
        //Find seed planting location
        Quaternion newRot = Quaternion.FromToRotation(transform.root.up, (transform.root.position - PlanetCore.Core.transform.position).normalized) * transform.root.rotation;
        //Plant Seedgrass
        GameObject newSeedgrass = (GameObject)Instantiate(Resources.Load<GameObject>("Prefabs/Flora-Kingdom/Grass-Family/Bushgrass-Genus/Seedgrass"), transform.root.position, newRot);
        newSeedgrass.name = "Seedgrass";

        //Pass on remaining energy
        FoodData seedFData = newSeedgrass.GetComponentInChildren<FoodData>();
        if (_passDownEnergy > seedFData.nutritionalValue)
            seedFData.energyStored = _passDownEnergy - seedFData.nutritionalValue;
        else
            seedFData.nutritionalValue = _passDownEnergy;
    }
}
