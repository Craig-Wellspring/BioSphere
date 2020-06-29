using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatchCreature : MonoBehaviour
{
    public GameObject creatureToHatch;

    
    private void SpawnCreature()
    {
        //Spawn baby creature
        GameObject newBaby = Instantiate(creatureToHatch, transform.position, transform.rotation);
        PlanetCore.Core.AlignWithGravity(newBaby.transform);
        newBaby.name = creatureToHatch.name;


        //Allocate Energy
        FoodData fData = GetComponentInChildren<FoodData>();
        newBaby.GetComponentInChildren<Metabolism>().storedEnergy = fData.nutritionalValue;
        fData.nutritionalValue = 0;
    }


    private void CrackShell()
    {
        Destroy(transform.root.gameObject);
    }
}
