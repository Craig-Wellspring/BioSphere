using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnergyData))]
public class Ovary : ObjectSpawner
{
    public bool logEggLaying = false;
    [Header("Settings")]
    public GameObject eggToSpawn;
    


    //// Spawn Egg with default Baby \\\\
    public void SpawnEgg(float _energyEndowed)
    {
        SpawnObject(eggToSpawn, 0, false, null, _energyEndowed, GetComponent<EnergyData>());
        

        if (logEggLaying)
            Debug.Log(transform.root.name + " laid an Egg.");
    }

    //// Spawn Egg with custom Baby \\\\
    public void SpawnEgg(float _energyEndowed, GameObject _customBaby)
    {
        GameObject newEgg = SpawnObject(eggToSpawn, 0, false, null, _energyEndowed, GetComponent<EnergyData>());

        newEgg.GetComponentInChildren<HatchCreature>().creatureToHatch = _customBaby;
        

        if (logEggLaying)
            Debug.Log(transform.root.name + " laid an Egg.");
    }

}
