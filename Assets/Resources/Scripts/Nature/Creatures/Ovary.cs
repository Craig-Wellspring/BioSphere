using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ovary : MonoBehaviour
{
    public GameObject eggToSpawn;
    public float reproductionCost;
    public bool logEggLaying = false;
    
    private Metabolism metabolism;

    void Start()
    {
        metabolism = GetComponent<Metabolism>();
    }
    

    public void SpawnEgg(float _energyEndowed)
    {
        GameObject newEgg = (GameObject)Instantiate(eggToSpawn, transform.position, transform.rotation);
        newEgg.name = eggToSpawn.name;
        
        newEgg.GetComponentInChildren<FoodData>().nutritionalValue += _energyEndowed;
        metabolism.SpendEnergy(_energyEndowed);

        if (logEggLaying)
            Debug.Log(transform.root.name + " laid an Egg.");
    }
    
}
