using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ovary : MonoBehaviour
{
    public GameObject eggToSpawn;
    public bool logEggLaying = false;
    
    private CreatureData cData;

    void Start()
    {
        cData = GetComponent<CreatureData>();
    }
    

    public void SpawnEgg(float _energyEndowed)
    {
        GameObject newEgg = (GameObject)Instantiate(eggToSpawn, transform.position, transform.rotation);
        newEgg.name = eggToSpawn.name;
        
        newEgg.GetComponent<FoodData>().nutritionalValue += _energyEndowed;
        cData.energyUnits -= _energyEndowed;

        if (logEggLaying)
            Debug.Log(transform.root.name + " laid an Egg.");
    }
    
}
