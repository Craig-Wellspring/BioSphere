using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Metabolism))]
public class Ovary : MonoBehaviour
{
    public float reproductionCost;
    public GameObject eggToSpawn;
    public bool logEggLaying = false;
    

    //Cache
    private Metabolism metabolism;

    void Start()
    {
        metabolism = GetComponent<Metabolism>();
    }



    //// Spawn Egg with default Baby \\\\
    public void SpawnEgg(float _energyEndowed)
    {
        GameObject newEgg = (GameObject)Instantiate(eggToSpawn, transform.position, transform.rotation);
        newEgg.name = eggToSpawn.name;

        newEgg.GetComponentInChildren<FoodData>().nutritionalValue += _energyEndowed;
        metabolism.SpendEnergy(_energyEndowed);

        if (logEggLaying)
            Debug.Log(transform.root.name + " laid an Egg.");
    }

    //// Spawn Egg with custom Baby \\\\
    public void SpawnEgg(float _energyEndowed, GameObject _customBaby)
    {
        GameObject newEgg = (GameObject)Instantiate(eggToSpawn, transform.position, transform.rotation);
        newEgg.name = eggToSpawn.name;
        newEgg.GetComponentInChildren<HatchCreature>().creatureToHatch = _customBaby;

        newEgg.GetComponentInChildren<FoodData>().nutritionalValue += _energyEndowed;
        metabolism.SpendEnergy(_energyEndowed);

        if (logEggLaying)
            Debug.Log(transform.root.name + " laid an Egg.");
    }

}
