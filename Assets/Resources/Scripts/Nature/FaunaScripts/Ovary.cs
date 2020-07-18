using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnergyData))]
[RequireComponent(typeof(ObjectSpawner))]
public class Ovary : MonoBehaviour
{
    public GameObject eggToSpawn;
    public float reproductionCost;

    [Header("Debug")]
    public bool logEggLaying = false;
    


    //// Spawn Egg with default Baby \\\\
    public void SpawnEgg(float _energyEndowed)
    {
        GetComponent<ObjectSpawner>().SpawnObject(eggToSpawn, 0, false, null, _energyEndowed, GetComponent<EnergyData>());
        

        if (logEggLaying)
            Debug.Log(transform.root.name + " laid an Egg.");
    }

    //// Spawn Egg with custom Baby \\\\
    public void SpawnEgg(float _energyEndowed, GameObject _customBaby)
    {
        GameObject newEgg = GetComponent<ObjectSpawner>().SpawnObject(eggToSpawn, 0, false, null, _energyEndowed, GetComponent<EnergyData>());

        newEgg.GetComponentInChildren<HatchCreature>().creatureToHatch = _customBaby;
        

        if (logEggLaying)
            Debug.Log(transform.root.name + " laid an Egg.");
    }

}
